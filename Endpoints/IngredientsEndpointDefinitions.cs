using DishesAPI.EndpointFilters;
using DishesAPI.EndpointHandlers;

namespace DishesAPI.Endpoints
{
    public class IngredientsEndpointDefinitions : IEndpointDefinition
    {
        public void RegisterEndpoints(IEndpointRouteBuilder builder)
        {
            var ingredientsEndPoint = builder.MapGroup("dishes/{dishId:guid}/ingredients")
                .RequireAuthorization() // l'autorizzazione 
                .WithTags("Ingredients")
                .AddEndpointFilter<PerformanceTrakingFilter>()
                .AddEndpointFilter<LogNotFoundResponseFilter>();


            ingredientsEndPoint.MapGet("", IngredientsHandlers.GetIngredientsAsync)
                .WithSummary("Get all ingredients")
                .WithDescription("Returns all ingredients, optionally filtered by name"); ;
        }
    }
}
