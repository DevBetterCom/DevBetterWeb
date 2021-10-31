namespace DevBetterWeb.Vimeo.Models
{
  public class TextTrackType
  {
    public enum TextTrackEnum
    {
      Captions, //  - The text track is the captions type.
      Chapters, // - The text track is the chapters type.
      Descriptions, // - The text track is the descriptions type.
      Metadata, // - The text track is the metadata type.
      Subtitles //- The text track is the subtitles type.
    }

    public TextTrackType(TextTrackEnum type)
    {
      Type = type;
    }

    public TextTrackEnum Type {  get; private set; }

    public override string ToString()
    {
      switch (Type)
      {
        case TextTrackEnum.Captions:
          return "captions";
        case TextTrackEnum.Chapters:
          return "chapters";
        case TextTrackEnum.Descriptions:
          return "descriptions";
        case TextTrackEnum.Metadata:
          return "metadata";
        case TextTrackEnum.Subtitles:
          return "subtitles";
        default:
          return string.Empty;
      }
    }
  }
}
