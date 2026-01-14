using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persons.Models;
using Persons.Server;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataBaseContext>(o => o.UseSqlite(connectionString));
var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", () => Results.BadRequest());
app.MapGet("/persons", async (DataBaseContext context) =>
{
    var persons = await context.Persons.ToListAsync();
    return persons.Count == 0 ? Results.NotFound() : Results.Ok(persons);
});
app.MapGet("/persons/{id:int}", async (int id, DataBaseContext context) =>
{
    var person = await context.Persons.SingleOrDefaultAsync(p => p.Id == id);
    return person == null ? Results.NotFound() : Results.Ok(person);
});
app.MapGet("/persons/{name}", async (string name, DataBaseContext context) =>
{
    var persons = await context.Persons
        .ToListAsync();
    var filteredPersons = persons
        .Where(p => p.LastName.Contains(name, StringComparison.OrdinalIgnoreCase)
                    || p.FirstName.Contains(name, StringComparison.OrdinalIgnoreCase))
        .ToList();
    return filteredPersons.Count == 0 ? Results.NotFound() : Results.Ok(filteredPersons);
});
app.MapPost("/persons", async (Person person, DataBaseContext context) =>
{
    var addedPerson = await context.Persons.AddAsync(person);
    await context.SaveChangesAsync();
    return Results.Created($"/persons/{addedPerson.Entity.Id}", addedPerson.Entity);
});

await app.RunAsync();