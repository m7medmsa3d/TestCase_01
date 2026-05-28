using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCase_01_DataAccess.Entities;

namespace TestCase_01_DataAccess.Data
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<TestCase> TestCases { get; set; }
        public DbSet<TestCaseStep> TestCaseSteps { get; set; }

    }
}
