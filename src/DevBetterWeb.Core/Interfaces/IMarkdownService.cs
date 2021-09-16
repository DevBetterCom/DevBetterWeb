namespace DevBetterWeb.Core.Interfaces
{
  public interface IMarkdownService
  {
    string RenderHTMLFromMD(string? mdContent);
  }
}
