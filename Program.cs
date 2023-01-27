using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>();

var app = builder.Build();

app.MapPost("/Products", (Product product) => {
    ProductRepository.Add(product);

    //Results é o resultado que vamos retornar para o cliente em forma de status code
    //Código 201 = Created. Utilizado quando o post dá certo e cria o dado.
    //Com isso, no header na resposta é retornado o location, que ajuda o cliente
    //a saber onde a informação foi salva, ajudando a saber qual a URI 
    //para obter a informção que acabou de ser salva
    return Results.Created($"/Products/{product.Code}", product.Code); 

});

app.MapGet("/Products/{code}", ([FromRoute] string code) => {
    var product = ProductRepository.GetBy(code);
    if(product != null){
        return Results.Ok(product);
    }else{
        return Results.NotFound();
    }
});

app.MapPut("/Products", (Product product) => {
    var productSaved = ProductRepository.GetBy(product.Code);
    productSaved.Name = product.Name;
    return Results.Ok();

});

app.MapDelete("/Products/{code}", ([FromRoute] string code) => {
    var productSaved = ProductRepository.GetBy(code);
    ProductRepository.Remove(productSaved);
    return Results.Ok();

});

app.Run();


// Classe estática pois assim ela não é gerada novamente a cada requisição,
// ficando salva na memória do servidor
public static class ProductRepository{
    public static List<Product> Products { get; set; }   

    public static void Add(Product product){
        if(Products == null){
            Products = new List<Product>();
        }
        Products.Add(product);
    }
    public static Product GetBy(string code) {
        //Aqui busco o dado, e caso ele não seja encontrado retorno algo como padrão
        return Products.FirstOrDefault(p => p.Code == code);
    }

    public static void Remove(Product product) {
        Products.Remove(product);
    } 

}

public class Product {
    public string Code { get; set; }
    public string Name { get; set; }
}

public class ApplicationDbContext : DbContext {
    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlServer(
        "Server=localhost;Database=Products;User Id=sa;Password=@Abcd1234;MultiplrActiveResultSets=true;Encrypt=YES;TrustServerCertificate=YES"
    );

}

