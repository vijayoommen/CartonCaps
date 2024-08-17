using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace CartonCaps.Api.Infrastructure.Helpers;

internal class AuthenticationHelpers
{
    public static void SetupAuthentication(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer();

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(Constants.RequireApiKeyPolicy, policy =>
            {
                policy.AddAuthenticationSchemes(new[] { JwtBearerDefaults.AuthenticationScheme });
                policy.Requirements.Add(new ApiKeyRequirement());
            });
        });
        builder.Services.AddScoped<IAuthorizationHandler, ApiKeyRequirementHandler>();
    }

}
