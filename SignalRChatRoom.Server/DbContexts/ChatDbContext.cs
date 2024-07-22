using Microsoft.EntityFrameworkCore;
using SignalRChatRoom.Server.Models;

namespace SignalRChatRoom.Server.DbContexts
{
    public class ChatDbContext : DbContext
    {
        private readonly IServiceProvider _serviceProvider;
        public ChatDbContext(DbContextOptions<ChatDbContext> options, IServiceProvider serviceProvider) : base(options)
        {
            _serviceProvider = serviceProvider;
        }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<Group> Groups{ get; set; }
        public DbSet<SeenMessageLog> SeenMessageLogs { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ChatRoom>()
                .HasOne(cr => cr.FromClient)
                .WithMany(c => c.SentMessages)
                .HasForeignKey(cr => cr.FromId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatRoom>()
                .HasOne(cr => cr.ToClient)
                .WithMany(c => c.ReceivedMessages)
                .HasForeignKey(cr => cr.ToId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatRoom>()
                .HasOne(cr => cr.Group)
                .WithMany(c => c.Messages)
                .HasForeignKey(cr => cr.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.Groups)
                .WithMany(e => e.Clients)
                .UsingEntity<GroupClient>();

            modelBuilder.Entity<ChatRoom>()
                .HasMany(cr => cr.SeenMessageLogs)
                .WithOne(s => s.ChatRoom)
                .HasForeignKey(s=>s.ChatRoomId);
        }
    }
}
