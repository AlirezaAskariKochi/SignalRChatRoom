using Microsoft.EntityFrameworkCore;
using SignalRChatRoom.Server.DbContexts;
using SignalRChatRoom.Server.Hubs;
using SignalRChatRoom.Server.IRepositories;
using SignalRChatRoom.Server.Models;
using SignalRChatRoom.Server.Repositories;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ChatDbContext>(options =>
   options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IRepository<Client>, Repository<Client>>();
builder.Services.AddTransient<IRepository<Group>, Repository<Group>>();
builder.Services.AddTransient<IRepository<ChatRoom>, Repository<ChatRoom>>();
builder.Services.AddTransient<IRepository<GroupClient>, Repository<GroupClient>>();
builder.Services.AddTransient<IRepository<SeenMessageLog>, Repository<SeenMessageLog>>();
builder.Services.AddScoped(typeof(GenericController<>));

// CORS politikalar� ayarlar�..
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyMethod().AllowAnyHeader().AllowCredentials().SetIsOriginAllowed(origin => true)));

// Sisteme WebSocket protokol�n� kullanaca�� bildiriliyor yani SignalR mod�l� devreye sokuluyor..
builder.Services.AddSignalR();

var app = builder.Build();

// Ayarlanan CORS politikas�n� kullanabilmek i�in ilgili middleware �a��r�l�yor..
app.UseCors();

// Hub�n hangi endpointte kullan�laca�� bildiriliyor..
app.MapHub<ChatHub>("/chathub");

app.Run();
