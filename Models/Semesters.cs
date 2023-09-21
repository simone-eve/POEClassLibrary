using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POEClassLibrary.Models
{
    public class Semesters
    {
        //properties for the semester class
        public Semesters(int numberOfWeeks, DateTime startDate)
        {
            NumberOfWeeks = numberOfWeeks;
            StartDate = startDate;
        }

        public int NumberOfWeeks { get; set; }
        public DateTime StartDate { get; set; }

    }
}
