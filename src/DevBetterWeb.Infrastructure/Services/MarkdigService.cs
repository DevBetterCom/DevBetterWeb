using System;
using DevBetterWeb.Core.Interfaces;
using Markdig;

namespace DevBetterWeb.Infrastructure.Services
{
  public class MarkdigService : IMarkdownService
  {
    private readonly MarkdownPipeline _pipeline;

    public MarkdigService()
    {
      _pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseBootstrap()
        .Build();
    }

    public string RenderHTMLFromMD(string mdContent)
    {
      var result = Markdown.ToHtml(mdContent, _pipeline);

      return result;
    }
  }
}
