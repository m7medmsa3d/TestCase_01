using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestCase_01_DTO
{
    public class TestCaseRequestDTO
    {
        [Required(ErrorMessage = "Requirement ID can not be empty")]
        public long RequirementId { get; set; }

        [Required(ErrorMessage = "Project ID can not be empty")]
        public long ProjectId { get; set; }

       
        public int? UserId { get; set; }

        [Required(ErrorMessage = "Testcase list can not be empty")]
        public List<TestCaseDTO> Testcases { get; set; } = new List<TestCaseDTO>();
    }
}