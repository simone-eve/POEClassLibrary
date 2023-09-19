using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POEClassLibrary.Models
{
    public class Modules
    {
        public Modules(string moduleCode, string moduleName, int numberOfCredits, int weeklyClassHours, int selfStudyHours)
        {
            ModuleCode = moduleCode;
            ModuleName = moduleName;
            NumberOfCredits = numberOfCredits;
            WeeklyClassHours = weeklyClassHours;
            SelfStudyHours = selfStudyHours;
        }

        public string ModuleCode { get; set; }
        public string ModuleName { get; set; }
        public int NumberOfCredits { get; set; }
        public int WeeklyClassHours { get; set; }
        public int SelfStudyHours { get; set; }
    }
}
