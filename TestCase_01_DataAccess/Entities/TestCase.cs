using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestCase_01_DataAccess.Entities
{
    [Table("TESTCASE")]
    public class TestCase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage = "TestCase Type is required.")]
        [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters.")]
        public string Type { get; set; }

        [Required(ErrorMessage = "TestCase Title is required.")]
        [StringLength(255, ErrorMessage = "Title cannot exceed 255 characters.")]
        public string Title { get; set; }

 
        public List<TestCaseStep> Steps { get; set; } = new List<TestCaseStep>();

        [Required(ErrorMessage = "Expected Result is required.")]
        
        public string ExpectedResult { get; set; }

        [Required(ErrorMessage = "Project ID is required.")]
        [Range(1, long.MaxValue, ErrorMessage = "Project ID must be a positive number greater than 0.")]
        public long ProjectId { get; set; }

        [Required(ErrorMessage = "Requirement ID is required.")]
        [Range(1, long.MaxValue, ErrorMessage = "Requirement ID must be a positive number greater than 0.")]
        public long RequirementId { get; set; }

        public bool Deleted { get; set; } = false;
    }
}