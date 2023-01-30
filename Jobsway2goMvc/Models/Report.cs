using Jobsway2goMvc.Enums;
using System.ComponentModel.DataAnnotations;

namespace Jobsway2goMvc.Models
{
    public class Report
    {
        public int Id { get; set; }
        public int? JobId { get; set; }
        public int? PostId { get; set; }
        public int? GroupId { get; set; }
        public ReportEnum reportType { get; set; }
    }
}
