using System.Data;

namespace Minimart_Api.Repositories
{
    public interface IReportRepo
    {
        public Task<DataSet> GetReportData(string reportType, Dictionary<string, string> parameters);
    }
}
