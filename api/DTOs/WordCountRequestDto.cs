using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTOs
{
    public class WordCountRequestDto
    {
        public IFormFile File { get; set; } = null!;
    }
}