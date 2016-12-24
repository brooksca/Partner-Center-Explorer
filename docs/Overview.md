# Partner Center Explorer Overview

## Authentication
Azure Active Directory (AAD) is utilized to provide authentication and authorization for this solution. This was done so that single sign-on to the Microsoft management 
portals could be provided, and so that the solution would be compliant the identity guideance from the products team. All of this is achieved by the code defined in the
_ConfigureAuth_ function found in the _Startup.Auth.cs_ code file. 

Whenever an unauthenticated user navigates to this solution they will be required to authenticate. In the event the user has already logged into another system that leverages
AAD for authentication the user will not be prompted for authentication. Once the user's identity has been provide the code in the _ConfigureAuth_ function will do the 
following 

1. Obtains the directory roles the user has been assigned to using Azure AD Graph API
2. Adds all roles assigned to the user as role claims
3. Obtains a reference to the customer who owns the authenticated user using the Partner Center API
4. If the authenticated user belongs to the partner then the code will verify the user belongs to the _Company Administrators_ directory role 

It is important to note that if the authenticated user belongs to a customer that does not have a relationship with the partner, then no customer reference can be obtained.
Which means the user will not be authorized to use this solution. Authorization is determined using the _IsAuthorized_ function found in the  _AuthorizationPolicy_ class. The 
complete function can be found below 

```csharp
public bool IsAuthorized(CustomerPrincipal principal, UserRole requiredUserRole)
{
    principal.AssertNotNull(nameof(principal));

    bool isAuthorized;

    switch (requiredUserRole)
    {
        case UserRole.Customer:
            isAuthorized = principal.IsCustomer && !principal.IsAdmin;
            break;
        case UserRole.Partner:
            isAuthorized = !principal.IsCustomer && principal.IsAdmin;
            break;
        case UserRole.Any:
            isAuthorized = principal.IsCustomer || principal.IsAdmin;
            break;
        case UserRole.None:
            isAuthorized = !principal.IsCustomer && !principal.IsAdmin;
            break;
        default:
            isAuthorized = false;
            break;
    }

    return isAuthorized;
}
```