using POEClassLibrary.Models;
using POEClassLibrary.Utils;
using System.Data;
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
        public int RemainingSelfStudyCalculation(DateTime currentDate, string moduleNames, int userId)
        {
            int remainingHours = 0;

            using (SqlConnection connection = new SqlConnection(Connection.Conn))
            {
                connection.Open();

                // Calculate the first and last day of the current week
                DateTime firstDay = currentDate.AddDays(-(int)currentDate.DayOfWeek);
                DateTime endDay = firstDay.AddDays(6);

                // Query the database to retrieve study records for the selected week
                string selectStudyRecordsQuery = "SELECT module_name, self_study_hours  FROM module " +
                    "WHERE StudyDate >= @StartDate AND StudyDate <= @EndDate AND UserID = @UserID";

                using (SqlCommand command = new SqlCommand(selectStudyRecordsQuery, connection))
                {
                    command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = firstDay;
                    command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDay;
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Calculate total study hours for the selected week
                        var totalStudyHours = new Dictionary<string, int>();
                        while (reader.Read())
                        {
                            string moduleName = reader["ModuleName"].ToString();
                            int studyHours = Convert.ToInt32(reader["NumberofStudyHours"]);

                            if (totalStudyHours.ContainsKey(moduleName))
                            {
                                totalStudyHours[moduleName] += studyHours;
                            }
                            else
                            {
                                totalStudyHours[moduleName] = studyHours;
                            }
                        }

                        // Calculate the remaining self-study hours for the specified module
                        if (totalStudyHours.ContainsKey(moduleNames))
                        {
                            int totalModuleStudyHours = totalStudyHours[moduleNames];

                            // Retrieve the initial self-study hours from the Modules table
                            string selectSelfStudyHoursQuery = "SELECT SelfStudyHours FROM Modules WHERE ModuleName = @ModuleName AND UserID = @UserID";
                            using (SqlCommand selfStudyHoursCommand = new SqlCommand(selectSelfStudyHoursQuery, connection))
                            {
                                selfStudyHoursCommand.Parameters.Add("@ModuleName", SqlDbType.VarChar).Value = moduleNames;
                                selfStudyHoursCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;

                                object selfStudyHoursObj = selfStudyHoursCommand.ExecuteScalar();
                                if (selfStudyHoursObj != DBNull.Value)
                                {
                                    int initialSelfStudyHours = Convert.ToInt32(selfStudyHoursObj);
                                    remainingHours = initialSelfStudyHours - totalModuleStudyHours;
                                }
                            }
                        }
                    }
                }

                // Update the Modules table with the new self-study hours
                string updateSelfStudyHoursQuery = "UPDATE module SET SelfStudyHours = @SelfStudyHours " +
                    "WHERE ModuleName = @ModuleName AND UserID = @UserID";
                using (SqlCommand updateCommand = new SqlCommand(updateSelfStudyHoursQuery, connection))
                {
                    updateCommand.Parameters.Add("@SelfStudyHours", SqlDbType.Int).Value = remainingHours;
                    updateCommand.Parameters.Add("@ModuleName", SqlDbType.VarChar).Value = moduleNames;
                    updateCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    updateCommand.ExecuteNonQuery();
                }
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

        public bool DoesUserHaveSemesterInfo(int userId)
        {
            using (SqlConnection connection = new SqlConnection(Connection.Conn))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM semester WHERE users_id = @UserID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Set the parameter for the user ID.
                    command.Parameters.Add(new SqlParameter("@UserID", userId));

                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }
    }
}

