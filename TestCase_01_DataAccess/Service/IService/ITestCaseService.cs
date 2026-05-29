using System.Collections.Generic;
using System.Threading.Tasks;
using TestCase_01_DTO;

namespace TestCase_01_DataAccess.Service.IService
{
    public interface ITestCaseService
    {
        Task CreateTestCaseAsync(TestCaseRequestDTO testCaseDto);
        Task<TestCaseResponseDTO> GetTestCaseByIdAsync(long testcaseid);
        Task<IEnumerable<TestCaseResponseDTO>> GetAllByProjectIdAsync(long projectId);
        Task<IEnumerable<TestCaseResponseDTO>> GetAllByRequirementIdAsync(long requirementId);
        Task DeleteByRequirementAsync(long requirementId);
        Task DeleteByTestCaseAsync(long testcaseid);
    }
}