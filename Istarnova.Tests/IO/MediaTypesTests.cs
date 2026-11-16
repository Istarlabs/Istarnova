// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using Istarnova.IO.Mime;

namespace Istarnova.Tests.IO;

public class MediaTypesTests
{
    [Theory]
    [InlineData("application/pdf", ".pdf")]
    [InlineData("IMAGE/JPEG", ".jpg")]
    [InlineData("text/plain; charset=utf-8", ".txt")]
    [InlineData("application/x-unknown", null)]
    [InlineData(null, null)]
    public void GetExtension_ShouldIgnoreCaseAndParameters(string? mediaType, string? expected)
        => MediaTypes.GetExtension(mediaType).Should().Be(expected);

    [Theory]
    [InlineData(".PDF", "application/pdf")]
    [InlineData("jpeg", "image/jpeg")]
    [InlineData(".heic", "image/heic")]
    [InlineData("/library/Cover.JPG", "image/jpeg")]
    [InlineData("volume.01.cbz", "application/vnd.comicbook+zip")]
    [InlineData("archive.", null)]
    [InlineData(".xyz", null)]
    [InlineData("", null)]
    public void FromExtension_ShouldTakeAnyCaseWithOrWithoutDot(string? extension, string? expected)
        => MediaTypes.FromExtension(extension).Should().Be(expected);

    [Theory]
    [InlineData("application/epub+zip", true, false, false, false)]
    [InlineData("IMAGE/PNG; q=1", false, true, false, false)]
    [InlineData("video/mp4", false, false, true, false)]
    [InlineData("application/pdf", false, false, false, true)]
    [InlineData("image/x-unknown", false, false, false, false)]
    [InlineData(null, false, false, false, false)]
    public void IsKnown_ShouldSortMediaTypesIntoOneCategory(
        string? mediaType, bool ebook, bool image, bool video, bool document)
    {
        MediaTypes.IsKnownEbook(mediaType).Should().Be(ebook);
        MediaTypes.IsKnownImage(mediaType).Should().Be(image);
        MediaTypes.IsKnownVideo(mediaType).Should().Be(video);
        MediaTypes.IsKnownDocument(mediaType).Should().Be(document);
    }

    [Fact]
    public void IsKnownExtension_ShouldResolveThroughTheMediaType()
    {
        MediaTypes.IsKnownEbookExtension(".CBZ").Should().BeTrue();
        MediaTypes.IsKnownImageExtension("jpeg").Should().BeTrue();
        MediaTypes.IsKnownVideoExtension(".mp4").Should().BeTrue();
        MediaTypes.IsKnownDocumentExtension(".docx").Should().BeTrue();
        MediaTypes.IsKnownDocumentExtension(".xyz").Should().BeFalse();
    }

    [Theory]
    [InlineData(MimeFileType.None)]
    [InlineData(MimeFileType.Application)]
    [InlineData(MimeFileType.Stream)]
    public void GetMimeTypeFromFileType_ShouldFallBackToOctetStream(MimeFileType fileType)
        => fileType.GetMimeTypeFromFileType().Should().Be(MediaTypes.OctetStream);
}
