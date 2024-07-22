using System.Xml;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRChatRoom.Server.IRepositories;
using SignalRChatRoom.Server.Models;
using SignalRChatRoom.Server.Models.Dtos;
using SignalRChatRoom.Server.Models.Enums;

namespace SignalRChatRoom.Server.Hubs
{
    // The class must derive from the hub class to understand that it is a hub class and to load its responsibilities.
    public class ChatHub : Hub
    {
        private readonly IRepository<Client> _clientRepository;
        private readonly IRepository<Group> _groupRepository;
        private readonly IRepository<ChatRoom> _chatRoomRepository;
        private readonly IRepository<GroupClient> _groupClientRepository;
        private readonly IRepository<SeenMessageLog> _seenMessageLogRepository;
        public ChatHub(IRepository<Client> clientRepository,
            IRepository<Group> groupRepository,
            IRepository<ChatRoom> chatRoomRepository,
            IRepository<GroupClient> groupClientRepository,
            IRepository<SeenMessageLog> seenMessageLogRepository)
        {
            _clientRepository = clientRepository;
            _groupRepository = groupRepository;
            _chatRoomRepository = chatRoomRepository;
            _groupClientRepository = groupClientRepository;
            _seenMessageLogRepository = seenMessageLogRepository;
        }
        public async Task GetUsernameAsync(string username)
        {
            try
            {
                Client? client = await _clientRepository.GetAll()/*.Include(x => x.Groups)*/.FirstOrDefaultAsync(c => c.Username == username);
                if (client is null)
                {
                    client = new Client(Guid.NewGuid().ToString(), username, Context.ConnectionId, username);
                    // Adds the caller (user joining the system) to the list of all existing clients.
                    await _clientRepository.AddAsync(client);
                }
                else
                {
                    client.AddConnectionId(Context.ConnectionId);
                    await _clientRepository.UpdateAsync(client);
                }

                await UpdateClientMessageHistoryAsync(client.Id,client.ConnectionId);

                // Notifies all clients except the caller (joining client) that a client has joined the system.
                await Clients.Others.SendAsync("clientJoined", username);


                // Notifies all clients of the updated list, including the new user.
                await GetClientsAsync();
                await GetGroupsAsync(client.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateClientMessageHistoryAsync(long clientId,string clientConnectionId, int? pageNumber=null,int? pageSize = null)
        {
            var clientGroupIds = _groupClientRepository.GetAll().Where(x => x.ClientId == clientId).Select(x => x.GroupId).Distinct().ToList();
            var messages = _chatRoomRepository.GetAll()
                .Include(x=>x.ToClient)
                .Include(x=>x.SeenMessageLogs)
                .Where(c => c.ToId == clientId || c.FromId == clientId || (c.GroupId > 0 && clientGroupIds.Contains(c.GroupId.Value)))
                .Paginate<ChatRoom>()
                .Select(x => new ClientMessage()
                {
                    Id = x.Id,
                    CreatedDate = x.CreateDate,
                    IsGroupMessage = x.GroupId > 0 ? true : false,
                    IsSenderReceiver = x.FromId == clientId ? true : false,
                    Message = x.Message,
                    ReceiverClientUsernameOrGroupName = x.GroupId > 0 ? x.Group!.GroupName : x.ToClient!.Username,
                    SenderClientUsername = x.FromClient.Username,
                    Seen = x.Seen,
                    SeenDate = x.SeenDateTime,
                    SeenCount = x.Type == ChatType.Public ? x.SeenMessageLogs.Count() : x.Type == ChatType.Private && x.Seen ? 1 : 0,
                    MessageType = x.MessageType,
                    SecretClientIds = x.SecretClients.Select(x=>x.Id).ToList(),
                }).ToList();

            await Clients.Client(clientConnectionId).SendAsync("updateReceiveMessages", messages);
        }

        public async Task AddSeenMessageLogAsync(long clientId,long messageId)
        {
            var message =await _chatRoomRepository.GetAll()
                .Include(x => x.ToClient)
                .Include(x => x.SeenMessageLogs)
                .FirstOrDefaultAsync(x=>x.Id==messageId);
            if (!message.SeenMessageLogs.Any(x=>x.ClientId==clientId))
            {
                message.AddSeenMessageLog(clientId);
                await _chatRoomRepository.UpdateAsync(message);
                await Clients.All.SendAsync("updateMessageSeen", messageId);
            }
        }

        public async Task GetClientsAsync()
        {
            var clients = await _clientRepository.GetAllAsync();
            // Notifies all clients, including the client joining the system, of the updated client list.
            await Clients.All.SendAsync("clients", clients);
        }

        // Triggered when the caller sends a message to a client.
        public async Task SendMessageAsync(string message, ClientDto client, long? replyId = null, long? forwardId = null)
        {
            try
            {
                // Retrieves the caller information.
                ClientDto? senderClient = await _clientRepository.GetAll().Select(x => new ClientDto
                {
                    Id = x.Id,
                    ConnectionId = x.ConnectionId,
                    Guid = x.Guid,
                    PersianName = x.PersianName,
                    Username = x.Username
                }).FirstOrDefaultAsync(c => c.ConnectionId == Context.ConnectionId);
                if (senderClient is null) { return; }

                var chatRoom = new ChatRoom(senderClient.Id, client.Id, null, message, ChatType.Private, replyId, forwardId);
                await _chatRoomRepository.AddAsync(chatRoom);

                // Triggers the receiveMessage function defined in the client. Returns message, senderClient, and receiverClient values.
                // The receiveMessage function expects 4 values. The 3rd return value expects a Client, which is null in group messaging.
                // The 4th return value expects a string, which is null in individual messaging.
                await Clients.Client(client.ConnectionId).SendAsync("receiveMessage", message, senderClient, client, null);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        //Since any client can create a group, the caller(creating client) is subscribed to the group initially.
        public async Task AddGroupAsync(string groupName)
        {
            if (await _groupRepository.GetAll().AnyAsync(x => x.GroupName == groupName))
            {
                throw new BadHttpRequestException($"another group is exist with {groupName} Name, please try with different Name");
            }

            // The server keeps the information of which clients are in which groups. No need to use ViewModel, etc.
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            Group group = new Group(groupName);

            // Adds the client information to the list held under Group.
            var client = await _clientRepository.GetAll().FirstOrDefaultAsync(c => c.ConnectionId == Context.ConnectionId);

            group.Clients.Add(client);

            // Needs to store the information of which groups exist in the system.
            await _groupRepository.AddAsync(group);

            // Notifies all clients that a group/room has been added to the system.
            await Clients.All.SendAsync("groupAdded", groupName);

            await GetGroupsAsync(client.Id);
        }

        public async Task GetGroupsAsync(long? clientId=null)
        {
            var clientGroupIds = _groupClientRepository.GetAll().Where(x => x.ClientId == clientId).Select(x => x.GroupId).Distinct().ToList();

            var groups = await _groupRepository.GetAll()
                .Where(x => clientGroupIds.Count()>0 &&  clientGroupIds.Contains(x.Id))
                .Select(x => new GroupDto()
            {
                GroupName = x.GroupName,
                Clients = x.Clients.Select(c => new ClientDto()
                {
                    Id = c.Id,
                    ConnectionId = c.ConnectionId,
                    Guid = c.Guid,
                    PersianName = c.PersianName,
                    Username = c.Username
                }).ToList(),
            }).ToListAsync();
            // Notifies all clients, including the client creating the group, of the added rooms/groups in the system.
            // Notifies all clients of the updated group list.
            await Clients.All.SendAsync("groups", groups);
        }
          
        //Adds the caller to the rooms specified in the parameter.
        public async Task AddClientToGroupsAsync(IEnumerable<string> groupNames)
        {
            // Retrieves the client information of the requester (caller).
            Client? client = await _clientRepository.GetAll().FirstOrDefaultAsync(c => c.ConnectionId == Context.ConnectionId);
            if (client == null) return;

            foreach (var groupName in groupNames)
            {
                // Finds the relevant group from the group list in GroupSource using groupName.
                Group? _group = await _groupRepository.GetAll().FirstOrDefaultAsync(g => g.GroupName == groupName);
                if (_group == null) continue;
                // Checks if the caller is subscribed to the relevant group.
                var result = _group.Clients.Any(c => c.ConnectionId == Context.ConnectionId);
                if (!result) // If not subscribed to the relevant group.
                {
                    // Adds the caller client to the client list held in the Group object.
                    _group.Clients.Add(client);
                    await _groupRepository.UpdateAsync(_group);

                    // Adds the relevant client (caller) to the group.
                    await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

                    // Returns the list of all clients belonging to the relevant group.
                    await GetClientsOfGroupAsync(groupName);
                }
            }
        }

        // Adds the caller to the room specified in the parameter.
        public async Task AddClientToGroupAsync(string groupName)
        {
            // Retrieves the client information of the requester (caller).
            Client client = await _clientRepository.GetAll().FirstOrDefaultAsync(c => c.ConnectionId == Context.ConnectionId);

            // Finds the relevant group from the group list in GroupSource using groupName.
            Group _group = await _groupRepository.GetAll().FirstOrDefaultAsync(g => g.GroupName == groupName);

            // Checks if the caller is subscribed to the relevant group.
            //var result = _group.Clients.Any(c => c.ConnectionId == Context.ConnectionId);
            //if (!result) // If not subscribed to the relevant group.
            //{
                // Adds the caller client to the client list held in the Group object.
                _group.Clients.Add(client);
                await _groupRepository.UpdateAsync(_group);

                // Adds the relevant client (caller) to the group.
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

                // Returns the list of all clients belonging to the relevant group.
                await GetClientsOfGroupAsync(groupName);
            //}
        }

        // Returns the list of all clients belonging to the relevant group.
        public async Task GetClientsOfGroupAsync(string groupName)
        {
            GroupDto? group = await _groupRepository.GetAll().Select(x => new GroupDto()
            {
                GroupName = x.GroupName,
                Clients = x.Clients.Select(c => new ClientDto()
                {
                    Id = c.Id,
                    ConnectionId = c.ConnectionId,
                    Guid = c.Guid,
                    PersianName = c.PersianName,
                    Username = c.Username
                }).ToList(),
            }).FirstOrDefaultAsync(g => g.GroupName.Equals(groupName));
            if (group == null) { return; }
            //await Clients.Caller.SendAsync("clientsOfGroup", group.Clients);
            await Clients.Groups(groupName).SendAsync("clientsOfGroup", group.Clients, group.GroupName);
        }

        // Sends a message to the relevant group.
        public async Task SendMessageToGroupAsync(string groupName, string message,List<long>? secretClientIds)
        {
            try
            {
                List<Client> secretClients = new();
                ClientDto senderClient = await _clientRepository.GetAll().Select(c => new ClientDto()
                {
                    Id = c.Id,
                    Username = c.Username,
                    PersianName = c.PersianName,
                    Guid = c.Guid,
                    ConnectionId = c.ConnectionId,
                }).FirstOrDefaultAsync(c => c.ConnectionId == Context.ConnectionId);
                if (secretClientIds != null && secretClientIds.Any())
                {
                    secretClients = await _clientRepository.GetAll().Where(x => secretClientIds.Contains(x.Id)).ToListAsync();
                }
                var group = await _groupRepository.GetAll().FirstOrDefaultAsync(g => g.GroupName == groupName);
                var chatRoom = new ChatRoom(senderClient.Id, null, group.Id, message, ChatType.Public, null, null, secretClients);
                await _chatRoomRepository.AddAsync(chatRoom);
                var clientMessage = new ClientMessage()
                {
                    Id = chatRoom.Id,
                    CreatedDate = chatRoom.CreateDate,
                    IsGroupMessage =true,
                    IsSenderReceiver = false,
                    Message = chatRoom.Message,
                    ReceiverClientUsernameOrGroupName = groupName,
                    SenderClientUsername = senderClient.Username,
                    Seen = false,
                    SeenCount = 0,
                    MessageType = secretClients.Any()?MessageType.Secret:MessageType.Normal,
                    SecretClientIds = secretClientIds
                };

                // Triggers the receiveMessage function in the client. Returns message, senderClient, and groupName values.
                // The receiveMessage function expects 4 values. The 3rd return value expects a Client, which is null in group messaging.
                // The 4th return value expects a string, which is null in individual messaging.
                await Clients.All.SendAsync("groupReceiveMessage", clientMessage);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
 

        //// This function will be triggered when a connection is established with the system.
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("clientJoined", Context.ConnectionId);
        }

        // This function will be triggered when an existing connection is disconnected from the system.
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            //var client = await _clientRepository.GetAll().FirstOrDefaultAsync(c => c.ConnectionId == Context.ConnectionId);
            //if (client == null) { return; }

            //await Clients.All.SendAsync("clientLeaved", client != null ? client.Username : null);

            //// Removes the caller (user joining the system) from the list of all existing clients.
            //await _clientRepository.DeleteAsync(client);

            //// Notifies all clients of the updated list after removing the user.
            //await GetClientsAsync();

            //var groups = await _groupRepository.GetAll().ToListAsync();
            //// Removes the client from the groups they subscribed to.
            //foreach (var group in groups)
            //{
            //    // Checks if the caller is subscribed to the relevant group.
            //    var result = group.Clients.Any(c => c.ConnectionId == Context.ConnectionId);
            //    if (result) // If subscribed to the relevant group.
            //    {
            //        // Removes the caller client from the client list held in the Group object.
            //        group.Clients.Remove(client);
            //        await _groupRepository.UpdateAsync(group);

            //        // Removes the relevant client (caller) from the group.
            //        await Groups.RemoveFromGroupAsync(Context.ConnectionId, group.GroupName);

            //        // Returns the list of all clients belonging to the relevant group.
            //        await GetClientsOfGroupAsync(group.GroupName);
            //    }
            //}
        }
    }
}
