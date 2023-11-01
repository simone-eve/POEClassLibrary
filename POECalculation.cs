using POEClassLibrary.Utils;
using System.Data.SqlClient;

namespace POEClassLibrary
{
    public class POECalculation
    {
        public SqlConnection sqlConnection;
        //calculation to work out the self study hours
        public int SelfStudyCalculation(int numberCredits, int semesterWeek, int weeklyHours)
        {
            int selfStudyHours = (((numberCredits * 10) / semesterWeek) - weeklyHours);
            return selfStudyHours;
        }

        //calculation to work out how many self study hours are remaining after the user has tracked hours 
        public int RemainingSelfStudyCalculation(DateTime currentDate, string moduleNames)
        {
            //__________________code attribution______________
            //The following method was taken from Code Project
            //Author: dillip.aim11
            //Link: https://www.codeproject.com/Questions/312829/get-first-and-last-date-of-week-by-passing-week-nu
            DateTime firstDay = currentDate.AddDays(-(int)currentDate.DayOfWeek);//working out the start of the week
            DateTime endDay = firstDay.AddDays(6);//working out the end of the week
            //__________________end______________________


            //__________________code attribution________________
            //The following method was taken from Stack Overflow
            //Author: Tim Schmelter
            //Link: https://stackoverflow.com/questions/18375752/filter-large-list-based-on-date-time
            var studyRecord = ModelUtils.hours //filtering for the modules selected week
                .Where(record => record.StudyDate.Date >= firstDay && record.StudyDate.Date <= endDay) //code attribution
                .ToList();
            //__________________end______________________

            //__________________code attribution________________
            //The following method was taken from Stack Overflow
            //Author: Diana Ionita
            //Link: https://stackoverflow.com/questions/16100900/select-multiple-fields-group-by-and-sum
            var totalStudyHours = studyRecord //calculating the total hours for the week
                .GroupBy(record => record.ModuleName)
                .Select(g => new
                {
                    Key = g.Key,
                    hours = g.Sum(s => s.NumberofStudyHours),
                    ModuleName = g.First().ModuleName
                })
             .FirstOrDefault(item => item.ModuleName == moduleNames);
            //__________________end______________________

            var updateList = ModelUtils.module.FirstOrDefault(record => record.ModuleName == moduleNames); //filtering the list of modules 


            var ModulesResult = from i in ModelUtils.module
                                where i.ModuleName == moduleNames
                                select i;

            int remainingHours = 0;
            foreach (var module in ModulesResult)
            {
                remainingHours = module.SelfStudyHours - totalStudyHours.hours; //calculating the remaining hours of the week
                updateList.SelfStudyHours = remainingHours; //updating the list with the new self study hours
            }

            return remainingHours;
        }

        public int GetUserId(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(Connection.Conn))
            {
                connection.Open();

                // Define the SQL query to retrieve the users_id based on username and password.
                string query = "SELECT users_id FROM Registered_User WHERE username = @Username AND password = @Password";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Set the parameters for username and password.
                    command.Parameters.Add(new SqlParameter("@Username", username));
                    command.Parameters.Add(new SqlParameter("@Password", password)); // Note: You should store and compare passwords securely in a real application.

                    // Execute the query and retrieve the users_id.
                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        // A matching user was found, and you have their users_id.
                        return (int)result;
                    }
                    else
                    {
                        // No matching user found.
                        return -1; // You can choose to handle this case differently if needed.
                    }
                }
            }
        }
    }
}