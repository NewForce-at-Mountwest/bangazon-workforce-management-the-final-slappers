using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using BangazonWorkforce.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonWorkforce.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IConfiguration _config;

        public DepartmentsController(IConfiguration config)
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

        // GET: DepartmentsController
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                        SELECT Department.Id AS DepartmentID,
                                            Department.Name,
                                            Department.Budget, Employee.Id AS EmployeeID
                                        FROM Department LEFT JOIN Employee ON Employee.DepartmentId = Department.Id";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Department> departments = new List<Department>();

                    while (reader.Read())
                    {
                        Department department = new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("DepartmentID")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                        };

                        if (departments.Any(dept => dept.Id == department.Id) == false)
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("EmployeeID")))
                            {
                                Employee employee = new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("EmployeeID"))
                                };

                                department.listOfEmployees.Add(employee);
                            }

                            departments.Add(department);
                        }

                        else
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("EmployeeID")))
                            {
                                Employee employee = new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("EmployeeID"))
                                };

                                departments.FirstOrDefault(d => d.Id == department.Id).listOfEmployees.Add(employee);
                            }
                        }
                    }

                    reader.Close();

                    return View(departments);
                }
            }
        }

        // GET: DepartmentsController/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                        SELECT Department.Id AS DepartmentID,
                                            Department.Name,
                                            Department.Budget, Employee.Id AS EmployeeID, Employee.FirstName, Employee.LastName
                                        FROM Department LEFT JOIN Employee ON Employee.DepartmentId = Department.Id WHERE Department.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    Department department = null;

                    while (reader.Read())
                    {
                        if (department == null)
                        {
                            department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentID")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                            };
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("EmployeeID")))
                        {
                            Employee employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            };

                            department.listOfEmployees.Add(employee);
                        }
                    }

                    reader.Close();

                    return View(department);
                }
            }
        }

        // GET: DepartmentsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DepartmentsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
