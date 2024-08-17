using CartonCaps.Api.Data.Entities;
using CartonCaps.Api.Infrastructure;
using CartonCaps.Api.Services;
using CartonCaps.Contracts;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CartonCaps.Api.Controllers;

[ApiController]
//[Authorize(Policy = Constants.RequireApiKeyPolicy)]
public class ReferralsController : ControllerBase
{
    private IReferralService _referralService;
    private IValidator<UserReferralEntity> _validator;

    public ReferralsController(
        IReferralService referralService,
        IValidator<UserReferralEntity> validator)
    {
        _referralService = referralService;
        _validator = validator;
    }

    /// <summary>
    /// Given a user id (example: 1), returns the referral code for that user.
    /// </summary>
    /// <param name="userId"></param>
    [HttpGet]
    [Route("api/users/{userId:int}/referal-code")]
    [ProducesResponseType<ReferralCodeResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorList>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUsersReferalCodeAsync([FromRoute] int userId)
    {
        // validate the userId
        var validationResult = _validator.ValidateAsync(new UserReferralEntity { UserId = userId }).Result;
        if (!validationResult.IsValid)
        {
            return BadRequest(new ErrorList(validationResult.Errors.Select(_ => _.ErrorMessage).ToList()));
        }

        var code =  await _referralService.GetReferralCodeAsync(userId);
        return Ok(new ReferralCodeResponse { ReferralCode = code });
    }

    /// <summary>
    /// Given a user Id (example: 1) and a list of referral requests, generates referral tokens for the user.
    /// </summary>
    /// <param name="userId">1</param>
    /// <param name="requests"></param>
    /// <returns>A list of tokens GUIDS for each user, which can be used as a deep link</returns>
    /// <response code="200">Returns the list of tokens</response>
    /// <response code="400">If the user id is invalid</response>
    [HttpPost]
    [Route("api/users/{userId:int}/referal-tokens")]
    [ProducesResponseType<ReferralResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorList>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GenerateReferalLinks([FromRoute] int userId, List<ReferralRequest> requests)
    {
        // validate the userId
        var validationResult = _validator.ValidateAsync(new UserReferralEntity { UserId = userId }).Result;
        if (!validationResult.IsValid)
        {
            return BadRequest(new ErrorList(validationResult.Errors.Select(_ => _.ErrorMessage).ToList()));
        }

        var response = await _referralService.GenerateReferralTokensAsync(userId, requests);
        return Ok(response);
    }

    /// <summary>
    /// Given a user Id (example: 1), returns a list of users referred and their status for that user.
    /// </summary>
    /// <param name="userId">1</param>
    /// <returns>A List of users referred and if they have enrolled</returns>
    /// <response code="200">Returns a list of users referred and if they have enrolled</response>
    /// <response code="400">The user id is invalid</response>
    [HttpGet]
    [Route("api/users/{userId:int}/referal-status")]
    [ProducesResponseType<ReferralStatus>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorList>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetReferralStatus([FromRoute] int userId)
    {
        // validate the userId
        var validationResult = _validator.ValidateAsync(new UserReferralEntity { UserId = userId }).Result;
        if (!validationResult.IsValid)
        {
            return BadRequest(new ErrorList(validationResult.Errors.Select(_ => _.ErrorMessage).ToList()));
        }

        var user = await _referralService.GetReferralStatusAsync(userId);
        return Ok(user);
    }

    /// <summary>
    /// Given a token (GUID) determines if it is valid and returns the info associated to that token
    /// </summary>
    /// <param name="token">dc62edc0-ec23-4074-ab4f-c3ba277a65ed</param>
    [HttpGet]
    [Route("api/referral/{token}/retrieve")]
    [ProducesResponseType<TokenInfoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorList>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTokenInfo([FromRoute] string token)
    {
        var tokenInfo = await _referralService.RetrieveReferralTokenAsync(token);

        if (tokenInfo == null)
        {
            return BadRequest(new ErrorList("Token does not exist."));
        }

        return Ok(tokenInfo);
    }
}
