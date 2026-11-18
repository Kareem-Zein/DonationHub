using Domain.Abstractions;
using Domain.Enums;
using MediatR;

namespace Application.Features.Auth.Commands.RegisterUser;

public sealed record RegisterUserCommand(string Email, string FullName, string Password, Language PrefferLanguage) : IRequest<Result<Guid>>;
