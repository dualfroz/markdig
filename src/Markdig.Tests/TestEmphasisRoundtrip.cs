// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers.Roundtrip;
using Markdig.Syntax;

namespace Markdig.Tests;

[TestFixture]
public class TestEmphasisRoundtrip
{
    private static string RoundTrip(string markdown, MarkdownPipeline pipeline)
    {
        MarkdownDocument document = Markdown.Parse(markdown, pipeline);
        var writer = new StringWriter();
        var renderer = new RoundtripRenderer(writer);
        pipeline.Setup(renderer);
        renderer.Write(document);
        return writer.ToString();
    }

    // Leftover delimiter characters below the minimum count used to stay nested
    // inside the emphasis they closed, moving the following text to the end of
    // the block. https://github.com/xoofx/markdig/issues/743
    [Test]
    public void GridTableSeparatorRoundtripsWithTrackTrivia()
    {
        string markdown =
            "+---------------+---------------+--------------------+\n" +
            "| Fruit         | Price         | Advantages         |\n" +
            "+===============+===============+====================+\n" +
            "| Bananas       | first line    | first line         |\n" +
            "|               | next line     | next line          |\n" +
            "+---------------+---------------+--------------------+\n";

        var pipeline = new MarkdownPipelineBuilder()
            .UseAutoLinks()
            .UseEmphasisExtras()
            .UseListExtras()
            .EnableTrackTrivia()
            .Build();

        Assert.That(RoundTrip(markdown, pipeline), Is.EqualTo(markdown));
    }

    // Same defect, minimal: leftovers (15 = 7 * 2 + 1) below the "==" minimum.
    [Test]
    public void UnbalancedMarkedRunsRoundtrip()
    {
        string markdown = "+===============+===============+\n";

        var pipeline = new MarkdownPipelineBuilder()
            .UseEmphasisExtras()
            .EnableTrackTrivia()
            .Build();

        Assert.That(RoundTrip(markdown, pipeline), Is.EqualTo(markdown));
    }

    // The fix must not prevent balanced "marked" emphasis from being detected.
    [Test]
    public void BalancedMarkedEmphasisStillParses()
    {
        var pipeline = new MarkdownPipelineBuilder().UseEmphasisExtras().Build();
        Assert.That(Markdown.ToHtml("==bold==", pipeline).Trim(), Is.EqualTo("<p><mark>bold</mark></p>"));
    }
}
