using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestCase_01_DataAccess.Entities;
using TestCase_01_DataAccess.Repository.IReposaitory;
using TestCase_01_DataAccess.Service.IService;
using TestCase_01_DTO;
using ClosedXML.Excel;
using Xceed.Words.NET;
using iTextSharp.text;
using iTextSharp.text.pdf;

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
                _logger.LogWarning("CreateTestCaseAsync received a null TestCaseRequestDTO.");
                throw new ArgumentNullException(nameof(testCaseRequestDto));
            }
            if (testCaseRequestDto.UserId == null)
            {
                throw new ArgumentException("Missing User ID.");
            }

            _logger.LogInformation("Creating Batch TestCases for Project: {ProjectId}, Requirement: {RequirementId}",
                testCaseRequestDto.ProjectId, testCaseRequestDto.RequirementId);

            try
            {
                var testCasesList = _mapper.Map<List<TestCase>>(testCaseRequestDto.Testcases);

                foreach (var testCase in testCasesList)
                {
                    testCase.ProjectId = testCaseRequestDto.ProjectId;
                    testCase.RequirementId = testCaseRequestDto.RequirementId;
                    testCase.UserId = testCaseRequestDto.UserId.Value;
                    testCase.CreatedAt = DateTime.UtcNow;
                    testCase.Deleted = false;
                }

                if (testCasesList.Any())
                {
                    foreach (var testCase in testCasesList)
                    {
                        await _unitOfWork.testCaseRepository.CreateAsync(testCase);
                    }
                    
                }

                _logger.LogInformation("{Count} TestCases successfully saved to DB.", testCasesList.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating TestCase.");
                throw;
            }
        }

        public async Task<TestCaseResponseDTO> GetTestCaseByIdAsync(long testcaseid, int userId)
        {
            _logger.LogInformation("Fetching TestCase ID: {TestCaseId} for User: {UserId}", testcaseid, userId);

            var testCase = await _unitOfWork.testCaseRepository.GetAsync(
                filter: t => t.Id == testcaseid && t.UserId == userId && !t.Deleted,
                tracked: false,
                includeproperties: "Steps"
            );

            if (testCase == null)
            {
                _logger.LogWarning("TestCase ID: {TestCaseId} was not found or access denied.", testcaseid);
                return null;
            }

            return _mapper.Map<TestCaseResponseDTO>(testCase);
        }

        public async Task<IEnumerable<TestCaseResponseDTO>> GetAllByProjectIdAsync(long projectId, int userId)
        {
            _logger.LogInformation("Fetching all TestCases for Project ID: {ProjectId} and User: {UserId}", projectId, userId);

            var query = _unitOfWork.testCaseRepository.GetAllAsync(
                filter: t => t.ProjectId == projectId && t.UserId == userId && !t.Deleted,
                includeproperties: "Steps",
                pagesize: 1000
            );

            var testCases = await query.ToListAsync();
            return _mapper.Map<IEnumerable<TestCaseResponseDTO>>(testCases);
        }

        public async Task<IEnumerable<TestCaseResponseDTO>> GetAllByRequirementIdAsync(long requirementId, int userId)
        {
            _logger.LogInformation("Fetching all TestCases for Requirement ID: {RequirementId} and User: {UserId}", requirementId, userId);

            var query = _unitOfWork.testCaseRepository.GetAllAsync(
                filter: t => t.RequirementId == requirementId && t.UserId == userId && !t.Deleted,
                includeproperties: "Steps",
                pagesize: 1000
            );

            var testCases = await query.ToListAsync();
            return _mapper.Map<IEnumerable<TestCaseResponseDTO>>(testCases);
        }

        public async Task DeleteByRequirementAsync(long requirementId, int userId)
        {
            _logger.LogInformation("Soft-deleting TestCases for Requirement ID: {RequirementId} and User: {UserId}", requirementId, userId);

            var query = _unitOfWork.testCaseRepository.GetAllAsync(
                filter: t => t.RequirementId == requirementId && t.UserId == userId && !t.Deleted,
                pagesize: 1000
            );

            var testCasesToDelete = await query.ToListAsync();

            if (testCasesToDelete.Any())
            {
                foreach (var testCase in testCasesToDelete)
                {
                    testCase.Deleted = true;
                }

                await _unitOfWork.testCaseRepository.SaveAsync();
                _logger.LogInformation("Successfully soft-deleted {Count} TestCases.", testCasesToDelete.Count);
            }
        }

        public async Task DeleteByTestCaseAsync(long testcaseid, int userId)
        {
            _logger.LogInformation("Attempting to soft delete TestCase with ID: {TestCaseId} for User: {UserId}", testcaseid, userId);

            var testCase = await _unitOfWork.testCaseRepository.GetAsync(t => t.Id == testcaseid && t.UserId == userId && !t.Deleted);

            if (testCase != null)
            {
                testCase.Deleted = true;
                await _unitOfWork.testCaseRepository.SaveAsync();
                _logger.LogInformation("TestCase with ID: {TestCaseId} successfully soft-deleted.", testcaseid);
            }
        }

        public async Task<ProfileStatsResponse> GetUserSummaryAsync(int userId)
        {
            _logger.LogInformation("Generating Profile Stats Dashboard for User: {UserId}", userId);

            var query = _unitOfWork.testCaseRepository.GetAllAsync(
                filter: t => t.UserId == userId && !t.Deleted,
                includeproperties: "Steps",
                pagesize: 5000 
            );

            var userTestCases = await query.ToListAsync();
            var totalCount = userTestCases.Count;

            var response = new ProfileStatsResponse
            {
                TestcasesCount = totalCount,
                ExportedTestcasesCount = userTestCases.Count(t => t.LastExportedAt != null),
                ProjectsCount = userTestCases.Select(t => t.ProjectId).Distinct().Count(),
                RequirementsCount = userTestCases.Select(t => t.RequirementId).Distinct().Count(),

                TestcaseTypeBreakdown = BuildTypeBreakdown(userTestCases),

                RecentActivity = userTestCases
                    .Where(t => t.CreatedAt != default)
                    .OrderByDescending(t => t.CreatedAt)
                    .Take(5)
                    .Select(t => new ProfileActivityResponse { Type = "TESTCASE_GENERATED", Name = t.Title, Time = t.CreatedAt })
                    .Concat(
                        userTestCases
                        .Where(t => t.LastExportedAt != null)
                        .OrderByDescending(t => t.LastExportedAt)
                        .Take(5)
                        .Select(t => new ProfileActivityResponse { Type = "EXPORTED_TESTCASE", Name = t.Title, Time = t.LastExportedAt.Value })
                    ).ToList()
            };

            response.ProjectBreakdown = userTestCases
                .GroupBy(t => t.ProjectId)
                .Select(g => new DashboardBreakdownResponse
                {
                    Id = g.Key,
                    Name = $"Project {g.Key}",
                    TestcasesCount = g.Count(),
                    ExportedTestcasesCount = g.Count(t => t.LastExportedAt != null),
                    LastActivityAt = g.Max(t => t.LastExportedAt ?? t.CreatedAt),
                    TestcaseTypeBreakdown = BuildTypeBreakdown(g.ToList())
                }).ToList();

            response.RequirementBreakdown = userTestCases
                .GroupBy(t => t.RequirementId)
                .Select(g => new DashboardBreakdownResponse
                {
                    Id = g.Key,
                    Name = $"Requirement {g.Key}",
                    TestcasesCount = g.Count(),
                    ExportedTestcasesCount = g.Count(t => t.LastExportedAt != null),
                    LastActivityAt = g.Max(t => t.LastExportedAt ?? t.CreatedAt)
                }).ToList();

            var generatedByDate = userTestCases.GroupBy(t => t.CreatedAt.Date);
            var exportedByDate = userTestCases.Where(t => t.LastExportedAt.HasValue).GroupBy(t => t.LastExportedAt.Value.Date);
            var allDates = generatedByDate.Select(g => g.Key).Union(exportedByDate.Select(g => g.Key)).OrderBy(d => d);

            response.GenerationTrend = allDates.Select(date => new DashboardTrendPointResponse
            {
                date = date,
                GeneratedTestCases = userTestCases.Count(t => t.CreatedAt.Date == date),
                exports = userTestCases.Count(t => t.LastExportedAt.HasValue && t.LastExportedAt.Value.Date == date)
            }).ToList();

            return response;
        }

       
        public async Task<byte[]> ExportTestCaseAsync(long testcaseId, int userId, string format)
        {
            var testCase = await _unitOfWork.testCaseRepository.GetAsync(
                filter: t => t.Id == testcaseId && t.UserId == userId && !t.Deleted,
                tracked: true,
                includeproperties: "Steps"
            );

            if (testCase == null) throw new KeyNotFoundException("No testcase found or access denied.");
            else
            {
                testCase.LastExportedAt = DateTime.UtcNow;

                
                _unitOfWork.testCaseRepository.Update(testCase);
                await _unitOfWork.testCaseRepository.SaveAsync();
            }

                return await ExportManyAsync(new List<TestCase> { testCase }, format);
        }

     
        public async Task<byte[]> ExportByRequirementAsync(long requirementId, int userId, string format)
        {
            var query = _unitOfWork.testCaseRepository.GetAllAsync(
                filter: t => t.RequirementId == requirementId && t.UserId == userId && !t.Deleted,
                includeproperties: "Steps",
                pagesize: 1000
            );

            var testCases = await query.ToListAsync();
            if (!testCases.Any()) throw new KeyNotFoundException("No testcases found for this requirement.");
           
                return await ExportManyAsync(testCases, format);
        }

     
        public async Task<byte[]> ExportByProjectAsync(long projectId, int userId, string format)
        {
            var query = _unitOfWork.testCaseRepository.GetAllAsync(
                filter: t => t.ProjectId == projectId && t.UserId == userId && !t.Deleted,
                includeproperties: "Steps",
                pagesize: 1000
            );

            var testCases = await query.ToListAsync();
            if (!testCases.Any()) throw new KeyNotFoundException("No testcases found for this project.");

            return await ExportManyAsync(testCases, format);
        }

        #region Helper Methods

        private async Task<byte[]> ExportManyAsync(List<TestCase> testCases, string format)
        {
            byte[] fileBytes = format.ToLower() switch
            {
                "excel" or "xlsx" => ExportExcel(testCases),
                "word" or "docx" => ExportWord(testCases),
                "pdf" => ExportPdf(testCases),
                _ => throw new ArgumentException("Unsupported export format")
            };

            var now = DateTime.UtcNow;
            foreach (var tc in testCases)
            {
                tc.LastExportedAt = now;
                

               
                _unitOfWork.testCaseRepository.Update(tc);

               
                
            }
           

            
           
            await _unitOfWork.testCaseRepository.SaveAsync();

            return fileBytes;
        }

        private byte[] ExportExcel(List<TestCase> testCases)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Testcases");
                var headers = new[] { "ID", "Title", "Type", "Requirement ID", "Project ID", "Steps", "Expected Result" };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(1, i + 1).Value = headers[i];
                    worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                }

                for (int i = 0; i < testCases.Count; i++)
                {
                    var tc = testCases[i];
                    var row = i + 2;
                    worksheet.Cell(row, 1).Value = tc.Id;
                    worksheet.Cell(row, 2).Value = tc.Title ?? "";
                    worksheet.Cell(row, 3).Value = tc.Type ?? "";
                    worksheet.Cell(row, 4).Value = tc.RequirementId;
                    worksheet.Cell(row, 5).Value = tc.ProjectId;
                    worksheet.Cell(row, 6).Value = string.Join(" | ", tc.Steps.Select(s => s.Step));
                    worksheet.Cell(row, 7).Value = tc.ExpectedResult ?? "";
                }

                worksheet.Columns().AdjustToContents();
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        private byte[] ExportWord(List<TestCase> testCases)
        {
            using (var stream = new MemoryStream())
            {
               
                using (var document = Xceed.Words.NET.DocX.Create(stream))
                {
                    
                    var titleFormat = new Xceed.Document.NET.Formatting
                    {
                        Size = 18,
                        Bold = true
                    };

                 
                    var titleParagraph = document.InsertParagraph("Testcases Export", false, titleFormat);
                    titleParagraph.Alignment = Xceed.Document.NET.Alignment.center;

                  
                    foreach (var tc in testCases)
                    {
                        document.InsertParagraph($"Title: {tc.Title}").Bold();
                        document.InsertParagraph($"Type: {tc.Type}");
                        document.InsertParagraph($"Requirement ID: {tc.RequirementId}");
                        document.InsertParagraph($"Project ID: {tc.ProjectId}");
                        document.InsertParagraph("Steps:").Bold();

                        if (tc.Steps != null)
                        {
                            foreach (var step in tc.Steps)
                            {
                                document.InsertParagraph($"- {step.Step}");
                            }
                        }

                        document.InsertParagraph($"Expected Result: {tc.ExpectedResult}").Bold();
                        document.InsertParagraph("---------------------------------------");
                    }

                   
                    document.Save();
                }
                return stream.ToArray();
            }
        }

        private byte[] ExportPdf(List<TestCase> testCases)
        {
            using (var stream = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 45, 45, 50, 50);
                var writer = PdfWriter.GetInstance(document, stream);
                document.Open();

                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                document.Add(new Paragraph("Testcases Export", titleFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 20 });

                foreach (var tc in testCases)
                {
                    document.Add(new Paragraph($"Title: {tc.Title}", boldFont));
                    document.Add(new Paragraph($"Type: {tc.Type}", normalFont));
                    document.Add(new Paragraph($"Requirement ID: {tc.RequirementId}", normalFont));
                    document.Add(new Paragraph($"Project ID: {tc.ProjectId}", normalFont));
                    document.Add(new Paragraph("Steps:", boldFont));

                    foreach (var step in tc.Steps)
                    {
                        document.Add(new Paragraph($"- {step.Step}", normalFont));
                    }

                    document.Add(new Paragraph($"Expected Result: {tc.ExpectedResult}", boldFont));
                    document.Add(new Paragraph("\n"));
                }

                document.Close();
                return stream.ToArray();
            }
        }

        private List<TestcaseTypeBreakdownResponse> BuildTypeBreakdown(List<TestCase> testCases)
        {
            long total = testCases.Count;
            if (total == 0) return new List<TestcaseTypeBreakdownResponse>();

            return testCases
                .GroupBy(t => string.IsNullOrWhiteSpace(t.Type) ? "UNSPECIFIED" : t.Type.Trim().ToUpper())
                .Select(g => new TestcaseTypeBreakdownResponse
                {
                    Type = g.Key,
                    Count = g.Count(),
                    Percentage = Math.Round((g.Count() * 100.0) / total, 2)
                })
                .OrderBy(b => b.Type)
                .ToList();
        }

        #endregion
    }
}