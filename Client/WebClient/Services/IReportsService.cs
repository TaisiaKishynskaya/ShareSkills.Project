namespace WebClient.Services
{
    public interface IReportsService
    {
        public Task<List<Report>> getReports();
    }
}
