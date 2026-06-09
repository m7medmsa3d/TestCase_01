using System.Collections.Generic;
using System.Threading.Tasks;
using TestCase_01_DTO;

namespace TestCase_01_DataAccess.Service.IService
{
    public interface ITestCaseService
    {
        Task CreateTestCaseAsync(TestCaseRequestDTO testCaseDto);
        Task<TestCaseResponseDTO> GetTestCaseByIdAsync(long testcaseid, int userId);
        Task<IEnumerable<TestCaseResponseDTO>> GetAllByProjectIdAsync(long projectId, int userId);
        Task<IEnumerable<TestCaseResponseDTO>> GetAllByRequirementIdAsync(long requirementId, int userId);
        Task DeleteByRequirementAsync(long requirementId, int userId);
        Task DeleteByTestCaseAsync(long testcaseid, int userId);



        Task<ProfileStatsResponse> GetUserSummaryAsync(int userId);
        Task<byte[]> ExportTestCaseAsync(long testcaseId, int userId, string format);
        Task<byte[]> ExportByRequirementAsync(long requirementId, int userId, string format);
        Task<byte[]> ExportByProjectAsync(long projectId, int userId, string format);

    }
}