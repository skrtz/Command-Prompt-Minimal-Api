using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SixMinApi.Data;
using SixMinApi.Dtos;
using SixMinApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("SQLLightDbConnection") ?? "Data Source=CommandsDb.db";

builder.Services.AddSqlite<AppDbContext>(connectionString);
builder.Services.AddScoped<ICommandRepo, CommandRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("api/commands", async (ICommandRepo repo, IMapper mapper) =>
{
    var commands = await repo.GetAllCommandsAsync();
    return Results.Ok(mapper.Map<IEnumerable<CommandReadDto>>(commands));
});

app.MapGet("api/commands/{id}", async (int id, ICommandRepo repo, IMapper mapper) =>
{
    var command = await repo.GetCommandByIdAsync(id);
    if (command == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(mapper.Map<CommandReadDto>(command));
});

app.MapPost("api/commands", async (CommandCreateDto cmdCreateDto, ICommandRepo repo, IMapper mapper) =>
{
    var command = mapper.Map<Command>(cmdCreateDto);

    await repo.CreateCommandAsync(command);
    await repo.SaveChangesAsync();

    var cmdReadDto = mapper.Map<CommandReadDto>(command);

    return Results.Created($"/api/commands/{cmdReadDto.Id}", cmdReadDto);
});

app.MapPut("api/commands/{id}", async (int id, CommandUpdateDto cmdUpdateDto, ICommandRepo repo, IMapper mapper) =>
{
    var command = await repo.GetCommandByIdAsync(id);
    if (command == null)
    {
        return Results.NotFound();
    }

    mapper.Map(cmdUpdateDto, command);

    await repo.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("api/commands/{id}", async (int id, ICommandRepo repo) =>
{
    var command = await repo.GetCommandByIdAsync(id);
    if (command == null)
    {
        return Results.NotFound();
    }

    repo.DeleteCommand(command);
    await repo.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();

