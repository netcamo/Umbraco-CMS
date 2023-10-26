using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Services.AuthorizationStatus;

namespace Umbraco.Cms.Core.Services;

/// <inheritdoc />
internal sealed class MediaPermissionsService : IMediaPermissionsService
{
    private readonly IMediaService _mediaService;
    private readonly IEntityService _entityService;
    private readonly AppCaches _appCaches;

    public MediaPermissionsService(
        IMediaService mediaService,
        IEntityService entityService,
        AppCaches appCaches)
    {
        _mediaService = mediaService;
        _entityService = entityService;
        _appCaches = appCaches;
    }

    /// <inheritdoc/>
    public async Task<MediaAuthorizationStatus> AuthorizeAccessAsync(IUser performingUser, IEnumerable<Guid> mediaKeys)
    {
        foreach (Guid mediaKey in mediaKeys)
        {
            IMedia? media = _mediaService.GetById(mediaKey);
            if (media is null)
            {
                return MediaAuthorizationStatus.NotFound;
            }

            if (performingUser.HasPathAccess(media, _entityService, _appCaches) == false)
            {
                return MediaAuthorizationStatus.UnauthorizedMissingPathAccess;
            }
        }

        return MediaAuthorizationStatus.Success;
    }

    /// <inheritdoc/>
    public async Task<MediaAuthorizationStatus> AuthorizeRootAccessAsync(IUser performingUser)
        => performingUser.HasMediaRootAccess(_entityService, _appCaches)
            ? MediaAuthorizationStatus.Success
            : MediaAuthorizationStatus.UnauthorizedMissingRootAccess;

    /// <inheritdoc/>
    public async Task<MediaAuthorizationStatus> AuthorizeBinAccessAsync(IUser performingUser)
        => performingUser.HasMediaBinAccess(_entityService, _appCaches)
            ? MediaAuthorizationStatus.Success
            : MediaAuthorizationStatus.UnauthorizedMissingBinAccess;
}
