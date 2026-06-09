using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCase_01_DTO
{
    public class DashboardBreakdownResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long RequirementsCount { get; set; }
        public long TestcasesCount { get; set; }
        public long GenerationAttempts { get; set; }
        public long SuccessfulGenerations { get; set; }
        public long ExportedTestcasesCount { get; set; }
        public DateTime? LastActivityAt { get; set; }
        public List<TestcaseTypeBreakdownResponse> TestcaseTypeBreakdown { get; set; } = new();
    }
}
