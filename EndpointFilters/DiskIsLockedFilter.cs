namespace DishesAPI.EndpointFilters
{
    public class DiskIsLockedFilter: IEndpointFilter
    {
        private readonly Guid _lockedDishId = Guid.Parse("fd630a57-2352-4731-b25c-db9cc7601b16");

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var dishIdValue = context.HttpContext.Request.RouteValues["dishId"];

            if (dishIdValue is not null && Guid.TryParse(dishIdValue.ToString(), out var dishId) &&
                dishId == _lockedDishId)
            {
                return TypedResults.Problem(
                    statusCode: StatusCodes.Status403Forbidden,
                    title: "Dish is locked",
                    detail: $"The dish with id {dishId} is locked and cannot be modfied");
            }

            return await next(context);
        }
    }
}
