using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biblioteka.DTO
{
    public class LoanDTO
    {
        public int Id{get;set;}
        public string BookName { get; set; }
        public string Status { get; set; }
        public DateTime StartOfLoan { get; set; }

        public DateTime? EndOfLoan { get; set; }
    }
}