using TestCase_01_DataAccess.Data;
using TestCase_01_DataAccess.DataAccess.Repository;
using TestCase_01_DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCase_01_DataAccess.Repository.IReposaitory;
using TestCase_01_DataAccess.Repository.IRepository;
using TestCase_01_DataAccess.Repository;

namespace TestCase_01_DataAccess.DataAccess.Repository
{
    
    public class UnitOfWork : IUnitofWork
    {
        private readonly ApplicationDbContext db;
      
        

        public ITestCaseRepository testCaseRepository { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            this.db = db;
            testCaseRepository = new TestCaseRepository(db);
         
        }
       
    }
}
