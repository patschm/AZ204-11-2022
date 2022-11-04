using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var conf = builder.Configuration;
builder.Services.AddMicrosoftIdentityWebApiAuthentication(conf);
builder.Services.AddControllers();
builder.Services.AddCors(cf=>{
    cf.AddPolicy("alles", pol=>{
        pol.AllowAnyHeader();
        pol.AllowAnyMethod();
        pol.AllowAnyOrigin();
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("alles");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
