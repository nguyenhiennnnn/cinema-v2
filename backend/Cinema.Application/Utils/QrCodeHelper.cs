using QRCoder;

namespace Cinema.Application.Utils;

public static class QrCodeHelper
{
    public static string GenerateDataUri(string content)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        using var pngQrCode = new PngByteQRCode(qrData);
        var bytes = pngQrCode.GetGraphic(10);
        return $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
    }
}
