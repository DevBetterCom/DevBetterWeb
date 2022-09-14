namespace DevBetterWeb.Web.Interfaces;

public interface IWebVTTParsingService
{
	string Parse(string vtt, string linkToVideo, int paragraphSize);
}