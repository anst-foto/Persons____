using Microsoft.EntityFrameworkCore;
using Persons.Models;

namespace Persons.Server;

public class DataBaseContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    
    public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options) { }
}