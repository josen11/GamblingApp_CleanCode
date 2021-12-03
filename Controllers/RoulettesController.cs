using GamblingApp.DTO;
using GamblingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GamblingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoulettesController : ControllerBase
    {
        private readonly IOptions<ConnectionStrings> appSettings;
        public RoulettesController(IOptions<ConnectionStrings> app)
        {
            appSettings = app;
        }
        // GET: api/<RoulettesController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RouletteModel>>> Get()
        {
            string constr = appSettings.Value.DefaultConnection;
            List<RouletteModel> roulettes = new List<RouletteModel>();
            string query = "SELECT * FROM Roulette";
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            roulettes.Add(new RouletteModel
                            {
                                Id = Convert.ToInt32(sdr["Id"]),
                                Status = Convert.ToBoolean(sdr["Status"]),
                                CreationDateTime = Convert.ToDateTime(sdr["CreationDateTime"]),
                                OpenDateTime = Convert.ToDateTime(sdr["OpenDateTime"]),
                                ClousureDateTime = Convert.ToDateTime(sdr["ClousureDateTime"]),
                                WinnerNumber = Convert.ToInt32(sdr["WinnerNumber"]),
                                Profit = Convert.ToDouble(sdr["Profit"])
                            });
                        }
                    }
                    con.Close();
                }
            }
            return roulettes;
        }

        // GET api/<RoulettesController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RouletteModel>> Get(int id)
        {
            string constr = appSettings.Value.DefaultConnection;
            RouletteModel rouletteObj = new RouletteModel();
            string query = "SELECT * FROM Roulette where Id=" + id;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            rouletteObj = new RouletteModel
                            {
                                Id = Convert.ToInt32(sdr["Id"]),
                                Status = Convert.ToBoolean(sdr["Status"]),
                                CreationDateTime = Convert.ToDateTime(sdr["CreationDateTime"]),
                                OpenDateTime = Convert.ToDateTime(sdr["OpenDateTime"]),
                                ClousureDateTime = Convert.ToDateTime(sdr["ClousureDateTime"]),
                                WinnerNumber = Convert.ToInt32(sdr["WinnerNumber"]),
                                Profit = Convert.ToDouble(sdr["Profit"])
                            };
                        }
                    }
                    con.Close();
                }
            }
            if (rouletteObj == null)
            {
                return NotFound();
            }
            return rouletteObj;
        }

        // POST api/<RoulettesController>
        [HttpPost]
        public async Task<ActionResult<RouletteModel>> Post(RouletteModel rouletteModel)
        {
            string constr = appSettings.Value.DefaultConnection;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
    }
            using (SqlConnection con = new SqlConnection(constr))
            {
                //inserting Patient data into database
                string query = "insert into Roulette values (@Status, @CreationDateTime, @ClousureDateTime,@WinnerNumber,@Profit, @OpenDateTime)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@Status", rouletteModel.Status);
                    cmd.Parameters.AddWithValue("@CreationDateTime", rouletteModel.CreationDateTime);
                    cmd.Parameters.AddWithValue("@OpenDateTime", rouletteModel.OpenDateTime);
                    cmd.Parameters.AddWithValue("@ClousureDateTime", rouletteModel.ClousureDateTime);
                    cmd.Parameters.AddWithValue("@WinnerNumber", rouletteModel.WinnerNumber);
                    cmd.Parameters.AddWithValue("@Profit", rouletteModel.Profit);
                    con.Open();
                    int i = cmd.ExecuteNonQuery();
                    if (i > 0)
                    {
                        return Ok();
                    }
                    con.Close();
                }
            }
            return BadRequest();
        }
        // POST api/<RoulettesController>/create
        [HttpPost("[action]")]
        public async Task<ActionResult<RouletteCreateResponseDTO>> create()
        {
            string constr = appSettings.Value.DefaultConnection;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (SqlConnection con = new SqlConnection(constr))
            {
                //inserting Patient data into database
                string query = "insert into Roulette output INSERTED.ID values (@Status, @CreationDateTime, @ClousureDateTime,@WinnerNumber,@Profit,@OpenDateTime)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@Status", false);
                    cmd.Parameters.AddWithValue("@CreationDateTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@ClousureDateTime", "");
                    cmd.Parameters.AddWithValue("@OpenDateTime", "");
                    cmd.Parameters.AddWithValue("@WinnerNumber", -1);
                    cmd.Parameters.AddWithValue("@Profit", 0);
                    con.Open();
                    int modified = (int)cmd.ExecuteScalar();
                    if (modified > 0)
                    {
                        var dto = new RouletteCreateResponseDTO()
                        {
                            Id = modified
                        };
                        return Ok(dto);
                    }
                    con.Close();
                }
            }
            return BadRequest();
        }

        // PUT api/<RoulettesController>/open/5/
        // [Route("open")]
        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> open(long id)
        {
            string constr = appSettings.Value.DefaultConnection;
            RouletteModel roulette = new RouletteModel();
            if (ModelState.IsValid)
            {
                string query = "UPDATE Roulette SET Status = 1, OpenDateTime = @OpenDateTime Where Id =@Id";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@OpenDateTime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Id", id);
                        con.Open();
                        int i = cmd.ExecuteNonQuery();
                        if (i > 0)
                        {
                            var dto = new GeneralResponseDTO()
                            {
                                Result = "OK"
                            };
                            return Ok(dto);
                        }
                        else
                        {
                            var dto = new GeneralResponseDTO()
                            {
                                Result = "Error"
                            };
                            return StatusCode(500);
                        }
                        con.Close();
                    }
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/<RoulettesController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, RouletteModel rouletteModel)
        {
            string constr = appSettings.Value.DefaultConnection;
            if (id != rouletteModel.Id)
            {
                return BadRequest();
            }
            RouletteModel roulette = new RouletteModel();
            if (ModelState.IsValid)
            {
                string query = "UPDATE Roulette SET Status = @Status, CreationDateTime = @CreationDateTime," +
                    "ClousureDateTime=@ClousureDateTime," +
                    "WinnerNumber=@WinnerNumber,Profit=@Profit, OpenDateTime=@OpenDateTime Where Id =@Id";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Status", rouletteModel.Status);
                        cmd.Parameters.AddWithValue("@CreationDateTime", rouletteModel.CreationDateTime);
                        cmd.Parameters.AddWithValue("@ClousureDateTime", rouletteModel.ClousureDateTime);
                        cmd.Parameters.AddWithValue("@OpenDateTime", rouletteModel.OpenDateTime);
                        cmd.Parameters.AddWithValue("@WinnerNumber", rouletteModel.WinnerNumber);
                        cmd.Parameters.AddWithValue("@Profit", rouletteModel.Profit);
                        cmd.Parameters.AddWithValue("@Id", rouletteModel.Id);
                        con.Open();
                        int i = cmd.ExecuteNonQuery();
                        if (i > 0)
                        {
                            var dto = new GeneralResponseDTO()
                            {
                                Result = "OK"
                            };
                            return Ok(dto);
                        }
                        else
                        {
                            var dto = new GeneralResponseDTO()
                            {
                                Result = "Error"
                            };
                            return StatusCode(500);
                        }
                        con.Close();
                    }
                }

            }
            return BadRequest(ModelState);
        }
        

        // DELETE api/<RoulettesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoulette(long id)
        {
            string constr = appSettings.Value.DefaultConnection;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "Delete FROM Roulette where Id='" + id + "'";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    int i = cmd.ExecuteNonQuery();
                    if (i > 0)
                    {
                        return NoContent();
                    }
                    con.Close();
                }
            }
            return BadRequest();
        }
    }
}
