using Tema2.BookInfo;
using Tema2.DB;
using Tema2.BookInfo.Commands;
using Tema2.BookInfo.Queries;
using Tema2.BookInfo.Handlers;
using Tema2.BookInfo.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite("Data Source=books.db"));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.MapPost("/books", async (IMediator m, CreateBookCommand cmd) =>
{
    var id = await m.Send(cmd);
    return Results.Created($"/books/{id}", new { id });
});

app.MapPut("/books/{id:int}", async (IMediator m, int id, UpdateBookCommand body) =>
{
    await m.Send(body with { Id = id });
    return Results.NoContent();
});

app.MapDelete("/books/{id:int}", async (IMediator m, int id) =>
{
    await m.Send(new DeleteBookCommand(id));
    return Results.NoContent();
});

app.MapGet("/books/{id:int}", (IMediator m, int id) => m.Send(new GetBookByIdQuery(id)));

app.MapGet("/books", (IMediator m, int page = 1, int pageSize = 10)
    => m.Send(new GetBooksQuery(page, pageSize)));

app.Run();