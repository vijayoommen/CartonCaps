using CartonCaps.Api.Controllers;
using CartonCaps.Api.Data;
using CartonCaps.Api.Data.Repositories;
using CartonCaps.Api.Services;
using CartonCaps.Api.Validators;
using CartonCaps.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CartonCaps.TestHarness;

public class Referrals
{
    private ReferralsController _controller;

    public Referrals()
    {
        var dataSource = new DataSource();
        var referralRepo = new ReferralsRepo(dataSource);
        var referralService = new ReferralService(referralRepo);
        var userRepo = new UserRepo(dataSource);
        var validator = new UserReferralValidator(userRepo);
        _controller = new ReferralsController(referralService, validator);
    }

    [Fact]
    public async Task GetReferralCodeAsync_for_valid_user_isSuccessful()
    {
        var result = await _controller.GetUsersReferalCodeAsync(1);
        Assert.IsType<OkObjectResult>(result);
        var value = ((Microsoft.AspNetCore.Mvc.ObjectResult)result).Value;
        Assert.IsType<ReferralCodeResponse>(value);
    }

    [Fact]
    public async Task GetReferralCodeAsync_for_invalid_user_isNotSuccessful()
    {
        var result = await _controller.GetUsersReferalCodeAsync(100);
        Assert.IsType<BadRequestObjectResult>(result);
        var value = ((Microsoft.AspNetCore.Mvc.ObjectResult)result).Value;
        Assert.IsType<ErrorList>(value);
    }

    [Fact]
    public async Task GetReferralTokens_for_valid_user_new_requests_isSuccessful()
    {
        var result = await _controller.GenerateReferalLinks(1, new List<ReferralRequest>
        {
            new ReferralRequest { Email = "testone@example-email.com", Phone = "" },
            new ReferralRequest { Email = "anothertest@example-email.com", Phone = "713-001-1111" },
        });
        Assert.IsType<OkObjectResult>(result);
        var value = ((Microsoft.AspNetCore.Mvc.ObjectResult)result).Value;
        Assert.IsType<List<ReferralResponse>>(value);
    }

    [Fact]
    public async Task GetReferralTokens_are_identical_for_repeat_requests_IsSuccessful()
    {
        // first time
        var firstResult = await _controller.GenerateReferalLinks(1, new List<ReferralRequest>
        {
            new ReferralRequest { Email = "testone@example-email.com", Phone = "7131231234" },
            new ReferralRequest { Email = "anothertest@example-email.com", Phone = "8321231236" },
        });

        // second time - repeat request
        var secondResult = await _controller.GenerateReferalLinks(1, new List<ReferralRequest>
        {
            new ReferralRequest { Email = "testone@example-email.com", Phone = "7131231234" },
            new ReferralRequest { Email = "anothertest@example-email.com", Phone = "8321231236" },
        });

        Assert.IsType<OkObjectResult>(secondResult);
        var firstResponse = ((Microsoft.AspNetCore.Mvc.ObjectResult)firstResult).Value;
        var secondResponse = ((Microsoft.AspNetCore.Mvc.ObjectResult)secondResult).Value;
        Assert.Equivalent(firstResponse, secondResponse, strict: true);
    }

    [Fact]
    public async Task GetReferralStatus_for_valid_user_isSuccessful()
    {
        var result = await _controller.GetReferralStatus(1);
        Assert.IsType<OkObjectResult>(result);
        var value = ((Microsoft.AspNetCore.Mvc.ObjectResult)result).Value;
        Assert.IsType<ReferralStatus>(value);
    }

    [Fact]
    public async Task GetReferralStatus_for_invalid_user_isNotSuccessful()
    {
        var result = await _controller.GetReferralStatus(100);
        Assert.IsType<BadRequestObjectResult>(result);
        var value = ((Microsoft.AspNetCore.Mvc.ObjectResult)result).Value;
        Assert.IsType<ErrorList>(value);
    }

    [Fact]
    public async Task GetTokenInfo_for_valid_token_isSuccessful()
    {
        var result = await _controller.GetTokenInfo("dc62edc0-ec23-4074-ab4f-c3ba277a65ed");
        Assert.IsType<OkObjectResult>(result);
        var value = ((Microsoft.AspNetCore.Mvc.ObjectResult)result).Value;
        Assert.IsType<TokenInfoResponse>(value);
    }

    [Fact]
    public async Task GetTokenInfo_for_invalid_token_isSuccessful()
    {
        var result = await _controller.GetTokenInfo("invalid-token-info-here");
        Assert.IsType<BadRequestObjectResult>(result);
    }
}