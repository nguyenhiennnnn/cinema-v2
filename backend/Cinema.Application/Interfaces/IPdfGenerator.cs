using Cinema.Application.DTOs.Pdf;

namespace Cinema.Application.Interfaces;

public interface IPdfGenerator
{
    Stream GenerateReport(PdfReportDocument document);

    Stream GenerateTickets(PdfTicketDocument document);
}
