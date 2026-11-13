namespace Cinema.Application.Utils;

public static class ExportFileName
{
    public static string Create(string module, string extension, DateTime? utcNow = null)
    {
        var local = BusinessTime.ToLocal(utcNow ?? DateTime.UtcNow);
        return $"{module}-{local:yyyyMMdd-HHmmss}.{extension}";
    }
}
