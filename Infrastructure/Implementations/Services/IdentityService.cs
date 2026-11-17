using Application.Abstractions.Interfaces;
using Application.Abstractions.Interfaces.Services;
using Application.Features.Auth.DTOs;
using Application.Features.Auth.LoginCommand;
using Application.Features.Auth.Queries.ProfileQuery;
using Application.Features.Auth.RegisterCommand;
using Domain.Abstractions;
using Infrastructure.Extensions;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Implementations.Services;

internal class IdentityService(UserManager<ApplicationUser> userManager, TimeProvider timeProvider, IUnitOfWork unitOfWork) : IIdentityService
{
    public async Task<Result<UserDataDTO>> GetUserDataByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return Result<UserDataDTO>.Failure(Error.NotFound("User.NotFound", "User not found"));

        var roles = await userManager.GetRolesAsync(user);

        return new UserDataDTO(userId, user.Email!, user.UserName!, user.IsActive, roles.AsReadOnly());
    }

    public async Task<Result<LoginIdentityResponseDto>> LoginAsync(LoginCommandDto dto, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);

        if (user is null)
            return Result<LoginIdentityResponseDto>.Failure(Error.Unauthorized("Auth.InvalidCredentials", "Email or password not valid"));

        cancellationToken.ThrowIfCancellationRequested();

        if (!await userManager.CheckPasswordAsync(user, dto.Password))
            return Result<LoginIdentityResponseDto>.Failure(Error.Unauthorized("Auth.InvalidCredentials", "Email or password not valid"));

        cancellationToken.ThrowIfCancellationRequested();

        var roles = await userManager.GetRolesAsync(user);

        return new LoginIdentityResponseDto(user.Id, user.Email!, user.UserName!, user.IsActive, roles);
    }

    public async Task<Result<ProfileQueryDto>> ProfileAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return Result<ProfileQueryDto>.Failure(Error.Unauthorized("Auth.UnAuthonticated", "Un authenticated"));

        cancellationToken.ThrowIfCancellationRequested();

        var roles = await userManager.GetRolesAsync(user);

        return new ProfileQueryDto(userId, user.Email!, user.FullName, user.PreferredLanguage, roles);
    }

    public async Task<Result> RegisterAsync(RegisterCommandDto dto, CancellationToken cancellationToken)
    {
        var userModel = ApplicationUser.Create(dto.Email, dto.Email, dto.FullName, dto.PrefferLanguage, timeProvider.GetUtcNow());

        if (userModel.IsFailure)
            return Result.Failure(userModel.Errors.ToArray());

        return await unitOfWork.ExecuteInTransactionAsync(async (ct) =>
        {
            var createResult = (await userManager.CreateAsync(userModel.Value!, dto.Password)).ToResult();

            if (createResult.IsFailure)
                return createResult;

            ct.ThrowIfCancellationRequested();

            var roleResult = await userManager.AddToRoleAsync(userModel.Value!, "Donor");

            return roleResult.ToResult();
        }, cancellationToken);
    }
}
