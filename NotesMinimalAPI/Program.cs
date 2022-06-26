using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Connect to PostgreSQL Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<NoteDb>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Welcome to Notes API!");

app.MapGet("/notes/{id:int}", async (int id, NoteDb db) =>
{
    return await db.Notes.FindAsync(id)
            is Note n
                ? Results.Ok(n)
                : Results.NotFound();
});

app.MapPost("/notes/", async (Note n, NoteDb db) =>
{
    db.Notes.Add(n);
    await db.SaveChangesAsync();

    return Results.Created($"/notes/{n.id}", n);
});

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<NoteDb>();
    dataContext.Database.Migrate();
}

app.Run();

record Note(int id)
{
    public string text { get; set; } = default!;
    public bool done { get; set; } = default!;
}

class NoteDb : DbContext
{
    public NoteDb(DbContextOptions<NoteDb> options) : base(options)
    {

    }
    public DbSet<Note> Notes => Set<Note>();
}
