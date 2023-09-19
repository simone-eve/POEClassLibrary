using POEClassLibrary.Utils;

namespace POEClassLibrary
{
    public class POECalculation
    {
        public int SelfStudyCalculation(int numberCredits, int semesterWeek, int weeklyHours)
        {
            int selfStudyHours = (((numberCredits * 10) / semesterWeek) - weeklyHours);
            return selfStudyHours;
        }

        public int RemainingSelfStudyCalculation(DateTime currentDate, string moduleNames)
        {
            //__________________code attribution______________________
            //The following method was taken from Code Project
            //Author: dillip.aim11
            //Link: https://www.codeproject.com/Questions/312829/get-first-and-last-date-of-week-by-passing-week-nu
            DateTime firstDay = currentDate.AddDays(-(int)currentDate.DayOfWeek);//code attribution
            DateTime endDay = firstDay.AddDays(6);
            //__________________end______________________


            //__________________code attribution______________________
            //The following method was taken from Stack Overflow
            //Author: Tim Schmelter
            //Link: https://stackoverflow.com/questions/18375752/filter-large-list-based-on-date-time
            var studyRecord = ModelUtils.hours
                .Where(record => record.StudyDate.Date >= firstDay && record.StudyDate.Date <= endDay) //code attribution
                .ToList();
            //__________________end______________________

            //__________________code attribution______________________
            //The following method was taken from Stack Overflow
            //Author: Diana Ionita
            //Link: https://stackoverflow.com/questions/16100900/select-multiple-fields-group-by-and-sum
            var totalStudyHours = studyRecord
                .GroupBy(record => record.ModuleName)
                .Select(g => new
                {
                    Key = g.Key,
                    hours = g.Sum(s => s.NumberofStudyHours),
                    ModuleName = g.First().ModuleName
                })
             .FirstOrDefault(item => item.ModuleName == moduleNames);
            //__________________end______________________

            var updateList = ModelUtils.module.FirstOrDefault(record => record.ModuleName == moduleNames);


            var ModulesResult = from i in ModelUtils.module
                                where i.ModuleName == moduleNames
                                select i;

            int remainingHours = 0;
            foreach (var module in ModulesResult)
            {
                remainingHours = module.SelfStudyHours - totalStudyHours.hours;
                updateList.SelfStudyHours = remainingHours;
            }

            return remainingHours;
        }
    }
}