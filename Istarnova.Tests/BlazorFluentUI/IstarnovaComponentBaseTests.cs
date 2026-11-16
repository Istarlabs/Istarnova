// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text;
using Bunit;
using Istarnova.Blazor.FluentUI.Components;
using Istarnova.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Istarnova.Tests.BlazorFluentUI;

public class IstarnovaComponentBaseTests : BunitContext
{
    private sealed class Probe : IstarnovaComponentBase<Probe>
    {
        public Probe(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
        }

        public Task DownloadBytesAsync(string fileName, byte[] data, string mediaType)
            => DownloadFileFromUrlAsync(fileName, data, mediaType);

        public Task DownloadTextAsync(string fileName, string data, string mediaType)
            => DownloadFileFromUrlAsync(fileName, data, mediaType);
    }

    private Probe CreateProbe()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;

        return new Probe(Services.GetRequiredService<IJSRuntime>());
    }

    [Fact]
    public async Task DownloadFileFromUrlAsync_ShouldPassTheBytesAsDataUrl_WhenGivenBytes()
    {
        var probe = CreateProbe();
        byte[] data = [1, 2, 3];

        await probe.DownloadBytesAsync("file.bin", data, "application/octet-stream");

        var call = JSInterop.VerifyInvoke("downloadFileFromUrl");
        call.Arguments[0].Should().Be("file.bin");
        ((string)call.Arguments[1]!).Should().Be(data.ToDataUrl("application/octet-stream"));
    }

    [Fact]
    public async Task DownloadFileFromUrlAsync_ShouldEncodeTheTextAsUtf8_WhenGivenText()
    {
        var probe = CreateProbe();

        await probe.DownloadTextAsync("file.csv", "ä;b", "text/csv");

        var dataUrl = (string)JSInterop.VerifyInvoke("downloadFileFromUrl").Arguments[1]!;
        dataUrl.Should().StartWith("data:text/csv");
        Encoding.UTF8.GetString(dataUrl.ToBytes()).Should().Be("ä;b");
    }
}
