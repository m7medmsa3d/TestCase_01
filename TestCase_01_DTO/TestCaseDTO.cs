using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TestCase_01_DTO
{
   
        public class TestCaseDTO
        {
            [Required(ErrorMessage = "Type of testcase can not be empty")]

            public string Type { get; set; }

            [Required(ErrorMessage = "title of user story can not be empty")]
         
            public string Title { get; set; }

            [Required(ErrorMessage = "Steps of testcase can not be empty")]
            [MinLength(1, ErrorMessage = "Steps of testcase can not be empty")]
          
            public List<string> Steps { get; set; } = new List<string>();

            [Required(ErrorMessage = "ExpectedResult can not be empty")]
            [JsonPropertyName("expected_result")] 
           
            public string ExpectedResult { get; set; }
        }
}

