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
        public async Task<ActionResult<List<WordCountSummaryDto>>> WordCount(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded or file is empty.");
                }

                var result = await _wordCountSummaryService.AnalyzeFileAsync(file);
                return Ok(result);
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