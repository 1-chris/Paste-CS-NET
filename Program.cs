using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PasteDbContext>(option => option.UseSqlite(connectionString));
builder.Services.AddHostedService<ExpiredCleanupWorkerService>();

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

    return paste?.Content is not null ? Results.Text(paste.Content) : Results.NotFound();
});

app.MapGet("/stats", async (PasteDbContext db) =>
{
    var count = await db.Pastes.CountAsync();

    return count;
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

public class ExpiredCleanupWorkerService : BackgroundService
{
    readonly ILogger<ExpiredCleanupWorkerService> _logger;
    private readonly IServiceProvider _services;

    public ExpiredCleanupWorkerService(ILogger<ExpiredCleanupWorkerService> logger, IServiceProvider services)
    {
        _logger = logger;
        _services = services;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Expired paste cleanup worker running at: {time}", DateTimeOffset.Now);

            using (var scope = _services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<PasteDbContext>();

                var expiredPastes = await dbContext.Pastes
                    .Where(paste => paste.Expiry < DateTime.UtcNow)
                    .ToListAsync();

                if (expiredPastes.Any())
                {
                    _logger.LogInformation("Removing pastes: {count}", expiredPastes.Count);
                    foreach (var paste in expiredPastes)
                    {
                        dbContext.Pastes.Remove(paste);
                    }
                    await dbContext.SaveChangesAsync();

                }
            }

            await Task.Delay(10000, stoppingToken);
        }
    }
}