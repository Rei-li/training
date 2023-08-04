﻿using ActionableEmailsTest;
namespace ActionableEmailsTest.Controllers;

public static class MessageEndpoints
{
    public static void MapMessageEndpoints (this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/api/Message", () =>
        {
            var data = File.ReadAllText("storage.txt");
            return new [] { new Message() { Data = data } };
        })
        .WithName("GetAllMessages")
        .Produces<Message[]>(StatusCodes.Status200OK);

        routes.MapGet("/api/Message/{id}", (int id) =>
        {
            //return new Message { ID = id };
        })
        .WithName("GetMessageById")
        .Produces<Message>(StatusCodes.Status200OK);

        routes.MapPut("/api/Message/{id}", (int id, Message input) =>
        {
            return Results.NoContent();
        })
        .WithName("UpdateMessage")
        .Produces(StatusCodes.Status204NoContent);

        routes.MapPost("/api/Message/", (Message model) =>
        {
            File.WriteAllText("storage.txt", model.Data);
            //return Results.Created($"/api/Messages/{model.ID}", model);
        })
        .WithName("CreateMessage")
        .Produces<Message>(StatusCodes.Status201Created);

        routes.MapDelete("/api/Message/{id}", (int id) =>
        {
            //return Results.Ok(new Message { ID = id });
        })
        .WithName("DeleteMessage")
        .Produces<Message>(StatusCodes.Status200OK);
    }
}
