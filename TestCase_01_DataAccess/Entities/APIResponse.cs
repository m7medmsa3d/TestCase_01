using System.Net;

namespace TestCase_01_DataAccess
{
    public class APIResponse
    {
        public APIResponse()
        {
            ErrorMessages = new List<string>();
        }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string DescriptiveMessage { get; set; }

        public List<string> ErrorMessages { get; set; } 
        public object Result { get; set; }
    }
}


