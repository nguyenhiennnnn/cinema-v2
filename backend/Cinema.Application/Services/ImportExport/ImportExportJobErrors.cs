using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;

namespace Cinema.Application.Services.ImportExport;

public static class ImportExportJobErrors
{
    public static string Describe(Exception exception) =>
        exception is AppException ? exception.Message : Messages.Job_UnknownError;
}
