using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Pdf;
using Cinema.Application.DTOs.Reports;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Utils;
using Cinema.Shared.Resources;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Services.Reports;

public class ReportExportService(
    IReportService reportService,
    IExportWriter exportWriter,
    IPdfGenerator pdfGenerator,
    IAppDbContext db,
    IOptions<BrandOptions> brandOptions
) : IReportExportService
{
    private const string RevenueModule = "revenue";
    private const string TopMoviesModule = "top-movies";
    private const string EmployeePerformanceModule = "employee-performance";

    public async Task<ExportFileDto> ExportRevenueAsync(
        DateOnly fromDate,
        DateOnly toDate,
        string groupBy,
        ExportFormat format,
        CancellationToken cancellationToken
    )
    {
        var report = await reportService.GetRevenueReportAsync(
            fromDate,
            toDate,
            groupBy,
            cancellationToken
        );

        IReadOnlyList<object?> totalRow =
        [
            Messages.Export_Total,
            report.Items.Sum(i => i.TicketRevenue),
            report.Items.Sum(i => i.ComboRevenue),
            report.Summary.TotalRevenue,
            report.Summary.TotalBookings,
        ];

        return await WriteAsync(
            RevenueModule,
            Messages.Export_SheetRevenue,
            Messages.Export_PdfTitleRevenue,
            fromDate,
            toDate,
            format,
            RevenueColumns(),
            report.Items,
            totalRow,
            cancellationToken
        );
    }

    public async Task<ExportFileDto> ExportTopMoviesAsync(
        DateOnly fromDate,
        DateOnly toDate,
        int limit,
        ExportFormat format,
        CancellationToken cancellationToken
    )
    {
        var movies = await reportService.GetTopMoviesReportAsync(
            fromDate,
            toDate,
            limit,
            cancellationToken
        );

        return await WriteAsync(
            TopMoviesModule,
            Messages.Export_SheetTopMovies,
            Messages.Export_PdfTitleTopMovies,
            fromDate,
            toDate,
            format,
            TopMovieColumns(),
            movies,
            null,
            cancellationToken
        );
    }

    public async Task<ExportFileDto> ExportEmployeePerformanceAsync(
        DateOnly fromDate,
        DateOnly toDate,
        ExportFormat format,
        CancellationToken cancellationToken
    )
    {
        var employees = await reportService.GetEmployeePerformanceReportAsync(
            fromDate,
            toDate,
            cancellationToken
        );

        return await WriteAsync(
            EmployeePerformanceModule,
            Messages.Export_SheetEmployeePerformance,
            Messages.Export_PdfTitleEmployeePerformance,
            fromDate,
            toDate,
            format,
            EmployeePerformanceColumns(),
            employees,
            null,
            cancellationToken
        );
    }

    private async Task<ExportFileDto> WriteAsync<T>(
        string module,
        string sheetName,
        string pdfTitle,
        DateOnly fromDate,
        DateOnly toDate,
        ExportFormat format,
        IReadOnlyList<ExportColumn<T>> columns,
        IReadOnlyList<T> rows,
        IReadOnlyList<object?>? totalRow,
        CancellationToken cancellationToken
    )
    {
        var content = format switch
        {
            ExportFormat.Pdf => await WritePdfAsync(
                pdfTitle,
                fromDate,
                toDate,
                columns,
                rows,
                totalRow,
                cancellationToken
            ),
            ExportFormat.Csv => await exportWriter.WriteCsvAsync(columns, rows, cancellationToken),
            _ => await exportWriter.WriteXlsxAsync(
                sheetName,
                columns,
                rows,
                totalRow,
                cancellationToken
            ),
        };

        return new ExportFileDto(
            content,
            ExportFormats.ContentType(format),
            ExportFileName.Create(module, ExportFormats.Extension(format))
        );
    }

    private async Task<Stream> WritePdfAsync<T>(
        string title,
        DateOnly fromDate,
        DateOnly toDate,
        IReadOnlyList<ExportColumn<T>> columns,
        IReadOnlyList<T> rows,
        IReadOnlyList<object?>? totalRow,
        CancellationToken cancellationToken
    )
    {
        var settings = await db.LoadCinemaSettingsOrDefaultAsync(cancellationToken);
        var cinemaName = string.IsNullOrWhiteSpace(settings.Name)
            ? brandOptions.Value.Name
            : settings.Name;

        var pdfColumns = columns
            .Select(c =>
            {
                var isNumeric = c.Type is ExportColumnType.Number or ExportColumnType.Money;
                return new PdfTableColumn(c.Header, isNumeric, isNumeric ? 1 : 2);
            })
            .ToList();

        var pdfRows = rows
            .Select(row =>
                (IReadOnlyList<string>)
                    columns.Select(c => PdfCellFormatter.Format(c.Selector(row), c.Type)).ToList()
            )
            .ToList();

        IReadOnlyList<string>? pdfTotalRow = totalRow is null
            ? null
            : columns
                .Select(
                    (c, i) =>
                        PdfCellFormatter.Format(i < totalRow.Count ? totalRow[i] : null, c.Type)
                )
                .ToList();

        var document = new PdfReportDocument(
            title,
            cinemaName,
            string.Format(
                Messages.Export_PdfPeriod,
                PdfCellFormatter.FormatDate(fromDate),
                PdfCellFormatter.FormatDate(toDate)
            ),
            string.Format(
                Messages.Export_PdfExportedAt,
                PdfCellFormatter.FormatDateTime(DateTime.UtcNow)
            ),
            Messages.Export_PdfPage,
            Messages.Export_PdfNoData,
            pdfColumns,
            pdfRows,
            pdfTotalRow
        );

        return pdfGenerator.GenerateReport(document);
    }

    private static IReadOnlyList<ExportColumn<RevenueReportItemDto>> RevenueColumns() =>
        [
            new(Messages.Export_ColPeriod, x => x.Period),
            new(Messages.Export_ColTicketRevenue, x => x.TicketRevenue, ExportColumnType.Money),
            new(Messages.Export_ColComboRevenue, x => x.ComboRevenue, ExportColumnType.Money),
            new(Messages.Export_ColTotalRevenue, x => x.TotalRevenue, ExportColumnType.Money),
            new(Messages.Export_ColBookingCount, x => x.BookingCount, ExportColumnType.Number),
        ];

    private static IReadOnlyList<ExportColumn<TopMovieDto>> TopMovieColumns() =>
        [
            new(Messages.Export_ColMovieId, x => x.MovieId, ExportColumnType.Number),
            new(Messages.Export_ColMovieTitle, x => x.Title),
            new(Messages.Export_ColTicketsSold, x => x.TicketsSold, ExportColumnType.Number),
            new(Messages.Export_ColRevenue, x => x.Revenue, ExportColumnType.Money),
        ];

    private static IReadOnlyList<ExportColumn<EmployeePerformanceDto>> EmployeePerformanceColumns() =>
        [
            new(Messages.Export_ColEmployeeId, x => x.EmployeeId, ExportColumnType.Number),
            new(Messages.Export_ColEmployeeName, x => x.FullName),
            new(Messages.Export_ColTicketsSold, x => x.TicketsSold, ExportColumnType.Number),
            new(Messages.Export_ColRevenue, x => x.Revenue, ExportColumnType.Money),
            new(Messages.Export_ColBookingCount, x => x.BookingCount, ExportColumnType.Number),
        ];
}
