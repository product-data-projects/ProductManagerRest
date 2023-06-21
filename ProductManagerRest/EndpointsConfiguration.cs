using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PmContext = ProductManagerDatabase.Database.ProductManagerContext;

namespace ProductManagerRest
{
    public static class EndpointsConfiguration
    {

        public static void MapEndpointsFromDbContext(this WebApplication webApplication)
        {

         
            var props = typeof(PmContext)
                            .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                            .Where(w => w.PropertyType.Name.Equals("DbSet`1"));


            foreach (var dbSet in props)
            {

                var entityType = dbSet.PropertyType.GenericTypeArguments[0];

                var mapGroup = webApplication
                                    .MapGroup($"/{entityType.Name}")
                                    .WithOpenApi()
                                    .WithTags(new[] { entityType.Name });


                mapGroup.MapGet("/", ([FromServices] PmContext dbContext) => Get(dbContext, entityType)).WithName($"GetAll{entityType.Name}s").WithOpenApi();

                mapGroup.MapGet("/{id}", (int id, [FromServices] PmContext dbContext) => Get(id, dbContext, entityType)).WithName($"Get{entityType.Name}");

                mapGroup.MapDelete("/{id}", (int id, [FromServices] PmContext dbContext) => Delete(id, dbContext, entityType)).WithName($"Delete{entityType.Name}");


            }

        }

      
        /// <summary>
        /// Return a single entity from the database using its primary key.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dbContext"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        static async Task<Results<Ok<object>, NotFound>> Get(object id, PmContext dbContext, Type entityType)
        {
            var result = await dbContext.FindAsync(entityType, id);
            return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
        }

        static async Task<Results<Ok<object>, NotFound>> Get(PmContext dbContext, Type entityType)
        {
            var result = await dbContext.FindAsync(entityType);
            return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
        }

        static async Task<Results<Ok, NotFound>> Delete(object id, PmContext dbContext, Type entityType)
        {
            var result = await dbContext.FindAsync(entityType, id);
            if (result is null)
            {
                return TypedResults.NotFound();
            }
            else
            {
                var res = dbContext.Remove(result);
                return TypedResults.Ok();
            }

        }

    }

}
