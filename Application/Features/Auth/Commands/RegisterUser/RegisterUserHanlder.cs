using Application.Abstractions.Interfaces.Services;
using Domain.Abstractions;
using MediatR;

namespace Application.Features.Auth.Commands.RegisterUser;

internal class RegisterUserHanlder(IIdentityService identityService) : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        => await identityService.RegisterAsync(new RegisterCommand.IdentityRegisterDto
        {
            Email = request.Email,
            FullName = request.FullName,
            Password = request.Password,
            PrefferLanguage = request.PrefferLanguage
        }, cancellationToken);
}
