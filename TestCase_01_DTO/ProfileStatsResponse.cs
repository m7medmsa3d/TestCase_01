using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCase_01_DTO
{
   public class ProfileStatsResponse
    {
        public long ProjectsCount { get; set; }
        public long RequirementsCount { get; set; }
        public long TestcasesCount { get; set; }
        public long GenerationAttempts { get; set; }
        public long SuccessfulGenerations { get; set; }
        public long ExportedTestcasesCount { get; set; }

        public List<TestcaseTypeBreakdownResponse> TestcaseTypeBreakdown { get; set; } = new();
        public List<ProfileActivityResponse> RecentActivity { get; set; } = new();
        public List<DashboardBreakdownResponse> ProjectBreakdown { get; set; } = new();
        public List<DashboardBreakdownResponse> RequirementBreakdown { get; set; } = new();
        public List<DashboardTrendPointResponse> GenerationTrend { get; set; } = new();
    }
}
