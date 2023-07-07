using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//De forma automatizada o C# já interpreta que esse configuration é do appsettings.json
var config = app.Configuration;

//Salvo os produtos na classe Product Repository
ProductRepository.Init(config);

app.MapPost("/Products", (Product product) =>
{
    ProductRepository.AddProduct(product);

    //Para retornar um código HTTP:
    //Created recebe dois parâmetros, o primeiro irá retornar no header da resposta e o segundo no Body
    return Results.Created($"/Products/{product.Code}", product.Code);
});

app.MapGet("/Products/{code}", ([FromRoute] int code) =>
{
    var productSaved = ProductRepository.GetBy(code);

    //Para retornar NotFound preciso também retornar Ok
    if (productSaved != null)
    {
        return Results.Ok(productSaved);
    }
    else
    {
        return Results.NotFound();
    }
});

app.MapPut("/Products", (Product product) =>
{
    var productSaved = ProductRepository.GetBy(product.Code);
    productSaved.Name = product.Name;

    //O PUT e o DELETE eu posso retornar 200
    return Results.Ok();
});

//Posso configurar para que determinada funcionalidade não funcione em um determinado ambiente
//Abaixo, configuro para rodar apenas no ambiente de Staging

app.MapDelete("/Products/{code}", ([FromRoute] int code) =>
{
    var productSaved = ProductRepository.GetBy(code);
    ProductRepository.RemoveProduct(productSaved);

    return Results.Ok();
});

if (app.Environment.IsStaging())
{
    app.MapGet("/Configuration/Database", (IConfiguration config) =>
    {
        return Results.Ok($"{config["Database:Connection"]}:{config["database:port"]}");
    });
}

app.Run();

public static class ProductRepository
{

    //O static é para manter os valores salvos na memória, pois se não ele apagará e enviará
    //dados novos a cada requisição
    public static List<Product> Products { get; set; } = new List<Product>();

    public static void Init(IConfiguration config)
    {
        //Pego os valores salvos no JSON com GetSection (pego a sessão Products)
        //e armazeno na lista
        var products = config.GetSection("Products").Get<List<Product>>();

        //Após isso salvo o conteúdo desta lista interna na classe Products
        Products = products;
    }

    public static void AddProduct(Product product)
    {
        Products.Add(product);
    }

    public static void RemoveProduct(Product product)
    {
        Products.Remove(product);
    }

    public static Product GetBy(int code)
    {
        //Retorna o primeiro código encontrado com First()
        //FirstOrDefault() retorna valores padrão 0, null, false...
        //Expressão abaixo: Onde o código é igual ao código que eu passei como parâmetro para o método
        return Products.FirstOrDefault(p => p.Code == code);
    }
}

public class Product
{
    public int Code { get; set; }
    public string Name { get; set; }
}

//Atraves do DbSet a classe ApplicationDbContext entende que ela deve ser mapeada para o banco de dados
public class ApplicationDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

}