namespace Cinema.Shared.Constants;

public class SignalREvents
{
    public const string HoldExpired = "hold.expired";
    public const string SeatBooked = "seat.booked";
    public const string SeatHeld = "seat.held";
    public const string SeatReleased = "seat.released";
    public const string ImportExportJobProgress = "import-export-job.progress";
    public const string ImportExportJobCompleted = "import-export-job.completed";
    public const string ImportExportJobFailed = "import-export-job.failed";
}
