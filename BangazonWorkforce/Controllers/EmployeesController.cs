using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforce.Models;
using BangazonWorkforce.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace BangazonWorkforce.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IConfiguration _config;
        public EmployeesController(IConfiguration config)
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

        // GET: EmployeesController
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
            SELECT e.Id,
                e.FirstName,
                e.LastName,
                e.DepartmentId,
                Department.Name
                
            FROM Employee e JOIN Department ON e.DepartmentId = Department.Id
        ";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Employee> employees = new List<Employee>();
                    while (reader.Read())
                    {
                        Employee employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Department = new Department
                            {
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            }
                            
                        };

                        employees.Add(employee);
                    }

                    reader.Close();

                    return View(employees);
                }
            }
        }

        // GET: Employees/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Employees/Create
        public ActionResult Create()
  
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    // Select all the cohorts
                    cmd.CommandText = @"SELECT Department.Id, Department.Name FROM Department";

                    SqlDataReader reader = cmd.ExecuteReader();

                    // Create a new instance of our view model
                    EmployeeDepartmentViewModel viewModel = new EmployeeDepartmentViewModel();
                    while (reader.Read())
                    {
                        // Map the raw data to our cohort model
                        Department department = new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };

                        // Use the info to build our SelectListItem
                        SelectListItem departmentOptionTag = new SelectListItem()
                        {
                            Text = department.Name,
                            Value = department.Id.ToString()
                        };

                        // Add the select list item to our list of dropdown options
                        viewModel.departments.Add(departmentOptionTag);

                    }

                    reader.Close();


                    // send it all to the view
                    return View(viewModel);
                }
            }
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeDepartmentViewModel viewModel)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Employee
                ( FirstName, LastName, DepartmentId )
                VALUES
                ( @firstName, @lastName, @departmentId )";
                        cmd.Parameters.Add(new SqlParameter("@firstName", viewModel.employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", viewModel.employee.LastName));
                        
                        cmd.Parameters.Add(new SqlParameter("@departmentId", viewModel.employee.DepartmentId));
                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int id)
        {
            EmployeeEditViewModel viewModel = new EmployeeEditViewModel();

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                         SELECT e.Id,
                                            e.FirstName,
                                            e.LastName,
                                            e.DepartmentId,
                                            e.isSuperVisor,
                                            ce.ComputerId
                                        FROM Employee e LEFT JOIN ComputerEmployee ce ON ce.EmployeeId = e.Id
                                        WHERE e.Id = @id
                                    ";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    viewModel.employee = null;

                    if (reader.Read())
                    {
                        viewModel.employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("isSuperVisor"))
                        };

                        if(!reader.IsDBNull(reader.GetOrdinal("ComputerId")))
                        {
                            viewModel.ComputerId = reader.GetInt32(reader.GetOrdinal("ComputerId"));
                        }

                    }
                    reader.Close();
                }

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Department.Id, Department.Name FROM Department";

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Department department = new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };

                        SelectListItem departmentOptionTag = new SelectListItem()
                        {
                            Text = department.Name,
                            Value = department.Id.ToString()
                        };

                        viewModel.departments.Add(departmentOptionTag);
                    }

                    reader.Close();
                }

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Computer.Id, Computer.Make FROM Computer";

                    SqlDataReader reader = cmd.ExecuteReader();

                    SelectListItem nullOptionTag = new SelectListItem()
                    {
                        Text = "Please select a computer",
                        Value = 0.ToString()
                    };

                    viewModel.computers.Add(nullOptionTag);

                    while (reader.Read())
                    {
                        Computer computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make"))
                        };

                        SelectListItem computerOptionTag = new SelectListItem()
                        {
                            Text = computer.Make,
                            Value = computer.Id.ToString()
                        };

                        viewModel.computers.Add(computerOptionTag);
                    }

                    reader.Close();
                }



                if (viewModel.employee != null)
                {
                    return View(viewModel);
                }
                else
                {
                    return RedirectToAction(nameof(NotFound));
                }
            }
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EmployeeEditViewModel viewModel)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Employee
                                            SET FirstName=@firstName, 
                                            LastName=@lastName, 
                                            DepartmentId=@deptId, 
                                            isSuperVisor=@isSupervisor
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@firstName", viewModel.employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", viewModel.employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@deptId", viewModel.employee.DepartmentId));
                        cmd.Parameters.Add(new SqlParameter("@isSupervisor", viewModel.employee.IsSupervisor));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();

                    }

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE ComputerEmployee
                                            SET UnassignDate = @unassignDate
                                            WHERE EmployeeId=@employeeId AND UnassignDate IS NULL";
                        cmd.Parameters.Add(new SqlParameter("@unassignDate", DateTime.Now));
                        cmd.Parameters.Add(new SqlParameter("@employeeId", id));

                        int rowsAffected = cmd.ExecuteNonQuery();

                    }

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO ComputerEmployee
                                            ( EmployeeId, ComputerId, AssignDate )
                                            VALUES
                                            ( @employeeId, @computerId, @assigndate )";
                        cmd.Parameters.Add(new SqlParameter("@employeeId", id));
                        cmd.Parameters.Add(new SqlParameter("@computerId", viewModel.ComputerId));
                        cmd.Parameters.Add(new SqlParameter("@assignDate", DateTime.Now));

                        int rowsAffected = cmd.ExecuteNonQuery();


                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(viewModel);
            }
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Employees/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
