namespace DevBetterWeb.Vimeo.Models
{
  public class TextTrack
  {
    public enum TextTrackType
    {
      Captions, //  - The text track is the captions type.
      Chapters, // - The text track is the chapters type.
      Descriptions, // - The text track is the descriptions type.
      Metadata, // - The text track is the metadata type.
      Subtitles //- The text track is the subtitles type.
    }

    public TextTrack(TextTrackType type)
    {
      Type = type;
    }

    public TextTrackType Type {  get; private set; }

    public override string ToString()
    {
      switch (Type)
      {
        case TextTrackType.Captions:
          return "captions";
        case TextTrackType.Chapters:
          return "chapters";
        case TextTrackType.Descriptions:
          return "descriptions";
        case TextTrackType.Metadata:
          return "metadata";
        case TextTrackType.Subtitles:
          return "subtitles";
        default:
          return string.Empty;
      }
    }
  }
}
