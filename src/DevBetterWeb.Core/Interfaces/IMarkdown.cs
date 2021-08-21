namespace DevBetterWeb.Core.Interfaces
{
  public interface IMarkdown
  {
    string RenderHTMLFromMD(string mdFilepath);
  }
}
