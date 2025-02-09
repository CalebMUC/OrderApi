using System.Data;
using Minimart_Api.Repositories;

namespace Minimart_Api.Services
{
    public class ReportService:IReportService
    {
        private readonly IReportRepo _reportRepo;
        public ReportService(IReportRepo reportRepo) { 
            _reportRepo = reportRepo;
        }
        public async Task<DataSet> GetReportData(string reportType, Dictionary<string, string> parameters) {

            var ReportData = await _reportRepo.GetReportData(reportType, parameters);

            return ReportData;
        }
    }
}
