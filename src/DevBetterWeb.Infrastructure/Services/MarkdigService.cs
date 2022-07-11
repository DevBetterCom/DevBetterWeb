using System;
using DevBetterWeb.Core.Interfaces;
using Markdig;

namespace DevBetterWeb.Infrastructure.Services;

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

  public string RenderHTMLFromMD(string? mdContent)
  {
    if (string.IsNullOrEmpty(mdContent))
    {
      return string.Empty;
    }
    var result = Markdown.ToHtml(mdContent, _pipeline);

    return result;
  }

  public static string RenderHtml(string mdContent)
  {
	  var pipeline = new MarkdownPipelineBuilder()
		  .UseAdvancedExtensions()
		  .UseBootstrap()
		  .Build();

	  var result = Markdown.ToHtml(mdContent, pipeline);

	  return result;
  }
}
