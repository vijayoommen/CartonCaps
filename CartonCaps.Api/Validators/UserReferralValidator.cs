using CartonCaps.Api.Data.Entities;
using CartonCaps.Api.Data.Repositories;
using FluentValidation;

namespace CartonCaps.Api.Validators;

public class UserReferralValidator : AbstractValidator<UserReferralEntity>
{
    public UserReferralValidator(IUserRepo userRepo)
    {
        RuleFor(x => x.UserId)
            .Custom((userId, context) =>
            {
                var user = userRepo.GetUserAsync(userId).Result;
                if (user == null)
                {
                    context.AddFailure("User not found");
                }
            });
    }
}
