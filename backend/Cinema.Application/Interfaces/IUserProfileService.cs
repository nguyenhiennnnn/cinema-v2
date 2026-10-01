using Cinema.Application.DTOs.Users;

namespace Cinema.Application.Interfaces;

public interface IUserProfileService
{
    Task<UserProfileDto> GetMyProfileAsync(int userId, CancellationToken cancellationToken);
    Task<UserProfileDto> UpdateMyProfileAsync(
        int userId,
        UpdateProfileRequest request,
        CancellationToken cancellationToken
    );
}
