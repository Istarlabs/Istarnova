#region "Header"

// File: TrustedProxyNetworks.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using System.Net;

namespace Istarnova.AspNetCore.Security;

/// <summary>
///     The networks a reverse proxy may sit in, whose <c>X-Forwarded-For</c> and <c>X-Forwarded-Proto</c> the application
///     believes; anyone else sending them could write any address into the audit trail.
/// </summary>
public static class TrustedProxyNetworks
{
    #region "Variables"

    /// <summary>
    ///     The name of the configuration list: <c>ForwardedHeaders:TrustedNetworks</c>, each entry a network like
    ///     <c>192.168.1.20/32</c> or <c>10.89.0.0/24</c>.
    /// </summary>
    public const string SectionName = "ForwardedHeaders:TrustedNetworks";

    /// <summary>
    ///     What is trusted while nothing is configured: this machine and the private ranges a proxy at home is in. Not the
    ///     internet - an instance that is reached directly from outside no longer believes what the caller writes.
    /// </summary>
    private static readonly string[] DefaultNetworks =
    [
        "127.0.0.0/8", "::1/128", "10.0.0.0/8", "172.16.0.0/12", "192.168.0.0/16", "fc00::/7",
    ];

    #endregion

    #region "Methods"

    /// <summary>
    ///     The networks to trust.
    /// </summary>
    /// <param name="configured">The configured list; empty means the default.</param>
    /// <exception cref="FormatException">An entry is not a network.</exception>
    public static IReadOnlyList<IPNetwork> Resolve(IEnumerable<string>? configured)
    {
        var entries = configured?.Where(entry => !string.IsNullOrWhiteSpace(entry)).ToList() ?? [];

        return
        [
            .. (entries.Count == 0 ? DefaultNetworks.AsEnumerable() : entries)
            .Select(entry => IPNetwork.TryParse(entry.Trim(), out var network)
                ? network
                : throw new FormatException(
                    $"{SectionName} holds '{entry}', which is not a network like 192.168.1.20/32.")),
        ];
    }

    #endregion
}
