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
                    cmd.CommandText = @"
            SELECT i.Id,
                i.FirstName,
                i.LastName,
                i.SlackHandle,
                i.CohortId,
                i.Specialty
            FROM Instructor i
        ";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<TrainingProgram> programList = new List<TrainingProgram>();
                    while (reader.Read())
                    {
                        Instructor instructorToAdd = new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Specialty = reader.GetString(reader.GetOrdinal("Specialty"))
                        };

                        instructors.Add(instructorToAdd);
                    }

                    reader.Close();

                    return View(instructors);
                }
            }
        }

        // GET: TrainingProgramsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TrainingProgramsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TrainingProgramsController/Create
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

        // GET: TrainingProgramsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TrainingProgramsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: TrainingProgramsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TrainingProgramsController/Delete/5
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
