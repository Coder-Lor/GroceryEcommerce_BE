using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Auth.Users.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Auth.Users.Handlers;

public sealed class GetUsersPagingHandler(
    IUserRepository repository,
    IMapper mapper,
    ILogger<GetUsersPagingHandler> logger
) : IRequestHandler<GetUsersPagingQuery, Result<PagedResult<UserDto>>>
{
    public async Task<Result<PagedResult<UserDto>>> Handle(GetUsersPagingQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting users paging: Page {Page}, PageSize {PageSize}", 
            request.Request.Page, request.Request.PageSize);

    var result = await repository.GetPagedAsync(request.Request, cancellationToken);
        if (!result.IsSuccess || result.Data == null)
     {
   logger.LogWarning("Failed to get users");
            return Result<PagedResult<UserDto>>.Failure(result.ErrorMessage);
        }

     var dtoResult = mapper.Map<PagedResult<UserDto>>(result.Data);
        return Result<PagedResult<UserDto>>.Success(dtoResult);
    }
}
