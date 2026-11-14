namespace Cinema.Application.Options;

public class PdfOptions
{
    public const string SectionName = "Pdf";

    public string FontDirectory { get; set; } = "Fonts";
    public string FontFamily { get; set; } = "Be Vietnam Pro";
    public string RegularFontFile { get; set; } = "BeVietnamPro-Regular.ttf";
    public string BoldFontFile { get; set; } = "BeVietnamPro-Bold.ttf";
}
