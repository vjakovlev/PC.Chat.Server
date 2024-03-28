using PC.Chat.Server.HubConfig;
using PC.Chat.Server.Models;
using PC.Chat.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddSignalR(o => o.StatefulReconnectBufferSize = 1000);
builder.Services.AddSignalR();

builder.Services.AddSingleton<IDictionary<string, UserRoomConnection>>(options => 
{
    return new Dictionary<string, UserRoomConnection>();
});

builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddCors(options => 
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseRouting();
app.UseCors();

app.MapHub<ChatHub>("/chat");

//app.MapHub<ChatHub>("/chat", options =>
//{
//    options.AllowStatefulReconnects = true;
//});


app.MapControllers();
app.Run();
