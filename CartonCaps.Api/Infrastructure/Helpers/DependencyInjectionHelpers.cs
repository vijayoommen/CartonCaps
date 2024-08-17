using CartonCaps.Api.Data;
using CartonCaps.Api.Data.Repositories;
using CartonCaps.Api.Services;
using FluentValidation;

internal static class DependencyInjectionHelpers
{
    public static void SetupDependencyInjection(WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IDataSource, DataSource>();
        builder.Services.AddTransient<IReferralsRepo, ReferralsRepo>();
        builder.Services.AddTransient<IUserRepo, UserRepo>();
        builder.Services.AddScoped<IReferralService, ReferralService>();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();
    }
}