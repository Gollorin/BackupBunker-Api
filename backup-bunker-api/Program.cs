using backup_bunker_api.Models;

namespace backup_bunker_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            FireStoreManager.SetFirestoreDatabase();



            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // CORS - BLBOST
            builder.Services.AddCors(p => p.AddPolicy("corspolicy", builder =>
            {
                builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
            }));


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // ENABLE CORS
            app.UseCors("corspolicy");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}