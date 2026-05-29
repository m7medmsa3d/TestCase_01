using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; 
using TestCase_01_DataAccess.Entities;
using TestCase_01_DataAccess.Repository.IReposaitory;
using TestCase_01_DataAccess.Service.IService;
using TestCase_01_DTO;

namespace TestCase_01_DataAccess.Service
{
    public class TestCaseService : ITestCaseService
    {
        private readonly IUnitofWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TestCaseService> _logger;

        public TestCaseService(IUnitofWork unitOfWork, IMapper mapper, ILogger<TestCaseService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task CreateTestCaseAsync(TestCaseRequestDTO testCaseRequestDto)
        {
           
            if (testCaseRequestDto == null)
            {
                _logger.LogWarning("CreateTestCaseAsync received a null TestCaseDTO.");
                throw new ArgumentNullException(nameof(testCaseRequestDto));
            }

            _logger.LogInformation("Mapping and creating Batch TestCases for Project: {ProjectId}, Requirement: {RequirementId}",
                testCaseRequestDto.ProjectId, testCaseRequestDto.RequirementId);

            try
            {
                var testCasesList = _mapper.Map<List<TestCase>>(testCaseRequestDto.Testcases);

               
                foreach (var testCase in testCasesList)
                {
                    testCase.ProjectId = testCaseRequestDto.ProjectId;
                    testCase.RequirementId = testCaseRequestDto.RequirementId;
                    testCase.Deleted = false; 
                }

               
                if (testCasesList.Any())
                {
                    foreach (var testCase in testCasesList)
                    {
                            await _unitOfWork.testCaseRepository.CreateAsync(testCase);
                    }

                     await _unitOfWork.testCaseRepository.SaveAsync();
                }

                _logger.LogInformation("{Count} TestCases successfully saved to DB.", testCasesList.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating TestCase.");
                throw;
            }
        }

        public async Task<TestCaseResponseDTO> GetTestCaseByIdAsync(long testcaseid)
        {
            _logger.LogInformation("Fetching TestCase ID: {TestCaseId}", testcaseid);

            var testCase = await _unitOfWork.testCaseRepository.GetAsync(
                filter: t => t.Id == testcaseid && !t.Deleted,
                tracked: false,
                includeproperties: "Steps"
            );

            if (testCase == null)
            {
                _logger.LogWarning("TestCase ID: {TestCaseId} was not found.", testcaseid);
                throw new KeyNotFoundException($"TestCase with ID {testcaseid} was not found.");
            }

            return _mapper.Map<TestCaseResponseDTO>(testCase);
        }

        public async Task<IEnumerable<TestCaseResponseDTO>> GetAllByProjectIdAsync(long projectId)
        {
            _logger.LogInformation("Fetching all TestCases for Project ID: {ProjectId}", projectId);

            var query = _unitOfWork.testCaseRepository.GetAllAsync(
                filter: t => t.ProjectId == projectId && !t.Deleted,
                includeproperties: "Steps",
                pagesize: 100
            );

            var testCases = await query.ToListAsync();
            return _mapper.Map<IEnumerable<TestCaseResponseDTO>>(testCases);
        }

        public async Task<IEnumerable<TestCaseResponseDTO>> GetAllByRequirementIdAsync(long requirementId)
        {
            _logger.LogInformation("Fetching all TestCases for Requirement ID: {RequirementId}", requirementId);

            var query = _unitOfWork.testCaseRepository.GetAllAsync(
                filter: t => t.RequirementId == requirementId && !t.Deleted,
                includeproperties: "Steps",
                pagesize: 100
            );

            var testCases = await query.ToListAsync();
            return _mapper.Map<IEnumerable<TestCaseResponseDTO>>(testCases);
        }

        public async Task DeleteByRequirementAsync(long requirementId)
        {
            _logger.LogInformation("Soft-deleting TestCases for Requirement ID: {RequirementId}", requirementId);

            var query = _unitOfWork.testCaseRepository.GetAllAsync(
                filter: t => t.RequirementId == requirementId && !t.Deleted,
                pagesize: 100
            );

            var testCasesToDelete = await query.ToListAsync();

            if (testCasesToDelete.Any())
            {
                foreach (var testCase in testCasesToDelete)
                {
                    testCase.Deleted = true;
                }

                await _unitOfWork.testCaseRepository.SaveAsync();
                _logger.LogInformation("Successfully deleted {Count} TestCases.", testCasesToDelete.Count);
            }
            else
            {
                _logger.LogInformation("No active TestCases found to delete for Requirement ID: {RequirementId}", requirementId);
            }
        }

        public async Task DeleteByTestCaseAsync(long testcaseid)
        {
            _logger.LogInformation("Attempting to soft delete TestCase with ID: {TestCaseId}", testcaseid);

            try
            {
              
                var testCase = await _unitOfWork.testCaseRepository.GetAsync(t => t.Id == testcaseid);

               
                if (testCase == null)
                {
                    _logger.LogWarning("TestCase with ID: {TestCaseId} was not found for deletion.", testcaseid);
                    throw new KeyNotFoundException($"TestCase with ID {testcaseid} not found.");
                }

               
                testCase.Deleted = true;

                
                await _unitOfWork.testCaseRepository.SaveAsync();

                _logger.LogInformation("TestCase with ID: {TestCaseId} successfully soft-deleted.", testcaseid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting TestCase with ID: {TestCaseId}", testcaseid);
                throw;
            }
        }

    }
}