using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCase_01_DataAccess.Repository.IRepository;

namespace TestCase_01_DataAccess.Repository.IReposaitory
{
    public interface IUnitofWork
    {
        public ITestCaseRepository testCaseRepository { get; }
        
      
    }
}
