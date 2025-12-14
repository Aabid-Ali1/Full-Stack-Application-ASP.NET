using Microsoft.Data.SqlClient;
using System.Data;
using static FullStackServerSide.Program;
using static System.Data.CommandType;
using Microsoft.Extensions.Configuration;

namespace FullStackServerSide
{
    /// <summary>
    /// Provides database access methods for the ClassTrak application.
    /// Handles retrieval, update, and deletion of student and class data.
    /// </summary>
    public static class ClassTrak
    {
        // Connection string used to connect to the SQL Server database

        private static string ClassTrakConn;
        public static void Init(IConfiguration config)
        {
            ClassTrakConn = config.GetConnectionString("ClassTrak");
        }
        /// <summary>
        /// Retrieves a list of students whose first name starts with E or F.
        /// The first row contains column headers, followed by student records.
        /// </summary>
        /// <returns>A 2D list representing student data</returns>
        public static List<List<string>> GetStudents()
        {
            string query =
                "SELECT student_id AS 'StudentID', " +
                "first_name AS 'First Name', " +
                "last_name AS 'Last Name', " +
                "school_id AS 'School ID' " +
                "FROM Students s " +
                "WHERE (s.first_name LIKE 'E%' OR s.first_name LIKE 'F%') " +
                "ORDER BY s.first_name";

            return Query(query);
        }

        /// <summary>
        /// Retrieves all classes associated with a specific student.
        /// Joins Students, Classes, and Instructors tables.
        /// </summary>
        /// <param name="id">Student ID</param>
        /// <returns>A 2D list representing class and instructor data</returns>
        public static List<List<string>> GetClasses(int id)
        {
            string query =
                "SELECT C.class_id 'Class ID', " +
                "C.class_desc 'Class Desc', " +
                "COALESCE(C.[days], 0) 'Days', " +
                "C.[start_date] 'StartDate', " +
                "I.instructor_id 'Instructor ID', " +
                "I.first_name 'First Name', " +
                "I.last_name 'Last Name' " +
                "FROM Students S " +
                "INNER JOIN class_to_student CTS ON S.student_id = CTS.student_id " +
                "INNER JOIN Classes C ON CTS.class_id = C.class_id " +
                "INNER JOIN Instructors I ON C.instructor_id = I.instructor_id " +
                $"WHERE S.student_id = {id}";

            return Query(query);
        }

        /// <summary>
        /// Deletes a student using a stored procedure.
        /// Returns the number of affected rows and outputs status information.
        /// </summary>
        /// <param name="id">Student ID</param>
        /// <param name="message">Output message from stored procedure</param>
        /// <param name="status">Output status from stored procedure</param>
        /// <returns>Number of rows affected</returns>
        public static int DeleteStudent(int id, out string message, out string status)
        {
            using (SqlConnection conn = new SqlConnection(ClassTrakConn))
            {
                conn.Open();

                using (SqlCommand cmd = new("DeleteStudent", conn))
                {
                    // Indicate that the command is a stored procedure
                    cmd.CommandType = StoredProcedure;

                    // Input parameter: Student ID
                    SqlParameter param = cmd.Parameters.Add("@StudentID", SqlDbType.Int, 15);
                    param.Value = id;

                    // Output parameter: Status
                    param = cmd.Parameters.Add("@Status", SqlDbType.VarChar, 30);
                    param.Direction = ParameterDirection.Output;

                    // Output parameter: Message
                    param = cmd.Parameters.Add("@message", SqlDbType.VarChar, 30);
                    param.Direction = ParameterDirection.Output;

                    // Execute the stored procedure
                    int rows = cmd.ExecuteNonQuery();

                    // Retrieve output values
                    message = Convert.ToString(cmd.Parameters["@message"].Value);
                    status = Convert.ToString(cmd.Parameters["@Status"].Value);

                    return rows;
                }
            }
        }

        /// <summary>
        /// Updates student information using a stored procedure.
        /// </summary>
        /// <param name="info">Object containing updated student data</param>
        /// <returns>Number of rows affected</returns>
        public static int EditStudent(UpdateInfo info)
        {
            using (SqlConnection conn = new SqlConnection(ClassTrakConn))
            {
                conn.Open();

                using (SqlCommand cmd = new("EditStudent", conn))
                {
                    cmd.CommandType = StoredProcedure;

                    // Input parameters for the stored procedure
                    cmd.Parameters.Add("@StudentID", SqlDbType.Int).Value = info.id;
                    cmd.Parameters.Add("@FirstName", SqlDbType.VarChar, 30).Value = info.FName;
                    cmd.Parameters.Add("@LastName", SqlDbType.VarChar, 30).Value = info.LName;
                    cmd.Parameters.Add("@SchoolId", SqlDbType.VarChar, 30).Value = info.SchoolID;

                    // Execute update
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Executes a SQL SELECT query and returns the result as a 2D list.
        /// The first row contains column names.
        /// </summary>
        /// <param name="query">SQL query string</param>
        /// <returns>Query results as a list of rows and columns</returns>
        public static List<List<string>> Query(string query)
        {
            List<List<string>> retData = new();

            using (SqlConnection conn = new(ClassTrakConn))
            {
                conn.Open();

                using (SqlCommand cmd = new(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // Add column headers as the first row
                    retData.Add(new List<string>());
                    for (int i = 0; i < reader.FieldCount; i++)
                        retData[0].Add(reader.GetName(i));

                    // Read each row of data
                    while (reader.Read())
                    {
                        List<string> row = new();
                        for (int i = 0; i < reader.FieldCount; i++)
                            row.Add(reader[i].ToString());

                        retData.Add(row);
                    }
                }
            }

            return retData;
        }
    }
}
