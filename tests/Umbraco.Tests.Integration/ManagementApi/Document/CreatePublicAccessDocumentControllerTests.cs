using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Json;
using Umbraco.Cms.Api.Management.Controllers.Document;
using Umbraco.Cms.Api.Management.ViewModels.PublicAccess;

namespace Umbraco.Cms.Tests.Integration.ManagementApi.Document;

public class CreatePublicAccessDocumentControllerTests : ManagementApiUserGroupTestBase<CreatePublicAccessDocumentController>
{
    protected override Expression<Func<CreatePublicAccessDocumentController, object>> MethodSelector =>
        x => x.Create(Guid.NewGuid(), null);

    protected override UserGroupAssertionModel AdminUserGroupAssertionModel => new()
    {
        ExpectedStatusCode = HttpStatusCode.Created
    };

    protected override UserGroupAssertionModel EditorUserGroupAssertionModel => new()
    {
        ExpectedStatusCode = HttpStatusCode.Created
    };

    protected override UserGroupAssertionModel SensitiveDataUserGroupAssertionModel => new()
    {
        ExpectedStatusCode = HttpStatusCode.Forbidden,
    };

    protected override UserGroupAssertionModel TranslatorUserGroupAssertionModel => new()
    {
        ExpectedStatusCode = HttpStatusCode.Forbidden
    };

    protected override UserGroupAssertionModel WriterUserGroupAssertionModel => new()
    {
        ExpectedStatusCode = HttpStatusCode.Created
    };

    protected override UserGroupAssertionModel UnauthorizedUserGroupAssertionModel => new()
    {
        ExpectedStatusCode = HttpStatusCode.Unauthorized
    };

    protected override async Task<HttpResponseMessage> ClientRequest()
    {
        PublicAccessRequestModel publicAccessRequestModel = new()
        {
            MemberUserNames = null,
            MemberGroupNames = null,
            ErrorPageId = Guid.NewGuid(),
            LoginPageId = Guid.NewGuid(),
        };

        return await Client.PostAsync(Url, JsonContent.Create(publicAccessRequestModel));
    }
}
