using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCase_01_DTO
{
   public class DashboardTrendPointResponse
    {
        public DateTime date { get; set; }
        public long GeneratedTestCases { get; set; }
        public long GenerationAttempts { get; set; }
        public long successfulGenerations { get; set; }
        public long exports { get; set; }
    }
}
