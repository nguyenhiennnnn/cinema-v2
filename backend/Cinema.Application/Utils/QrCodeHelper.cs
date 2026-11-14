using QRCoder;

namespace Cinema.Application.Utils;

public static class QrCodeHelper
{
    public static string GenerateDataUri(string content) =>
        $"data:image/png;base64,{Convert.ToBase64String(GeneratePngBytes(content))}";

    public static byte[] GeneratePngBytes(string content)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        using var pngQrCode = new PngByteQRCode(qrData);
        return pngQrCode.GetGraphic(10);
    }
}
