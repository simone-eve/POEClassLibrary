using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POEClassLibrary.Models
{
    public class SelfStudyHours
    {
        //properties for the self study class
        public SelfStudyHours(string moduleName, int numberofStudyHours, DateTime studyDate)
        {
            ModuleName = moduleName;
            NumberofStudyHours = numberofStudyHours;
            StudyDate = studyDate;
        }

        public string ModuleName { get; set; }
        public int NumberofStudyHours { get; set; }
        public DateTime StudyDate { get; set; }
    }
}
