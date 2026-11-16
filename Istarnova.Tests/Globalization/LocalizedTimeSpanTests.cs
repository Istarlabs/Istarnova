// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Globalization;
using Istarnova.Globalization;

namespace Istarnova.Tests.Globalization;

public class LocalizedTimeSpanTests
{
    [Theory]
    [InlineData("en", "2 Years 2 Months 4 Days ")]
    [InlineData("de", "2 Jahre 2 Monate 4 Tage ")]
    [InlineData("ja-JP", "2 年 2 月 4 日 ")]
    public void ToString_ShouldUseTheUiCulture_WhenTranslationExists(string culture, string expected)
    {
        var previous = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(culture);

        try
        {
            var span = new LocalizedTimeSpan(new DateTime(2020, 1, 1), new DateTime(2022, 3, 5));

            span.ToString().Should().Be(expected);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }
}
