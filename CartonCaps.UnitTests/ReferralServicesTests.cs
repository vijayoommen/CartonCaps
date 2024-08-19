using AutoFixture;
using CartonCaps.Api.Data.Entities;
using CartonCaps.Api.Data.Repositories;
using CartonCaps.Api.Services;
using CartonCaps.Contracts;
using Moq;

namespace CartonCaps.UnitTests;

public class ReferralServicesTests
{
    private Mock<IUserRepo> _userRepoMock;
    private Mock<IReferralsRepo> _referralsRepoMock;
    private ReferralService _referralService;
    private Fixture _fixture;

    public ReferralServicesTests()
    {
        _userRepoMock = new Mock<IUserRepo>();
        _referralsRepoMock = new Mock<IReferralsRepo>();
        _referralService = new ReferralService(_referralsRepoMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetReferralToken_should_be_successfullAsync()
    {
        _referralsRepoMock.Setup(_ => _.GetReferralUserAsync(It.IsAny<int>()))
            .ReturnsAsync(_fixture.Build<UserReferralEntity>().With(_ => _.UserId, 1).Create());

        var requests = _fixture.Build<ReferralRequest>().CreateMany(2).ToList();
        var responses = await _referralService.GenerateReferralTokensAsync(1, requests);

        Assert.IsType<List<ReferralResponse>>(responses);
        responses.ForEach(response =>
        {
            Assert.Contains(requests, r => r.Email == response.Email && r.Phone == response.Phone && r.Name == response.Name);
            Assert.True(!string.IsNullOrEmpty(response.ReferralToken));
        });
    }

    [Fact]
    public async Task GetReferralToken_should_return_null_for_invalid_user()
    {
        _referralsRepoMock.Setup(_ => _.GetReferralUserAsync(It.IsAny<int>())).ReturnsAsync((UserReferralEntity?)null);

        var requests = _fixture.Build<ReferralRequest>().CreateMany(2).ToList();
        var responses = await _referralService.GenerateReferralTokensAsync(99999, requests);

        Assert.Empty(responses);
    }

    [Fact]
    public async Task GetReferralStatus_should_be_successfull()
    {
        var referralEntity = _fixture.Build<UserReferralEntity>().With(_ => _.UserId, 1).Create();
        _referralsRepoMock.Setup(_ => _.GetReferralUserAsync(It.IsAny<int>()))
            .ReturnsAsync(referralEntity);

        var response = await _referralService.GetReferralStatusAsync(1);

        Assert.IsType<ReferralStatus>(response);
        Assert.True(response.ReferralCode == referralEntity.ReferralCode);

    }

    [Fact]
    public async Task GetReferralStatus_for_invalid_userId_should_not_be_successfull()
    {
        _referralsRepoMock.Setup(_ => _.GetReferralUserAsync(It.IsAny<int>()))
            .ReturnsAsync((UserReferralEntity?)null);

        var response = await _referralService.GetReferralStatusAsync(1);

        Assert.True(response.ReferralCode == string.Empty);

    }

    [Fact]
    public async Task RetrieveUserByToken_should_contain_one_token_record()
    {
        var targetReferral = _fixture.Build<Referred>().With(_ => _.Token, "TESTTOKEN").Create();
        var otherReferrals = _fixture.Build<Referred>().CreateMany(2);
        var referralEntity = _fixture.Build<UserReferralEntity>()
            .With(_ => _.Referred, new List<Referred> { targetReferral }.Concat(otherReferrals).ToList())
            .Create();

        _referralsRepoMock.Setup(_ => _.GetUserByToken(It.IsAny<string>()))
            .ReturnsAsync(referralEntity);

        var response = await _referralService.RetrieveReferralTokenAsync("TESTTOKEN");

        Assert.IsType<TokenInfoResponse>(response);
        Assert.True(response.Name == targetReferral.Name 
                    && response.Email == targetReferral.Email 
                    && response.Name == targetReferral.Name 
                    && response.Token == targetReferral.Token);
        Assert.True(response.ReferralCode == referralEntity.ReferralCode);
    }

    [Fact]
    public async Task RetrieveUserByToken_should_ignore_other_records()
    {
        var targetReferral = _fixture.Build<Referred>().With(_ => _.Token, "TESTTOKEN").Create();
        var otherReferrals = _fixture.Build<Referred>().CreateMany(2);
        var referralEntity = _fixture.Build<UserReferralEntity>()
            .With(_ => _.Referred, new List<Referred> { targetReferral }.Concat(otherReferrals).ToList())
            .Create();

        _referralsRepoMock.Setup(_ => _.GetUserByToken(It.IsAny<string>()))
            .ReturnsAsync(referralEntity);

        var response = await _referralService.RetrieveReferralTokenAsync("TESTTOKEN");

        Assert.IsType<TokenInfoResponse>(response);
        otherReferrals.ToList().ForEach(other =>
        {
            Assert.False(response.Name == other.Name
                        && response.Email == other.Email
                        && response.Name == other.Name
                        && response.Token == other.Token);
        });
    }
}