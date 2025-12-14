using Microsoft.Data.SqlClient;

namespace FullStackServerSide
{
    public class Program
    {
        /// <summary>
        /// DTO (Data Transfer Object) used for updating student information.
        /// Automatically bound from JSON in PUT requests.
        /// </summary>
        public record UpdateInfo(int id, string FName, string LName, int SchoolID);

        public static void Main(string[] args)
        {
            // Create the WebApplication builder
            var builder = WebApplication.CreateBuilder(args);

            // Register controller support (used for model binding and routing)
            builder.Services.AddControllers();

            // Register IConfiguration for dependency injection
            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

            // Build the application
            var app = builder.Build();

            // Configure CORS to allow requests from any origin
            // Useful for frontend applications running on a different host/port
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
            );

            // Enable detailed developer exception pages
            app.UseDeveloperExceptionPage();

            // Initialize database connection settings for the ClassTrak data layer
            ClassTrak.Init(app.Configuration);

            // Default test endpoint to verify the server is running
            app.MapGet("/", () => "Hello World!");

            /// <summary>
            /// Startup endpoint used to retrieve the initial student list.
            /// </summary>
            app.MapGet("/StartUp", () =>
            {
                List<List<string>> Students = ClassTrak.GetStudents();
                return new { students = Students };
            });

            /// <summary>
            /// Retrieves class information for a specific student.
            /// </summary>
            /// <param name="id">Student ID</param>
            app.MapGet("/Retrieve", (int id) =>
            {
                List<List<string>> Classes = ClassTrak.GetClasses(id);
                return new { table = Classes };
            });

            /// <summary>
            /// Deletes a student record using a stored procedure.
            /// Returns updated student data and status information.
            /// </summary>
            /// <param name="id">Student ID</param>
            app.MapDelete("/Delete", (int id) =>
            {
                string message, status;

                int rowsAffected = ClassTrak.DeleteStudent(id, out message, out status);

                // Refresh student list after deletion
                List<List<string>> Students = ClassTrak.GetStudents();

                return new
                {
                    rows = rowsAffected,
                    students = Students,
                    message,
                    status
                };
            });

            /// <summary>
            /// Updates an existing student record.
            /// Expects JSON body matching UpdateInfo record.
            /// </summary>
            app.MapPut("/Update", (UpdateInfo Info) =>
            {
                int rows = ClassTrak.EditStudent(Info);
                return new { rows };
            });

            // Start the web application
            app.Run();
        }
    }
}
