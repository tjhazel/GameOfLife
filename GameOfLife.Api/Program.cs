using GameOfLife.Api;

var builder = WebApplication.CreateBuilder(args);

var app = AppBuilder.BuildApp(builder, builder.Environment.EnvironmentName == "Development");

//Expose Swagger UI
app.UseSwagger();
app.UseSwaggerUI(options =>
{
   //Expose ui as site root
   options.RoutePrefix = string.Empty;

   options.SwaggerEndpoint("/swagger/v1/swagger.json", "GameOfLife API");
});

// Configure the HTTP request pipeline.

app.UseCors("AllowCors");

app.UseHttpsRedirection();

app.UsePathBase("/api");

//app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
