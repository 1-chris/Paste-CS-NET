using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Diagnostics;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PasteDbContext>(option => option.UseSqlite(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/paste", async (Paste paste, PasteDbContext db) =>
{
    if (paste.Expiry is null)
        paste.Expiry = new DateTime(DateTime.UtcNow.AddDays(7).Ticks);

    paste.Id = Guid.NewGuid();
    db.Pastes.Add(paste);
    await db.SaveChangesAsync();

    return paste;
});

app.MapGet("/paste/{id}", async (Guid id, PasteDbContext db) =>
{
    var paste = await db.Pastes.FindAsync(id);

    if (paste?.Expiry < DateTime.UtcNow)
    {
        db.Pastes.Remove(paste);
        await db.SaveChangesAsync();

        return Results.NotFound();
    }

    return paste?.Content is not null ? Results.Text(paste.Content) : Results.NotFound();
});

app.UseDefaultFiles();
app.UseStaticFiles("");

app.Run();

public class Paste
{
    public Guid Id { get; set; }
    public DateTime? Expiry { get; set; } = new DateTime(DateTime.UtcNow.AddDays(7).Ticks);
    public string Language { get; set; } = "Plaintext";
    public string Content { get; set; } = "";

};

public class PasteDbContext : DbContext
{
    public DbSet<Paste> Pastes => Set<Paste>();

    public PasteDbContext(DbContextOptions<PasteDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Paste>().HasData(new List<Paste>(){ 
            new Paste {
                Id = Guid.NewGuid(),
                Expiry = new DateTime(DateTime.UtcNow.AddYears(100).Ticks),
                Language = "Plaintext",
                Content = "Dummy paste"
            } 
        }.AsEnumerable());
    }

}