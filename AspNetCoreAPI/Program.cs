using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    //options.Audience = "https://localhost:7299";
    options.MetadataAddress = "https://adfs.contoso.com/adfs/.well-known/openid-configuration";
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {        
        ValidAudiences = new string[] { "https://localhost:7299", "api://webapi" },
        ValidIssuer = "http://adfs.contoso.com/adfs/services/trust"
    };
    //Don't do this in production
    options.BackchannelHttpHandler = new HttpClientHandler()
    {
        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true,
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = async (context) =>
  {
      var result = context.Result;
  }
    };
});

builder.Logging.AddConsole();

var app = builder
    .Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
