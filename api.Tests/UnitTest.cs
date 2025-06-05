using System.Text;
using Xunit;
using api.Services;
using api.Repositories;
using api.Models;
using Microsoft.AspNetCore.Http;

namespace api.Tests
{
    public class TestWordCountRepository : IWordCountRepository
    {
        private List<WordCount> _data = new List<WordCount>();

        public Task CreateAsync(List<WordCount> wordCounts, string fileName)
        {
            _data.AddRange(wordCounts);
            return Task.CompletedTask;
        }

        public Task DeleteByFileNameAsync(string fileName)
        {
            _data.RemoveAll(w => w.FileName == fileName);
            return Task.CompletedTask;
        }

        public Task<List<WordCount>> GetAllAsync()
        {
            return Task.FromResult(_data.ToList());
        }
    }

    public class WordCountServiceTests
    {
        private IFormFile CreateTestFile(string content, string fileName = "test.txt")
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            
            var formFile = new FormFile(stream, 0, bytes.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };
            
            return formFile;
        }

        [Fact]
        public async Task AnalyzeFileAsync_WithSimpleText_ReturnsCorrectCounts()
        {
            var repository = new TestWordCountRepository();
            var service = new WordCountSummaryService(repository);
            var file = CreateTestFile("ipsum lorem ipsum IPsUM Lorem");

            var result = await service.AnalyzeFileAsync(file);

            var lorem = result.First(w => w.Word?.ToLower() == "lorem");
            var ipsum = result.First(w => w.Word?.ToLower() == "ipsum");
            
            Assert.Equal(2, lorem.Count);
            Assert.Equal(3, ipsum.Count);
        }

        [Fact]
        public async Task AnalyzeFileAsync_WithEmptyFile_ThrowsArgumentException()
        {
            var repository = new TestWordCountRepository();
            var service = new WordCountSummaryService(repository);
            var file = CreateTestFile("");

            await Assert.ThrowsAsync<ArgumentException>(() => service.AnalyzeFileAsync(file));
        }

        [Fact]
        public async Task AnalyzeFileAsync_IgnoresSingleCharacterWords()
        {
            var repository = new TestWordCountRepository();
            var service = new WordCountSummaryService(repository);
            var file = CreateTestFile("I am a tester");

            var result = await service.AnalyzeFileAsync(file);

            Assert.DoesNotContain(result, w => w.Word?.ToLower() == "i");
            Assert.DoesNotContain(result, w => w.Word?.ToLower() == "a");
            Assert.Contains(result, w => w.Word?.ToLower() == "am");
            Assert.Contains(result, w => w.Word?.ToLower() == "tester");
        }

        [Fact]
        public async Task AnalyzeFileAsync_ResultsAreSortedByCountDescending()
        {
            var repository = new TestWordCountRepository();
            var service = new WordCountSummaryService(repository);
            var file = CreateTestFile("the quick brown fox jumps over the lazy dog the");

            var result = await service.AnalyzeFileAsync(file);

            Assert.Equal("the", result.First().Word?.ToLower());
            Assert.Equal(3, result.First().Count);
            
            for (int i = 0; i < result.Count - 1; i++)
            {
                Assert.True(result[i].Count >= result[i + 1].Count);
            }
        }

        [Fact]
        public async Task GetWordCountSummaryAsync_ReturnsStoredData()
        {
            var repository = new TestWordCountRepository();
            var service = new WordCountSummaryService(repository);
            var fileName = "test.txt";
            var file = CreateTestFile("hello world hello", fileName);

            await service.AnalyzeFileAsync(file);

            var result = await service.GetWordCountSummaryAsync(fileName);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, w => w.Word?.ToLower() == "hello" && w.Count == 2);
            Assert.Contains(result, w => w.Word?.ToLower() == "world" && w.Count == 1);
        }
    }
}
