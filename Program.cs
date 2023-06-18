using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ONLINE_SCHOOL_BACKEND.Data;
using ONLINE_SCHOOL_BACKEND.Models;
using Microsoft.AspNetCore.Cors;

namespace ONLINE_SCHOOL_BACKEND
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("OnlineSchoolingAppContextConnection") ?? throw new InvalidOperationException("Connection string 'CollegeContextConnection' not found.");
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<OnlineSchoolDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddIdentity<OnlineSchoolUser, IdentityRole>()
        .AddEntityFrameworkStores<OnlineSchoolDbContext>()
        .AddDefaultTokenProviders();

            builder.Services.AddCors();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}