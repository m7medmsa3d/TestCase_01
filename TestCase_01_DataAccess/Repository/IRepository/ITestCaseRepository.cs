using TestCase_01_DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCase_01_DataAccess.Entities;

namespace TestCase_01_DataAccess.Repository.IRepository
{
   public interface ITestCaseRepository : IRepository<TestCase>
    {

    }
}
