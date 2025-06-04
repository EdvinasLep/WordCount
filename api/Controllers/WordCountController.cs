using Microsoft.AspNetCore.Mvc;
using api.Services;
using api.DTOs;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WordCountController : ControllerBase
    {
        private readonly IWordCountSummaryService _wordCountSummaryService;

        public WordCountController(IWordCountSummaryService wordCountSummaryService)
        {
            _wordCountSummaryService = wordCountSummaryService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<List<WordCountSummaryDto>>> WordCount([FromForm] List<IFormFile> files)
        {
            try
            {
                if (files == null || !files.Any() || files.All(f => f.Length == 0))
                {
                    return BadRequest("No files uploaded or all files are empty.");
                }

                var allWordCounts = new List<WordCountSummaryDto>();

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var fileResult = await _wordCountSummaryService.AnalyzeFileAsync(file);
                        allWordCounts.AddRange(fileResult);
                    }
                }

                var combinedWordCounts = allWordCounts
                    .GroupBy(wc => wc.Word?.ToLower())
                    .Select(g => new WordCountSummaryDto
                    {
                        Word = g.First().Word,
                        Count = g.Sum(x => x.Count)
                    })
                    .OrderByDescending(x => x.Count)
                    .ToList();

                return Ok(combinedWordCounts);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}