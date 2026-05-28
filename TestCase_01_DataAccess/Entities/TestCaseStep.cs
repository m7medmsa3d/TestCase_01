using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestCase_01_DataAccess.Entities
{
    [Table("test_case_steps")]
    public class TestCaseStep
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage = "Step description is required.")]
        [StringLength(1000, ErrorMessage = "Step description cannot exceed 1000 characters.")]
        
        public string Step { get; set; }

        [Required(ErrorMessage = "TestCase ID is required.")]
        [Range(1, long.MaxValue, ErrorMessage = "TestCase ID must be a positive number greater than 0.")]
        
        public long TestCaseId { get; set; }

        [ForeignKey(nameof(TestCaseId))]
        public TestCase TestCase { get; set; }
    }
}