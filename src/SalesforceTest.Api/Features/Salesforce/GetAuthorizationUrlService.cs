using SalesforceTest.Api.Common;
using SalesforceTest.Api.Interfaces;

namespace SalesforceTest.Api.Features.Salesforce;

public sealed class GetAuthorizationUrlService
{
    private readonly ISalesforceOAuthService _oauthService;

    public GetAuthorizationUrlService(ISalesforceOAuthService oauthService)
    {
        _oauthService = oauthService;
    }

    public Result<string> Execute(Guid userId)
    {
        var state = userId.ToString();
        var url = _oauthService.BuildAuthorizationUrl(state);
        return Result.Success(url);
    }
}
