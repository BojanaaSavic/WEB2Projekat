using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebProjekat.Application.DTOs
{
    public class ExpenseCategoryDTO
    {
        public string Category { get; set; }
        public decimal Total { get; set; }
        public int Count { get; set; }
    }
}
