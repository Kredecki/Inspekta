using Inspekta.Shared.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Inspekta.Client;

public static class AuthPolicyNames
{
    public const string InspectorPolicy = "InspectorPolicy";
    public const string AdminPolicy = "AdminPolicy";
    public const string SuperAdminPolicy = "SuperAdminPolicy";
}

public static class AppPolicies
{
    public static AuthorizationPolicy InspectorPolicy
        => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole(EUserRole.SuperAdministrator.ToString(),
            EUserRole.Administrator.ToString(),
            EUserRole.Inspector.ToString())
        .Build();

    public static AuthorizationPolicy AdminAuthPolicy
        => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole(EUserRole.SuperAdministrator.ToString(),
            EUserRole.Administrator.ToString())
        .Build();

    public static AuthorizationPolicy SuperAdminAuthPolicy
        => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole(EUserRole.SuperAdministrator.ToString())
        .Build();

    public static Dictionary<string, AuthorizationPolicy> Policies = new()
    {
        {AuthPolicyNames.InspectorPolicy, InspectorPolicy},
        {AuthPolicyNames.AdminPolicy, AdminAuthPolicy},
        {AuthPolicyNames.SuperAdminPolicy, SuperAdminAuthPolicy}
    };
}
