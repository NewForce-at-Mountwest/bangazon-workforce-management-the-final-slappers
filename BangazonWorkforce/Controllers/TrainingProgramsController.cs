using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using BangazonWorkforce.Models;
//using BangazonWorkforce.Models.ViewModels;

namespace BangazonWorkforce.Controllers
{
    public class TrainingProgramsController : Controller
    {
        private readonly IConfiguration _config;

        public TrainingProgramsController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET: TrainingProgramsController
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    DateTime today = DateTime.Today;
                    cmd.CommandText = @"
            SELECT tp.Id,
                tp.Name,
                tp.StartDate,
                tp.EndDate,
                tp.MaxAttendees
            FROM TrainingProgram tp
            WHERE EndDate > @today
        ";
                    cmd.Parameters.Add(new SqlParameter("@today", today));
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<TrainingProgram> programList = new List<TrainingProgram>();
                    while (reader.Read())
                    {
                        TrainingProgram programToAdd = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };

                        programList.Add(programToAdd);
                    }

                    reader.Close();

                    return View(programList);
                }
            }
        }

        // GET: TrainingProgramsController/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
            SELECT tp.Id,
                tp.Name,
                tp.StartDate,
                tp.EndDate,
                tp.MaxAttendees
            FROM TrainingProgram tp
            WHERE tp.Id = @id
        ";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    TrainingProgram program = null;
                    while (reader.Read())
                    {
                        program = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };
                    }

                    reader.Close();

                    cmd.CommandText = @"
            SELECT e.Id AS EmployeePrimaryKey,
                e.FirstName,
                e.LastName,
                e.DepartmentId,
                e.IsSuperVisor,
                d.Id AS DepartmentPrimaryKey,
                d.Name,
                d.Budget
            FROM EmployeeTraining ep
            JOIN Employee e ON ep.EmployeeId = e.Id
            JOIN Department d ON e.DepartmentId = d.Id
            WHERE ep.TrainingProgramId = @id
        ";

                    SqlDataReader secondReader = cmd.ExecuteReader();
                    
                    while (secondReader.Read())
                    {
                        Employee employee = new Employee
                        {
                            Id = secondReader.GetInt32(secondReader.GetOrdinal("EmployeePrimaryKey")),
                            FirstName = secondReader.GetString(secondReader.GetOrdinal("FirstName")),
                            LastName = secondReader.GetString(secondReader.GetOrdinal("LastName")),
                            DepartmentId = secondReader.GetInt32(secondReader.GetOrdinal("DepartmentId")),
                            IsSupervisor = secondReader.GetBoolean(secondReader.GetOrdinal("IsSuperVisor")),
                            departmentOfEmployee = new Department
                            {
                                Id = secondReader.GetInt32(secondReader.GetOrdinal("DepartmentPrimaryKey")),
                                Name = secondReader.GetString(secondReader.GetOrdinal("Name")),
                                Budget = secondReader.GetInt32(secondReader.GetOrdinal("Budget"))
                            }
                        };

                        if (!program.AttendingEmployees.Any(attendingEmployee => attendingEmployee.Id == employee.Id))
                        {
                            program.AttendingEmployees.Add(employee);
                        }
                    }

                    secondReader.Close();

                    return View(program);
                }
            }
        }

        // GET: TrainingProgramsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TrainingProgramsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TrainingProgram programToAdd)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO TrainingProgram (Name, StartDate, EndDate, MaxAttendees)
                                        OUTPUT INSERTED.Id
                                        VALUES (@name, @startDate, @endDate, @maxAttendees)";
                        cmd.Parameters.Add(new SqlParameter("@name", programToAdd.Name));
                        cmd.Parameters.Add(new SqlParameter("@startDate", programToAdd.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@endDate", programToAdd.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@maxAttendees", programToAdd.MaxAttendees));

                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TrainingProgramsController/Edit/5
        public ActionResult Edit(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
            SELECT tp.Id,
                tp.Name,
                tp.StartDate,
                tp.EndDate,
                tp.MaxAttendees
            FROM TrainingProgram tp
            WHERE tp.Id = @id
        ";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    TrainingProgram program = null;
                    while (reader.Read())
                    {
                        program = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };
                    }

                    reader.Close();

                    return View(program);
                }
            }
        }

        // POST: TrainingProgramsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TrainingProgram programToEdit)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE TrainingProgram
                                            SET Name = @name,
                                                StartDate = @startDate,
                                                EndDate = @endDate,
                                                MaxAttendees = @maxAttendees
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@name", programToEdit.Name));
                        cmd.Parameters.Add(new SqlParameter("@startDate", programToEdit.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@endDate", programToEdit.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@maxAttendees", programToEdit.MaxAttendees));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Details), new { id = programToEdit.Id.ToString() });
            }
            catch
            {
                return View();
            }
        }

        // GET: TrainingProgramsController/Delete/5
        public ActionResult Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
            SELECT tp.Id,
                tp.Name,
                tp.StartDate,
                tp.EndDate,
                tp.MaxAttendees
            FROM TrainingProgram tp
            WHERE tp.Id = @id
        ";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    TrainingProgram program = null;
                    while (reader.Read())
                    {
                        program = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };
                    }

                    reader.Close();

                    return View(program);
                }
            }
        }

        // POST: TrainingProgramsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM TrainingProgram WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
