#region "Header"

// File: ClaimTypesConstants.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.Security.Claims;

public sealed class ClaimTypesConstants
{
    #region "Constants"

    public const string AzureObjectId = "http://schemas.microsoft.com/identity/claims/objectidentifier";
    public const string PreferredUserName = "preferred_username";
    public const string Roles = "roles";
    public const string TenantId = "http://schemas.microsoft.com/identity/claims/tenantid";

    #endregion
}
