using Microsoft.AspNetCore.Builder;
using api.Services;
using api.Repositories;
using api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


builder.Services.AddDbContext<WordCountContext>(options =>
    options.UseSqlite("Data Source=wordcount.db"));

builder.Services.AddScoped<IWordCountRepository, WordCountRepository>();
builder.Services.AddScoped<IWordCountSummaryService, WordCountSummaryService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WordCountContext>();
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();


