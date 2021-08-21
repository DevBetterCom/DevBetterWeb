using System;
using DevBetterWeb.Core.Interfaces;
using Markdig;

namespace DevBetterWeb.Infrastructure.Services
{
  public class MarkdigService : IMarkdown
  {
    private readonly MarkdownPipeline _pipeline;

    public MarkdigService()
    {
      _pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseBootstrap()
        .Build();
    }

    public string RenderHTMLFromMD(string mdFilepath)
    {
      string Document = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + mdFilepath);

      var result = Markdown.ToHtml(Document, _pipeline);

      return result;
    }
  }
}
