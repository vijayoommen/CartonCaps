using Microsoft.AspNetCore.Authorization;

namespace CartonCaps.Api.Infrastructure;

public class ApiKeyRequirement : IAuthorizationRequirement { }

public class ApiKeyRequirementHandler : AuthorizationHandler<ApiKeyRequirement>
{
    private IHttpContextAccessor _httpContextAccessor;

    public ApiKeyRequirementHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiKeyRequirement requirement)
    {
        if (_httpContextAccessor?.HttpContext?.Request.Headers.TryGetValue(Constants.ApiKeyName, out var apiKey) ?? false)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        context.Fail();
        return Task.CompletedTask;
    }
}
