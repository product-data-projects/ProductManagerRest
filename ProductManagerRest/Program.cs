namespace ProductManagerRest
{
    public class Program
    {
        public static void Main(string[] args)
        {

            // create container & add services
            var builder = WebApplication.CreateBuilder(args);
            {
                builder.Services.AddAuthorization();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
                builder.Services.AddDbContext<ProductManagerDatabase.Database.ProductManagerContext>();
            }
            

            // build app and run
            var app = builder.Build();
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseHttpsRedirection();
                app.UseAuthorization();

                GeneratedEndpoints.Config.ConfigureMinimalApi(app);
            }
            app.Run();

        }

    }

}