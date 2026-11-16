#region "Header"

// File: ClaimsPrincipalExtensions.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Security.Claims;
using Istarnova.Resources;

#endregion

namespace Istarnova.Security.Claims;

public static partial class ClaimsPrincipalExtensions
{
    #region "Methods"

    extension(ClaimsPrincipal principal)
    {
        #region "Methods"

        public string? GetAzureObjectId() =>
            principal.FindFirstValue(ClaimTypesConstants.AzureObjectId);

        public Guid? GetAzureObjectIdAsGuid()
        {
            var objectId = principal.GetAzureObjectId();
            if (Guid.TryParse(objectId, out var guid))
            {
                return guid;
            }

            return null;
        }

        public string? GetPreferredUsername() =>
            principal.FindFirstValue(ClaimTypesConstants.PreferredUserName);

        public string? GetTenantId() =>
            principal.FindFirstValue(ClaimTypesConstants.TenantId);

        public Guid? GetTenantIdAsGuid()
        {
            var tenantId = principal.GetTenantId();
            if (Guid.TryParse(tenantId, out var guid))
            {
                return guid;
            }

            return null;
        }

        #endregion
    }

    #endregion
}

public static partial class ClaimsPrincipalExtensions
{
    #region "Methods"

    extension(ClaimsPrincipal principal)
    {
        #region "Methods"

        public string? FindFirstValue(string claimType)
        {
            ArgumentNullException.ThrowIfNull(principal);
            ArgumentNullException.ThrowIfNull(claimType);

            return principal.FindFirst(claimType)?.Value;
        }

        public string? FindFirstValue(string claimType, string defaultValue)
        {
            ArgumentNullException.ThrowIfNull(principal);
            ArgumentNullException.ThrowIfNull(claimType);

            return principal.FindFirst(claimType)?.Value ?? defaultValue;
        }

        public DateTimeOffset? GetExpireDate()
        {
            if (long.TryParse(principal.FindFirstValue("exp"), out var seconds))
            {
                return DateTimeOffset.FromUnixTimeSeconds(seconds);
            }

            return null;
        }

        public DateTimeOffset? GetExpiresAt()
        {
            ArgumentNullException.ThrowIfNull(principal);

            var expiresAtClaim = principal.FindFirst(ClaimTypes.Expiration);
            if (expiresAtClaim != null && DateTimeOffset.TryParse(expiresAtClaim.Value, out var expiresAt))
            {
                return expiresAt;
            }

            return null;
        }

        public string GetFullName()
        {
            ArgumentNullException.ThrowIfNull(principal);
            var givenName = principal.FindFirstValue(ClaimTypes.GivenName);
            var familyName = principal.FindFirstValue(ClaimTypes.Surname);
            return $"{givenName} {familyName}";
        }

        public string? GetInitials()
        {
            var initialGivenName = principal.FindFirstValue(ClaimTypes.GivenName);
            var initialFamilyName = principal.FindFirstValue(ClaimTypes.Surname);
            return $"{initialGivenName?[0]}{initialFamilyName?[0]}";
        }

        public string? GetNameIdentifier()
        {
            ArgumentNullException.ThrowIfNull(principal);
            return principal.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public string GetRolesAsString(string separator = ", ")
        {
            ArgumentNullException.ThrowIfNull(principal);
            ArgumentNullException.ThrowIfNull(separator);

            var roles = principal.FindAll(ClaimTypes.Role);
            return string.Join(separator, roles.Select(r => r.Value));
        }

        public string? GetSureName() => principal.FindFirstValue(ClaimTypes.Surname);

        #endregion
    }

    /// <summary>
    ///     The text to show for a claim type, in the current UI culture, or the claim type itself when it has none.
    /// </summary>
    /// <param name="claimType">The claim type, for example <see cref="ClaimTypes.Email" />.</param>
    public static string GetClaimTypeDisplayName(string claimType) =>
        claimType switch
        {
            ClaimTypes.Role => ClaimsResources.ClamRole,
            ClaimTypes.NameIdentifier => ClaimsResources.ClaimUserId,
            ClaimTypes.Email => ClaimsResources.ClaimEmail,
            ClaimTypesConstants.AzureObjectId => ClaimsResources.ClaimAzureObjectId,
            ClaimTypesConstants.PreferredUserName => ClaimsResources.ClaimPreferredUsername,
            ClaimTypesConstants.TenantId => ClaimsResources.ClaimTenant,
            _ => claimType,
        };

    #endregion
}
