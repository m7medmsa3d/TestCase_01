using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TestCase_01_DTO
{
    public class TestCaseResponseDTO
    {
        public long Id { get; set; }

      
        public string Title { get; set; }

       
        public string Type { get; set; }

  
        public List<string> Steps { get; set; } = new List<string>();

        
     
        public string ExpectedResult { get; set; }
    }
}
