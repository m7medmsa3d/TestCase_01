using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TestCase_01_DataAccess;
using TestCase_01_DataAccess.Service.IService;
using TestCase_01_DTO;

namespace TestCase_01.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TestCaseController : ControllerBase
    {
        private readonly ITestCaseService _testCaseService;
        protected APIResponse _response;

        public TestCaseController(ITestCaseService testCaseService)
        {
            _testCaseService = testCaseService;
            _response = new APIResponse(); 
        }

       
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTestCase([FromBody] TestCaseRequestDTO testCaseRequestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.DescriptiveMessage = "Validation failed.";
                    return BadRequest(_response);
                }

                await _testCaseService.CreateTestCaseAsync(testCaseRequestDto);

                _response.StatusCode = HttpStatusCode.Created;
                _response.IsSuccess = true;
                _response.DescriptiveMessage = "TestCase created successfully.";

                return CreatedAtAction(nameof(GetTestCaseById), new { testcaseid = 0 }, _response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

       
        [HttpGet("{testcaseid:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTestCaseById(long testcaseid)
        {
            try
            {
                var testCaseDto = await _testCaseService.GetTestCaseByIdAsync(testcaseid);

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.DescriptiveMessage = "TestCase retrieved successfully.";
                _response.Result = testCaseDto;

                return Ok(_response);
            }
            catch (KeyNotFoundException ex)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);
                _response.DescriptiveMessage = "Resource not found.";
                return NotFound(_response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        
        [HttpGet("by-project/{projectId:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTestCasesByProjectId(long projectId)
        {
            try
            {
                var testCases = await _testCaseService.GetAllByProjectIdAsync(projectId);

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.DescriptiveMessage = $"Retrieved all test cases for project ID {projectId}.";
                _response.Result = testCases;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        
        [HttpGet("by-requirement/{requirementId:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTestCasesByRequirementId(long requirementId)
        {
            try
            {
                var testCases = await _testCaseService.GetAllByRequirementIdAsync(requirementId);

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.DescriptiveMessage = $"Retrieved all test cases for requirement ID {requirementId}.";
                _response.Result = testCases;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

       
        [HttpDelete("by-requirement/{requirementId:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteByRequirement(long requirementId)
        {
            try
            {
                await _testCaseService.DeleteByRequirementAsync(requirementId);

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.DescriptiveMessage = $"All test cases for requirement ID {requirementId} have been soft-deleted.";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("delete-testcase/{testcaseid}")]
        public async Task<IActionResult> DeleteTestCase(long testcaseid)
        {
            try
            {
                if (testcaseid <= 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.DescriptiveMessage = "Invalid TestCase ID.";
                    return BadRequest(_response);
                }

                await _testCaseService.DeleteByTestCaseAsync(testcaseid);

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.DescriptiveMessage = "TestCase deleted successfully (Soft Delete).";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }


        private IActionResult HandleException(Exception ex)
        {
            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.IsSuccess = false;
            _response.DescriptiveMessage = "An unexpected error occurred.";
            _response.ErrorMessages = new List<string> { ex.Message };

            return StatusCode(StatusCodes.Status500InternalServerError, _response);
        }
    }
}