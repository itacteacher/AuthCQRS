using AuthCQRS.Application.Common.Attributes;
using AuthCQRS.Application.Common.Interfaces;
using MediatR;
using System.Reflection;

namespace AuthCQRS.Application.Common.Behaviours;
public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public AuthorizationBehaviour (IUser user, IIdentityService identityService)
    {
        _user = user;
        _identityService = identityService;
    }

    public async Task<TResponse> Handle (TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

        if (authorizeAttributes.Any())
        {
            if (_user.Id == null)
            {
                throw new Exception("User If was not found");
            }
        }

        //roles
        var attributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

        if (attributesWithRoles.Any())
        {
            var authorized = false;

            foreach (var roles in attributesWithRoles.Select(a => a.Roles.Split(',')))
            {
                foreach (var role in roles)
                {
                    var isInRole = await _identityService.IsInRoleAsync(_user.Id, role);

                    if (isInRole)
                    {
                        authorized = true;
                        break;
                    }
                }
            }

            if (!authorized)
            {
                throw new Exception();
            }
        }

        //policies
        var attributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));

        if (attributesWithPolicies.Any())
        {
            foreach (var policy in attributesWithPolicies.Select(a => a.Policy))
            {
                var authorized = await _identityService.AuthorizeAsync(_user.Id, policy);

                if (!authorized)
                {
                    throw new Exception();
                }
            }
        }

        return await next();
    }
}
