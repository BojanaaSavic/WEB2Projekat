using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebProjekat.Application.DTOs
{
    public class SharedTokenDTO
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public string AccessType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
