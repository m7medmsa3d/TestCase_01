using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestCase_01_DataAccess.Service.IService;
using TestCase_01_DTO;

namespace TestCase_01.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestCaseController : ControllerBase
    {
        private readonly ITestCaseService _testCaseService;

        public TestCaseController(ITestCaseService testCaseService)
        {
            _testCaseService = testCaseService;
        }

        #region 1️⃣ Create
        [HttpPost("/create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> createTestcases([FromBody] TestCaseRequestDTO testCaseRequestDto)
        {
            try
            {
               
                if (testCaseRequestDto == null || testCaseRequestDto.UserId == null || testCaseRequestDto.UserId <= 0)
                {
                    return BadRequest(new { message = "Invalid or missing UserId inside the request body. It must be greater than 0." });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Validation failed.", errors = ModelState });
                }

                await _testCaseService.CreateTestCaseAsync(testCaseRequestDto);

                return CreatedAtAction(nameof(GetTestCaseById), new { testcaseid = 0, userId = testCaseRequestDto.UserId }, new { message = "TestCase batch created successfully." });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        #endregion

        #region 2️⃣ Get Methods
        [HttpGet("/testcase/{testcaseid}/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTestCaseById(long testcaseid, int userId)
        {
            try
            {
                if (userId <= 0) return BadRequest(new { message = "Invalid or missing userId parameter. It must be greater than 0." });
                if (testcaseid <= 0) return BadRequest(new { message = "Invalid TestCase ID." });

                var testCaseDto = await _testCaseService.GetTestCaseByIdAsync(testcaseid, userId);

                if (testCaseDto == null)
                {
                    return NotFound(new { message = $"TestCase with ID {testcaseid} was not found, or access is denied for this user." });
                }

                return Ok(testCaseDto);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("/projects/{projectId}/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTestCasesByProjectId(long projectId,  int userId)
        {
            try
            {
                if (userId <= 0) return BadRequest(new { message = "Invalid or missing userId parameter. It must be greater than 0." });
                if (projectId <= 0) return BadRequest(new { message = "Invalid Project ID." });

                var testCases = await _testCaseService.GetAllByProjectIdAsync(projectId, userId);

             
                if (testCases == null || !testCases.Any())
                {
                    return NotFound(new { message = $"Project with ID {projectId} was not found, or it has no test cases associated  or access is denied for this user.." });
                }

                return Ok(testCases);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("/requirements/{requirementId}/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> getTestcasesByRequirementId(long requirementId,  int userId)
        {
            try
            {
                if (userId <= 0) return BadRequest(new { message = "Invalid or missing userId parameter. It must be greater than 0." });
                if (requirementId <= 0) return BadRequest(new { message = "Invalid Requirement ID." });

                var testCases = await _testCaseService.GetAllByRequirementIdAsync(requirementId, userId);

                
                if (testCases == null || !testCases.Any())
                {
                    return NotFound(new { message = $"Requirement with ID {requirementId} was not found, or it has no test cases associated  or access is denied for this user.." });
                }

                return Ok(testCases);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        #endregion

        #region 3️⃣ Delete Methods (Soft Delete)
        [HttpDelete("/requirements/{requirementId}/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteByRequirement(long requirementId, int userId)
        {
            try
            {
                if (userId <= 0) return BadRequest(new { message = "Invalid or missing userId parameter. It must be greater than 0." });
                if (requirementId <= 0) return BadRequest(new { message = "Invalid Requirement ID." });

                var existing = await _testCaseService.GetAllByRequirementIdAsync(requirementId, userId);
                if (existing == null || !existing.Any())
                {
                    return NotFound(new { message = $"Cannot delete. Requirement with ID {requirementId} was not found or has no active test cases  or access is denied for this user.." });
                }

                await _testCaseService.DeleteByRequirementAsync(requirementId, userId);
                return Ok(new { message = $"All test cases for requirement ID {requirementId} have been successfully soft-deleted." });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("/testcase/{testcaseid}/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTestCase(long testcaseid, int userId)
        {
            try
            {
                if (userId <= 0) return BadRequest(new { message = "Invalid or missing userId parameter. It must be greater than 0." });
                if (testcaseid <= 0) return BadRequest(new { message = "Invalid TestCase ID." });

                var testCase = await _testCaseService.GetTestCaseByIdAsync(testcaseid, userId);
                if (testCase == null)
                {
                    return NotFound(new { message = $"TestCase with ID {testcaseid} was not found or you don't have permission to delete it  or access is denied for this user.." });
                }

                await _testCaseService.DeleteByTestCaseAsync(testcaseid, userId);
                return Ok(new { message = "TestCase deleted successfully (Soft Delete)." });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        #endregion

        #region 4️⃣ Dashboard Analytics
        [HttpGet("/internal/users/{userId}/summary")]
        [ProducesResponseType(typeof(ProfileStatsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDashboardStats( int userId)
        {
            try
            {
                if (userId <= 0) return BadRequest(new { message = "Invalid or missing userId parameter. It must be greater than 0." });

                var stats = await _testCaseService.GetUserSummaryAsync(userId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        #endregion

        #region 5️⃣ Export Engines (Excel, Word, PDF)
        [HttpGet("/{testcaseId}/{userId}/{format}/export")]
        public async Task<IActionResult> ExportSingleTestCase(long testcaseId,  int userId,  string format = "pdf")
        {
            try
            {
                if (userId <= 0) return BadRequest(new { message = "Invalid or missing userId parameter. It must be greater than 0." });
                if (testcaseId <= 0) return BadRequest(new { message = "Invalid TestCase ID." });

                byte[] fileBytes = await _testCaseService.ExportTestCaseAsync(testcaseId, userId, format);
                return File(fileBytes, GetContentType(format), $"TestCase_{testcaseId}_{DateTime.UtcNow:yyyyMMdd}.{GetExtension(format)}");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = $"Export failed. {ex.Message}" });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("/requirements/{requirementId}/{userId}/{format}/export")]
        public async Task<IActionResult> ExportByRequirement(long requirementId,  int userId,  string format = "excel")
        {
            try
            {
                if (userId <= 0) return BadRequest(new { message = "Invalid or missing userId parameter. It must be greater than 0." });
                if (requirementId <= 0) return BadRequest(new { message = "Invalid Requirement ID." });

                byte[] fileBytes = await _testCaseService.ExportByRequirementAsync(requirementId, userId, format);
                return File(fileBytes, GetContentType(format), $"Requirement_{requirementId}_TestCases_{DateTime.UtcNow:yyyyMMdd}.{GetExtension(format)}");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = $"Export failed. {ex.Message}" });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("/projects/{projectId}/{userId}/{format}/export")]
        public async Task<IActionResult> ExportByProject(long projectId,  int userId, string format = "word")
        {
            try
            {
                if (userId <= 0) return BadRequest(new { message = "Invalid or missing userId parameter. It must be greater than 0." });
                if (projectId <= 0) return BadRequest(new { message = "Invalid Project ID." });

                byte[] fileBytes = await _testCaseService.ExportByProjectAsync(projectId, userId, format);
                return File(fileBytes, GetContentType(format), $"Project_{projectId}_TestCases_{DateTime.UtcNow:yyyyMMdd}.{GetExtension(format)}");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = $"Export failed. {ex.Message}" });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        #endregion

        #region 🧰 Helpers
        private string GetContentType(string format)
        {
            return format.ToLower() switch
            {
                "excel" or "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "word" or "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "pdf" => "application/pdf",
                _ => "application/octet-stream"
            };
        }

        private string GetExtension(string format)
        {
            return format.ToLower() switch
            {
                "excel" or "xlsx" => "xlsx",
                "word" or "docx" => "docx",
                "pdf" => "pdf",
                _ => "bin"
            };
        }

        private IActionResult HandleException(Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred on the server.", error = ex.Message });
        }
        #endregion
    }
}