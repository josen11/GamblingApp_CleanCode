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
                                CreationDateTime = Formatter(Convert.ToDateTime(sdr["CreationDateTime"])),
                                OpenDateTime = Formatter(Convert.ToDateTime(sdr["OpenDateTime"])),
                                ClousureDateTime = Formatter(Convert.ToDateTime(sdr["ClousureDateTime"])),
                                WinnerNumber = Convert.ToInt32(sdr["WinnerNumber"]),
                                Profit = Convert.ToDouble(sdr["Profit"])
                            });
                        }
                    }
                    _logger.LogInformation($"Get all roulettes. Retrieved {roulettes.Count} item(s)");
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
                                CreationDateTime = Formatter(Convert.ToDateTime(sdr["CreationDateTime"])),
                                OpenDateTime = Formatter(Convert.ToDateTime(sdr["OpenDateTime"])),
                                ClousureDateTime = Formatter(Convert.ToDateTime(sdr["ClousureDateTime"])),
                                WinnerNumber = Convert.ToInt32(sdr["WinnerNumber"]),
                                Profit = Convert.ToDouble(sdr["Profit"])
                            };
                        }
                    }
                    _logger.LogInformation($"Get roulette id {id}");
                    con.Close();
                }
            }
            if (rouletteObj == null)
            {
                _logger.LogWarning($"Not found id  {id} ");
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
        // Create a roulette based in requerimients
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
                        _logger.LogInformation($"New roulette created: Id {modified}");
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
        // Open a roulette based in requerimients
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
                            _logger.LogInformation($"Roulette id {id} opened");
                            var dto = new GeneralResponseDTO()
                            {
                                Result = "OK"
                            };
                            return Ok(dto);
                        }
                        else
                        {
                            _logger.LogError($"Error when roulette id {id} was trying to open");
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
        // PUT api/<RoulettesController>/close/5/
        // Close a roulette based in requerimients and considering the roulette profit and winner users credit 
        
        [HttpPut("[action]/{id}")]
        public async Task<ActionResult> close(int id)
        {
            int WinnerNumber = new Random().Next(0, 37);
            _logger.LogInformation($"Winner Number for Roulette {id}:  {WinnerNumber}");
            setWinners(actionsByRoulette:getActionsbyRolette(id), winnerNumber:WinnerNumber);
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
                        con.Close();
                        if (i > 0)
                        {
                            actions = getActionsbyRolette(id);
                            _logger.LogInformation($"Roulette id {id} closed. Total done actions {actions.Count}");
                            return Ok(actions);
                        }
                        else
                        {
                            _logger.LogError($"Error when roulette id {id} was trying to close");
                            var dto = new GeneralResponseDTO()
                            {
                                Result = "Error"
                            };
                            return StatusCode(500);
                        }
                    }
                }
            }
            return BadRequest(ModelState);
        }
        #region Validations and functions
        [ApiExplorerSettings(IgnoreApi = true)]
        // Get winner bets and update roulette profit and winner users credit 
        public void setWinners(List<ActionModel> actionsByRoulette, int winnerNumber)
        {
            List<ActionModel> actions = actionsByRoulette;
            List<int> WinnersId = new List<int>();
            string winnerColor = getWinnerColor(winnerNumber);
            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i].Bet == winnerNumber.ToString())
                {
                    WinnersId.Add(actions[i].Id);
                    updateCreditbyPrize(userId:actions[i].UserId, handle:actions[i].Handle,typeBet: '0');
                    _logger.LogInformation($"Number bet Winner: Paid prize to Userid {actions[i].UserId}");
                    updateProfitbyPrize(id:actions[i].RouletteId, prize:getWinnerPrize(actions[i].Handle, typeBet: '0'));
                    _logger.LogInformation($"Roulette {actions[i].RouletteId} profit updated after pay prize");
                }
                if (actions[i].Bet == winnerColor)
                {
                    WinnersId.Add(actions[i].Id);
                    updateCreditbyPrize(userId:actions[i].UserId, handle:actions[i].Handle, typeBet:'1');
                    _logger.LogInformation($"Color bet Winner: Paid prize to Userid {actions[i].UserId}");
                    updateProfitbyPrize(id:actions[i].RouletteId, prize:getWinnerPrize(actions[i].Handle, typeBet: '1'));
                    _logger.LogInformation($"Roulette {actions[i].RouletteId} profit updated after pay prize");
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
                                _logger.LogError("Error when updating IsWinner");
                            }
                            con.Close();
                        }
                    }
                }
            }
        }
        
        [ApiExplorerSettings(IgnoreApi = true)]
        // Get list of actions (bets) by roulette 
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
                                CreationDateTime = Formatter(Convert.ToDateTime(sdr["CreationDateTime"])),
                                BetType = Convert.ToBoolean(sdr["BetType"]),
                                Bet = Convert.ToString(sdr["Bet"]),
                                Handle = Convert.ToDouble(sdr["Handle"]),
                                UserId = Convert.ToString(sdr["UserId"]),
                                RouletteId = Convert.ToInt32(sdr["RouletteId"]),
                                IsWinner = Convert.ToBoolean(sdr["IsWinner"]),
                            });
                        }
                    }
                    _logger.LogInformation($"Get actions by roulette {idRoulette}: Retrieved items {actions.Count}");
                    con.Close();
                }
            }
            return actions;
        }
        
        [ApiExplorerSettings(IgnoreApi = true)]
        // Get winner color based in requeriments 
        public string getWinnerColor(int winnerNumber)
        {
            if (winnerNumber == 0 || winnerNumber % 2 == 0)
                return "rojo";
            else
                return "negro";
        }
        
        [ApiExplorerSettings(IgnoreApi = true)]
        // Calculate prize based in requeriments (color 1.8X and number 5X) 
        public double getWinnerPrize(double handle,char typeBet)
        {
            if (typeBet == '0')
            {
                return handle * 5;
            }
            else
            {
                return handle * 1.8;
            }
        }
        
        [ApiExplorerSettings(IgnoreApi = true)]
        // Get current user credit 
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
                _logger.LogWarning($"Not found userId {userId}");
                return -1;
            }
            else
            {
                _logger.LogInformation($"Userid {userId} credit retrieved: {userCredit.Credit}");
                return userCredit.Credit;
            }
        }
        
        [ApiExplorerSettings(IgnoreApi = true)]
        // Get current roulette profit 
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
                _logger.LogWarning($"Not found roulette id {Id}");
                return -1;
            }
            else
            {
                _logger.LogInformation($"Roulette id {Id} profit retrieved: {rouletteProfit.Profit}");
                return rouletteProfit.Profit;
            }
        }
        
        [ApiExplorerSettings(IgnoreApi = true)]
        // Update user credit by prize
        public void updateCreditbyPrize(string userId, double handle, char typeBet)
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
                        cmd.Parameters.AddWithValue("@Credit", currentCredit+getWinnerPrize(handle:handle, typeBet:typeBet));
                        con.Open();
                        int i = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
        }
        
        [ApiExplorerSettings(IgnoreApi = true)]
        // Update roulette profit by prize
        public void updateProfitbyPrize(int id, double prize)
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
        //Datetime formatter based in requeriments
        public string Formatter(DateTime datetime)
        {
            string final = "";
            TimeZoneInfo localZone = TimeZoneInfo.Local;
            if (localZone.DisplayName.Contains("(UTC)"))
            {
                final = datetime.ToString("yyyy-MM-ddTHH:mm:ssZ");
            }
            if (localZone.DisplayName.Contains("(UTC-05:00)"))
            {
                final = datetime.ToString("yyyy-MM-ddTHH:mm:sszzz");
            }
            return final;
        }
        #endregion

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
                        con.Close();
                        if (i > 0)
                        {
                            _logger.LogWarning($"Roulette {id} updated");
                            var dto = new GeneralResponseDTO()
                            {
                                Result = "OK"
                            };
                            return Ok(dto);
                        }
                        else
                        {
                            _logger.LogError($"Error when roulette id {id} was trying to update");
                            var dto = new GeneralResponseDTO()
                            {
                                Result = "Error"
                            };
                            return StatusCode(500);
                        }
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
                        _logger.LogWarning($"Roulette {id} deleted");
                        return NoContent();
                    }
                    con.Close();
                }
            }
            return BadRequest();
        }
    }
}
