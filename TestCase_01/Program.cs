
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using TestCase_01_DataAccess.Data;
using TestCase_01_DataAccess.DataAccess.Repository;
using TestCase_01_DataAccess.Repository.IReposaitory;
using TestCase_01_DataAccess.Service.IService;
using TestCase_01_DataAccess.Service;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;

namespace TestCase_01
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.

            //builder.Services.AddDbContext<ApplicationDbContext>(options =>
            //  options.UseInMemoryDatabase("DefaultInMemoryConnection"));


            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySQL(connectionString));


            builder.Services.AddScoped<IUnitofWork, UnitOfWork>();

        
            builder.Services.AddScoped<ITestCaseService, TestCaseService>();

            builder.Services.AddAutoMapper(typeof(MappingConfig));
            builder.Services.AddCors(options => {
                options.AddPolicy("AllowAll", policy => {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
