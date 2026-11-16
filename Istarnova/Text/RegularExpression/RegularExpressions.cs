#region "Header"

// File: RegularExpressions.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Text.RegularExpressions;

#endregion

namespace Istarnova.Text.RegularExpression;

public static partial class RegularExpressions
{
    #region "Methods"

    [GeneratedRegex("^[A-Za-z]+$")]
    public static partial Regex CharacterRegEx();

    [GeneratedRegex(
        "(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])")]
    public static partial Regex EmailRegEx();

    [GeneratedRegex("\\W|_")]
    public static partial Regex NonAlphanumericRegex();

    [GeneratedRegex("\\d+")]
    public static partial Regex NumberRegex();

    [GeneratedRegex(@"^[+]?(\d{1,3})?[\s.-]?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$")]
    public static partial Regex PhoneNumberRegEx();

    [GeneratedRegex("<!--.*?-->")]
    public static partial Regex RemoveHtmlCommentsRegex();

    [GeneratedRegex(@"(?s)<script.*?(/>|</script>)",
        RegexOptions.Singleline | RegexOptions.IgnoreCase)]
    public static partial Regex RemoveHtmlScriptsRegex();

    [GeneratedRegex(@"(?s)<style.*?(/>|</style>)",
        RegexOptions.Singleline | RegexOptions.IgnoreCase)]
    public static partial Regex RemoveHtmlStylesRegex();

    [GeneratedRegex("""(?></?\w+)(?>(?:[^>'"]+|'[^']*'|"[^"]*")*)>""")]
    public static partial Regex RemoveHtmlTagsRegex();

    #endregion
}
