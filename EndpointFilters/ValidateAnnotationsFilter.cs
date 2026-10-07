using System.ComponentModel.DataAnnotations;

namespace DishesAPI.EndpointFilters
{
    public class ValidateAnnotationsFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            foreach (var argument in context.Arguments)
            {
                if (argument is null) continue;
                var validationResult = new List<ValidationResult>();
                var validationContext = new ValidationContext(argument);

                if (!Validator.TryValidateObject(argument, validationContext, validationResult,
                        validateAllProperties: true))
                {
                    return TypedResults.ValidationProblem(
                        validationResult
                            .Where(vr => vr.ErrorMessage is not null)
                            .ToDictionary(
                                vr => vr.MemberNames.FirstOrDefault() ?? string.Empty,
                                vr => new[] { vr.ErrorMessage! }
                            )
                    );
                }

                if (argument is IValidatableObject validatable)
                {
                    var customResult = validatable.Validate(validationContext).ToList();

                    if (customResult.Count > 0 && customResult.Any(r => r != ValidationResult.Success))
                    {
                        return TypedResults.ValidationProblem(
                            customResult.Where(vr => vr != ValidationResult.Success && vr.ErrorMessage is not null)
                                .ToDictionary(vr => vr.MemberNames.FirstOrDefault() ?? string.Empty,
                                    vr => new[] { vr.ErrorMessage! })
                        );
                    }
                }


            }

            return await next(context);
        }
    }
}
