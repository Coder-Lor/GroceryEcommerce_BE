using GroceryEcommerce.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryEcommerce.Application.Features.Authentication.Commands
{
    public sealed record RefreshTokenCommand(string RefreshToken)
        :IRequest<Result<RefreshTokenResponse>>;

    public sealed record RefreshTokenResponse
    {
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
        public DateTime ExpiresAt { get; init; }
    }
}
