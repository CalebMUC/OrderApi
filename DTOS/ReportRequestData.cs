namespace Minimart_Api.DTOS
{
    public class ReportRequestData
    {
        public string ReportType { get; set; }
        public string Format { get; set; }

        public Dictionary<string, string> parameters { get; set; }
          
    }
}
