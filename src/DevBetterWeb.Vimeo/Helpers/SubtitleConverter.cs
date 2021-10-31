using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DevBetterWeb.Vimeo.Helper
{  
  public class SubtitleConverter
  {
    private static string SIMPLE_TIME_FORMAT = "mm:ss,fff";
    private static string EXTENDED_TIME_FORMAT = "HH:mm:ss,fff";
    private static readonly Regex _rgxCueID = new Regex(@"^\d+$");
    private static readonly Regex _rgxTimeFrame = new Regex(@"(\d\d:\d\d:\d\d(?:[,.]\d\d\d)?) --> (\d\d:\d\d:\d\d(?:[,.]\d\d\d)?)");

    public static string ConvertSrtToVtt(string srtValue)
    {
      var streamOut = new MemoryStream();
      using (var srtReader = new StreamReader(GenerateStreamFromString(srtValue)))      
      using (var vttWriter = new StreamWriter(streamOut))
      {
        vttWriter.WriteLine("WEBVTT"); // Starting line for the WebVTT files
        vttWriter.WriteLine("");

        string srtLine;
        while ((srtLine = srtReader.ReadLine()) != null)
        {
          if (_rgxCueID.IsMatch(srtLine)) // Ignore cue ID number lines
          {
            continue;
          }

          Match match = _rgxTimeFrame.Match(srtLine);
          if (match.Success) // Format the time frame to VTT format (and handle offset)
          {
            var startTime = TimeSpan.Parse(match.Groups[1].Value.Replace(',', '.'));
            var endTime = TimeSpan.Parse(match.Groups[2].Value.Replace(',', '.'));

            srtLine =
                startTime.ToString(@"hh\:mm\:ss\.fff") +
                " --> " +
                endTime.ToString(@"hh\:mm\:ss\.fff");
          }

          vttWriter.WriteLine(srtLine);
        }
      }

      return Encoding.ASCII.GetString(streamOut.ToArray());
    }

    // Reference https://github.com/AhmedOS/VTT-to-SRT-Converter/blob/master/WebVTT-to-SubRip-Converter/Core/WebvttSubripConverter.cs
    public static string ConvertToSubrip(string webvttSubtitle)
    {
      var reader = new StringReader(webvttSubtitle);
      var output = new StringBuilder();
      var lineNumber = 1;
      string line;
      while ((line = reader.ReadLine()) != null)
      {
        if (IsTimecode(line))
        {

          output.AppendLine(lineNumber.ToString());
          lineNumber++;

          line = line.Replace('.', ',');

          line = DeleteCueSettings(line);

          var timeSrt1 = line.Substring(0, line.IndexOf('-'));
          var timeSrt2 = line.Substring(line.IndexOf('>') + 1);
          var divIt1 = timeSrt1.Count(x => x == ':');
          var divIt2 = timeSrt1.Count(x => x == ':');

          string timeFormat = SIMPLE_TIME_FORMAT;
          if (divIt1 != divIt2)
            throw null;

          if (divIt1 == 2 && divIt2 == 2)
            timeFormat = EXTENDED_TIME_FORMAT;

          var timeAux1 = DateTime.ParseExact(timeSrt1.Trim(), timeFormat, CultureInfo.InvariantCulture);
          var timeAux2 = DateTime.ParseExact(timeSrt2.Trim(), timeFormat, CultureInfo.InvariantCulture);
          line = timeAux1.ToString(EXTENDED_TIME_FORMAT) + " --> " + timeAux2.ToString(EXTENDED_TIME_FORMAT);

          output.AppendLine(line);

          bool foundCaption = false;
          while (true)
          {
            if ((line = reader.ReadLine()) == null)
            {
              if (foundCaption)
                break;
              else
                return null;
            }
            if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))
            {
              output.AppendLine();
              break;
            }
            foundCaption = true;
            output.AppendLine(line);
          }
        }
      }
      return output.ToString();
    }

    private static Stream GenerateStreamFromString(string value)
    {
      var stream = new MemoryStream();
      var writer = new StreamWriter(stream);
      writer.Write(value);
      writer.Flush();
      stream.Position = 0;
      return stream;
    }

    private static bool IsTimecode(string line)
    {
      return line.Contains("-->");
    }

    private static string DeleteCueSettings(string line)
    {
      var output = new StringBuilder();
      foreach (var ch in line)
      {
        var chLower = char.ToLower(ch);
        if (chLower >= 'a' && chLower <= 'z')
        {
          break;
        }
        output.Append(ch);
      }
      return output.ToString();
    }

  }
}
