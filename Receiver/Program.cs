using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using reciever;
using reciever.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
builder.Services.AddSingleton<Receive>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});
var app = builder.Build();
app.MapHub<Hubs>("/orderHub");

// Start the RabbitMQ receiver in the background
var receiver = app.Services.GetRequiredService<Receive>();
_ = receiver.StartReceiving();

app.Run();