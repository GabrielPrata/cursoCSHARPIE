using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

// new {} --->  Usado para criar um objeto anônimo. 
app.MapGet("/User", () => new {Name = "Gabriel Prata", Age = "21"});

// Aqui eu altero a resposta da requisição
// tudo que está depois de => é o corpo do meu método
app.MapGet("/AddHeader", (HttpResponse response) => {
    response.Headers.Add("Teste", "Eu gosto de Pudim");
    return new {Name = "Gabriel Prata", Age = 21};

});

app.MapPost("/SaveProduct", (Product product) => {
    return product.Code + " - " + product.Name;
});

// Formas de passar a informação pela URL

// 1 - Através de Query
//api.app.com/Users?datestart={DataQualquer}&dateend={OutraDataQualquer}
// [FromQuery] é usado para dizer ao servidor que o parâmetro está sendo passado via query
app.MapGet("/GetProduct", ([FromQuery] string dateStart, [FromQuery] string dateEnd) => {
    return dateStart + " - " + dateEnd;
});


// 2 - Através da Rota
// Através da rota as informações são obrigatórias de serem passadass
//api.app.com/User/{code}
// [FromRoute] é usado para dizer ao servidor que o parâmetro está sendo passado via rota
app.MapGet("/GetProduct/{code}", ([FromRoute] string code) => {
    return code;
});


app.MapGet("/GetProductByHeader", (HttpRequest request) => {
    return request.Headers["product-code"].ToString();
});

app.Run();

public class Product {
    public string Code { get; set; }
    public string Name { get; set; }
}

