using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Repositories
{
    public interface IWordCountRepository
    {
        Task CreateAsync(List<WordCount> wordCount, string fileName);
        Task<List<WordCount>> GetAllAsync();
        Task DeleteByFileNameAsync(string fileName);
    }
}