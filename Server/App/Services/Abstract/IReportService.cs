using Libraries.Contracts.Report;

namespace App.Services.Abstract;

public interface IReportService
{
    Task<IEnumerable<ReportDto>> GetReportForUserAsync(Guid userId);
}