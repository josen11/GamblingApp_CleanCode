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
        private readonly ILogger<ActionsController> _logger;

        public RoulettesController(IOptions<ConnectionStrings> app, ILogger<ActionsController> logger)
        {
            appSettings = app;
            _logger = logger;
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
        public async Task<IActionResult> open(int id)
        {
            string constr = appSettings.Value.DefaultConnection;
            RouletteModel roulette = new RouletteModel();
            if (ModelState.IsValid)
            {
                string query = "UPDATE Roulette SET Status = 1, OpenDateTime = @OpenDateTime Where Id=" +id;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@OpenDateTime", DateTime.Now);
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

        [HttpPut("[action]/{id}")]
        public async Task<ActionResult> close(int id)
        {
            int WinnerNumber = new Random().Next(0, 37);
            setWinners(getActionsbyRolette(id), WinnerNumber);
            string constr = appSettings.Value.DefaultConnection;
            RouletteModel roulette = new RouletteModel();
            List<ActionModel> actions = new List<ActionModel>();
            if (ModelState.IsValid)
            {
                string query = "UPDATE Roulette SET Status = 0, ClousureDateTime = @ClousureDateTime,WinnerNumber="+WinnerNumber+" Where Id =" + id;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ClousureDateTime", DateTime.Now);
                        con.Open();
                        int i = cmd.ExecuteNonQuery();
                        if (i > 0)
                        {
                            actions = getActionsbyRolette(id);
                            return Ok(actions);
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
        [ApiExplorerSettings(IgnoreApi = true)]
        public void setWinners(List<ActionModel> actionsByRoulette, int winnerNumber)
        {
            List<ActionModel> actions = actionsByRoulette;
            List<int> WinnersId = new List<int>();
            string winnerColor = getColor(winnerNumber);
            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i].Bet == winnerNumber.ToString())
                {
                    WinnersId.Add(actions[i].Id);
                    updateCreditPrize(actions[i].UserId, actions[i].Handle, '0');
                    updateProfitPrize(actions[i].RouletteId, getPrizeWinner(actions[i].Handle, '0'));
                }
                if (actions[i].Bet == winnerColor)
                {
                    WinnersId.Add(actions[i].Id);
                    updateCreditPrize(actions[i].UserId, actions[i].Handle, '1');
                    updateProfitPrize(actions[i].RouletteId, getPrizeWinner(actions[i].Handle, '1'));
                }
            }
            if (WinnersId.Count > 0)
            {
                string constr = appSettings.Value.DefaultConnection;
                if (ModelState.IsValid)
                {
                    string ids = string.Join(", ", WinnersId);
                    string query = "UPDATE Action SET IsWinner = 1 WHERE Id IN (" + ids + ")";
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        using (SqlCommand cmd = new SqlCommand(query))
                        {
                            cmd.Connection = con;
                            con.Open();
                            int i = cmd.ExecuteNonQuery();
                            if (i > 0)
                            {
                                _logger.LogInformation("IsWinner updated");
                            }
                            else
                            {
                                _logger.LogInformation("Error when updating IsWinner");
                            }
                            con.Close();
                        }
                    }
                }
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public List<ActionModel> getActionsbyRolette(int idRoulette)
        {
            string constr = appSettings.Value.DefaultConnection;
            List<ActionModel> actions = new List<ActionModel>();
            string query = "SELECT * FROM Action where RouletteId="+idRoulette;
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
                            actions.Add(new ActionModel
                            {
                                Id = Convert.ToInt32(sdr["Id"]),
                                CreationDateTime = Convert.ToDateTime(sdr["CreationDateTime"]),
                                BetType = Convert.ToBoolean(sdr["BetType"]),
                                Bet = Convert.ToString(sdr["Bet"]),
                                Handle = Convert.ToDouble(sdr["Handle"]),
                                UserId = Convert.ToString(sdr["UserId"]),
                                RouletteId = Convert.ToInt32(sdr["RouletteId"]),
                                IsWinner = Convert.ToBoolean(sdr["IsWinner"]),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return actions;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public string getColor(int winnerNumber)
        {
            if (winnerNumber == 0 || winnerNumber % 2 == 0)
                return "rojo";
            else
                return "negro";
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public double getPrizeWinner(double handle,char typeBeatWon)
        {
            if (typeBeatWon == '0')
            {
                return handle * 5;
            }
            else
            {
                return handle * 1.8;
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public double getCredit(string userId)
        {
            string constr = appSettings.Value.DefaultConnection;
            UsersCreditDTO userCredit = new UsersCreditDTO();
            string query = "SELECT Credit FROM Users where UserId='" + userId + "'";
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
                            userCredit = new UsersCreditDTO
                            {
                                Credit = Convert.ToDouble(sdr["Credit"])
                            };
                        }
                    }
                    con.Close();
                }
            }
            if (userCredit == null)
            {
                return -1;
            }
            else
            {
                return userCredit.Credit;
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public double getProfit(int Id)
        {
            string constr = appSettings.Value.DefaultConnection;
            RouletteProfitDTO rouletteProfit = new RouletteProfitDTO();
            string query = "SELECT Profit FROM Roulette where Id=" + Id;
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
                            rouletteProfit = new RouletteProfitDTO
                            {
                                Profit = Convert.ToDouble(sdr["Profit"])
                            };
                        }
                    }
                    con.Close();
                }
            }
            if (rouletteProfit == null)
            {
                return -1;
            }
            else
            {
                return rouletteProfit.Profit;
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public void updateCreditPrize(string userId, double handle, char typeBetWon)
        {
            double currentCredit = getCredit(userId);
            if (currentCredit >= 0)
            {
                string constr = appSettings.Value.DefaultConnection;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "UPDATE Users SET Credit=@Credit where UserId='" + userId + "'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Credit", currentCredit+getPrizeWinner(handle, typeBetWon));
                        con.Open();
                        int i = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public void updateProfitPrize(int id, double prize)
        {
            double currentProfit = getProfit(id);
            if (currentProfit >= 0)
            {
                string constr = appSettings.Value.DefaultConnection;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "UPDATE Roulette SET Profit=@Profit where Id=" + id;
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Profit", currentProfit - prize);
                        con.Open();
                        int i = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }       
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public void updateWinnerNumber(int idRoulette, int winnerNumber)
        {
            string constr = appSettings.Value.DefaultConnection;
            RouletteModel roulette = new RouletteModel();
            List<ActionModel> actions = new List<ActionModel>();
            if (ModelState.IsValid)
            {
                string query = "UPDATE Rulette SET WinnerNumber = " + winnerNumber + " Where Id =" + idRoulette;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        int i = cmd.ExecuteNonQuery();
                        if (i > 0)
                        {
                            _logger.LogInformation("Winner number updated");
                        }
                        else
                        {
                            _logger.LogInformation("Error when updating Winner number");
                        }
                        con.Close();
                    }
                }
            }
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
