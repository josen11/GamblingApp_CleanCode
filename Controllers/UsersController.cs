using GamblingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GamblingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IOptions<ConnectionStrings> appSettings;
        public UsersController(IOptions<ConnectionStrings> app)
        {
            appSettings = app;
        }
        // GET: api/<UsersController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsersModel>>> Get()
        {
            string constr = appSettings.Value.DefaultConnection;
            List<UsersModel> users = new List<UsersModel>();
            string query = "SELECT * FROM Users";
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
                            users.Add(new UsersModel
                            {
                                UserId = Convert.ToString(sdr["UserId"]),
                                Password = Convert.ToString(sdr["Password"]),
                                Status = Convert.ToBoolean(sdr["Status"]),
                                Credit = Convert.ToDouble(sdr["Credit"])
                            });
                        }
                    }
                    con.Close();
                }
            }
            return users;
        }

        // GET api/<UsersController>/userId
        [HttpGet("{userId}")]
        public async Task<ActionResult<UsersModel>> Get(string userId)
        {
            string constr = appSettings.Value.DefaultConnection;
            UsersModel usersObj = new UsersModel();
            string query = "SELECT * FROM Users where UserId='" + userId+"'";
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
                            usersObj = new UsersModel
                            {
                                UserId = Convert.ToString(sdr["UserId"]),
                                Password = Convert.ToString(sdr["Password"]),
                                Status = Convert.ToBoolean(sdr["Status"]),
                                Credit = Convert.ToDouble(sdr["Credit"])
                            };
                        }
                    }
                    con.Close();
                }
            }
            if (usersObj == null)
            {
                return NotFound();
            }
            return usersObj;
        }

        // POST api/<UsersController>
        [HttpPost]
        public async Task<ActionResult<UsersModel>> Post(UsersModel usersModel)
        {
            string constr = appSettings.Value.DefaultConnection;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (SqlConnection con = new SqlConnection(constr))
            {
                //inserting Patient data into database
                string query = "insert into Users values (@UserId, @Password, @Status,@Credit)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@UserId", usersModel.UserId);
                    cmd.Parameters.AddWithValue("@Password", usersModel.Password);
                    cmd.Parameters.AddWithValue("@Status", usersModel.Status);
                    cmd.Parameters.AddWithValue("@Credit", usersModel.Credit);
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

        // PUT api/<UsersController>/UserId
        [HttpPut("{userId}")]
        public async Task<IActionResult> Put(string userId, UsersModel usersModel)
        {
            string constr = appSettings.Value.DefaultConnection;
            if (userId != usersModel.UserId)
            {
                return BadRequest();
            }
            UsersModel user = new UsersModel();
            if (ModelState.IsValid)
            {
                string query = "UPDATE Users SET Password = @Password, Status = @Status," +
                    "Credit=@Credit Where UserId =@UserId";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UserId", usersModel.UserId);
                        cmd.Parameters.AddWithValue("@Password", usersModel.Password);
                        cmd.Parameters.AddWithValue("@Status", usersModel.Status);
                        cmd.Parameters.AddWithValue("@Credit", usersModel.Credit);
                        con.Open();
                        int i = cmd.ExecuteNonQuery();
                        if (i > 0)
                        {
                            return NoContent();
                        }
                        con.Close();
                    }
                }

            }
            return BadRequest(ModelState);
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUsers(string userId)
        {
            string constr = appSettings.Value.DefaultConnection;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "Delete FROM Users where Id='" + userId + "'";
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
