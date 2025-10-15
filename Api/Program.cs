using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Adiciona suporte ao Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ativa Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Lista de produtos inicial
var produtos = new List<Produto>
{
    new Produto { Id = 1, Nome = "Teclado", Preco = 150.0 },
    new Produto { Id = 2, Nome = "Mouse", Preco = 80.0 },
    new Produto { Id = 3, Nome = "Monitor", Preco = 1200.0 }
};

// GET todos os produtos
app.MapGet("/produtos", () => Results.Ok(produtos));

// GET produto por id
app.MapGet("/produtos/{id:int}", (int id) =>
{
    var produto = produtos.FirstOrDefault(p => p.Id == id);
    return produto != null ? Results.Ok(produto) : Results.NotFound(new { message = "Produto não encontrado" });
});

// POST criar produto
app.MapPost("/produtos", (Produto produto) =>
{
    if (string.IsNullOrWhiteSpace(produto.Nome) || produto.Preco <= 0)
        return Results.BadRequest(new { message = "Nome ou preço inválido" });

    produto.Id = produtos.Max(p => p.Id) + 1;
    produtos.Add(produto);
    return Results.Created($"/produtos/{produto.Id}", produto);
});

// PUT atualizar produto
app.MapPut("/produtos/{id:int}", (int id, Produto produtoAtualizado) =>
{
    var produto = produtos.FirstOrDefault(p => p.Id == id);
    if (produto == null) return Results.NotFound(new { message = "Produto não encontrado" });

    if (string.IsNullOrWhiteSpace(produtoAtualizado.Nome) || produtoAtualizado.Preco <= 0)
        return Results.BadRequest(new { message = "Nome ou preço inválido" });

    produto.Nome = produtoAtualizado.Nome;
    produto.Preco = produtoAtualizado.Preco;
    return Results.Ok(produto);
});

// DELETE produto
app.MapDelete("/produtos/{id:int}", (int id) =>
{
    var produto = produtos.FirstOrDefault(p => p.Id == id);
    if (produto == null) return Results.NotFound(new { message = "Produto não encontrado" });

    produtos.Remove(produto);
    return Results.NoContent();
});

app.Run();

// Modelo de produto
public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public double Preco { get; set; }
}
