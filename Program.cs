using Paradas.Interface;
using Paradas.Services;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao contêiner
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost5173",
        policy => policy.WithOrigins("http://localhost:5173")
                       .AllowAnyHeader()
                       .AllowAnyMethod());
});

// 🔹 Obter configurações do appsettings.json
var configuration = builder.Configuration;
var connString = configuration.GetConnectionString("ConnString");
var activeLog = configuration.GetValue<bool>("ActiveLog");

// 🔹 Registrar o serviço corretamente
builder.Services.AddScoped<IParadaService>(provider => new ParadaService(connString, activeLog));
builder.Services.AddScoped<IJustificativaService>(provider => new JustificativaService(connString, activeLog));
builder.Services.AddScoped<ITremService>(provider => new TremService(connString, activeLog));
builder.Services.AddScoped<IMovimentacaoTremService>(provider => new MovimentacaoTremService(connString, activeLog));

var app = builder.Build();

// Configurar o pipeline de requisição HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();  // Redireciona para HTTPS (caso esteja usando HTTPS)
app.UseCors("AllowLocalhost5173");  // Ordem correta para o CORS
app.UseAuthorization();  // Garantir autorização após o CORS

app.MapControllers();  // Mapeia os controladores para as rotas
app.Run();
