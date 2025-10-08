using GroceryEcommerce.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryEcommerce.Application.Features.Authentication.Commands
{
    public sealed record RevokeTokenCommand(Guid UserId, string? RefreshToken = null) :IRequest<Result>;
}
