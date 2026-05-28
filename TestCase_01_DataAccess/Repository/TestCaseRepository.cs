using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCase_01_DataAccess.Data;
using TestCase_01_DataAccess.Entities;
using TestCase_01_DataAccess.Repository.IRepository;

namespace TestCase_01_DataAccess.Repository
{
   public class TestCaseRepository : Repository<TestCase>, ITestCaseRepository
   {
        private readonly ApplicationDbContext _db;
         public TestCaseRepository(ApplicationDbContext db) : base(db)
         {
             _db = db;
         }
    
   }
}
