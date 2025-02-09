using System.Data;

namespace Minimart_Api.Services
{
    public interface IReportService
    {
        public Task<DataSet> GetReportData(string reportType, Dictionary<string, string> parameters);
            
        
    }
}
