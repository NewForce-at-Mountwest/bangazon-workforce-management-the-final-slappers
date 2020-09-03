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
    public class ComputerController : Controller
    {
        private readonly IConfiguration _config;

        public ComputerController(IConfiguration config)
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
        // GET: ComputerController
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT
                        Id,
                        Make
                    FROM Computer
                                    ";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Computer> computers = new List<Computer>();
                    while (reader.Read())
                    {
                        Computer computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            //Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            Make = reader.GetString(reader.GetOrdinal("Make"))
                           //PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate"))
                        };

                        computers.Add(computer);
                    }

                    reader.Close();

                    return View(computers);
                }
            }
        }

        // GET: ComputerController/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    // Select a single computer using SQL by their id
                    cmd.CommandText = @"
                                        SELECT
                                                Id,
                                                Manufacturer,
                                                Make,
                                                PurchaseDate,
                                                DecomissionDate
                                        FROM Computer 
                                        WHERE id = @id
                                                                 ";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    // Map the raw SQL data to a computer model
                    Computer computer = null;
                    if (reader.Read())
                    {
                        DateTime? dateTime = null;
                        computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            DecomissionDate = reader.IsDBNull(reader.GetOrdinal("DecomissionDate")) ?  dateTime :  reader.GetDateTime(reader.GetOrdinal("DecomissionDate"))
                        };

                    }

                    reader.Close();

                    // If we got something back to the db, send us to the details view
                    if (computer != null)
                    {
                        return View(computer);
                    }
                    else
                    {
                        // If we didn't get anything back from the db, we made a custom not found page down here
                        return RedirectToAction(nameof(NotFound));
                    }

                }
            }
        }

        // GET: ComputerController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ComputerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Computer computerToAdd)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Computer (PurchaseDate, Make, Manufacturer)
                                        OUTPUT INSERTED.Id
                                        VALUES (@purchasedate, @make, @manufacturer)";
                        cmd.Parameters.Add(new SqlParameter("@purchasedate", computerToAdd.PurchaseDate));
                        cmd.Parameters.Add(new SqlParameter("@make", computerToAdd.Make));
                        cmd.Parameters.Add(new SqlParameter("@manufacturer", computerToAdd.Manufacturer));

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

        // GET: ComputerController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ComputerController/Edit/5
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

        // GET: ComputerController/Delete/5
        public ActionResult Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
            SELECT       Id,
                         Manufacturer,
                         Make,
                         PurchaseDate,
                         DecomissionDate
            FROM Computer
            WHERE Id = @id
        ";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    Computer computer = null;
                    DateTime? dateTime = null;
                    while (reader.Read())
                    {
                        computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            DecomissionDate = reader.IsDBNull(reader.GetOrdinal("DecomissionDate")) ? dateTime : reader.GetDateTime(reader.GetOrdinal("DecomissionDate"))
                        };
                    }

                    reader.Close();

                    return View(computer);
                }
            }
        }

        // POST: ComputerController/Delete/5
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
                        cmd.CommandText = @"DELETE FROM Computer WHERE Id = @id";
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
