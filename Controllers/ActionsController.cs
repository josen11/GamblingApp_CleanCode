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
    public class ActionsController : ControllerBase
    {
        private readonly IOptions<ConnectionStrings> appSettings;
        private readonly ILogger<ActionsController> _logger;
        public ActionsController(IOptions<ConnectionStrings> app, ILogger<ActionsController> logger)
        {
            appSettings = app;
            _logger = logger;
        }
        // GET: api/<ActionsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActionModel>>> Get()
        {
            string constr = appSettings.Value.DefaultConnection;
            List<ActionModel> actions = new List<ActionModel>();
            string query = "SELECT * FROM Action";
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
                    _logger.LogInformation($"Get all actions (bets), retrieve {actions.Count} item(s)");
                    con.Close();
                }
            }
            return actions;
        }

        // GET api/<ActionsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ActionModel>> Get(int id)
        {
            string constr = appSettings.Value.DefaultConnection;
            ActionModel actionObj = new ActionModel();
            string query = "SELECT * FROM Action where Id=" + id;
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
                            actionObj = new ActionModel
                            {
                                Id = Convert.ToInt32(sdr["Id"]),
                                CreationDateTime = Formatter(Convert.ToDateTime(sdr["CreationDateTime"])),
                                BetType = Convert.ToBoolean(sdr["BetType"]),
                                Bet = Convert.ToString(sdr["Bet"]),
                                Handle = Convert.ToDouble(sdr["Handle"]),
                                UserId = Convert.ToString(sdr["UserId"]),
                                RouletteId = Convert.ToInt32(sdr["RouletteId"]),
                                IsWinner = Convert.ToBoolean(sdr["IsWinner"]),
                            };
                        }
                    }
                    _logger.LogInformation($"Get actions (bets) by id: Retrieve item with id  {id}");
                    con.Close();
                }
            }
            if (actionObj == null)
            {
                return NotFound();
                _logger.LogWarning($"Not found action id: {id} ");
            }
            return actionObj;
        }

        #region Validations and functions
        //Use this for ignoring functions in Swagger
        [ApiExplorerSettings(IgnoreApi = true)]
        //Get currect Roulettestatus and validate if roulette is open
        public bool validateStatusRoulette(int id)
        {
            string constr = appSettings.Value.DefaultConnection;
            RouletteStatusDTO rouletteObj = new RouletteStatusDTO();
            string query = "SELECT status FROM Roulette where Id=" + id;
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
                            rouletteObj = new RouletteStatusDTO
                            {
                                Status = Convert.ToBoolean(sdr["Status"])
                            };
                        }
                    }
                    con.Close();
                }
            }
            if (rouletteObj == null)
            {
                return false;
                _logger.LogWarning($"Not found roulette id: {id} ");
            }
            else
            {
                _logger.LogInformation($"Get current status of roulette id {id}: {rouletteObj.Status}");
                if (rouletteObj.Status)
                    return true;
                else
                    return false;
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        //Validate correct format or options of a bet
        public bool validateBet(bool type,string Bet)
        {
            //False=Number True=Color
            string finalbet = Bet.ToLower().Trim();
            if (type)
            {
                if (finalbet == "negro" || finalbet == "rojo")
                    return true;
                else
                    return false;
            }
            else
            {
                if (int.TryParse(finalbet, out int value))
                {
                    if (value >= 0 && value <= 36)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        //Validate user has enough credit based in bet handle
        public double validateCredit(string userId, double handle)
        {
            string constr = appSettings.Value.DefaultConnection;
            UsersCreditDTO userCredit = new UsersCreditDTO();
            string query = "SELECT Credit FROM Users where UserId='" + userId+"'";
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
                return 0;
                _logger.LogWarning($"Not found userId {userId}");
            }
            else
            {
                _logger.LogInformation($"Current userid {userId} credit: {userCredit.Credit}");
                return userCredit.Credit - handle;
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        //Update user credit after placing the bet
        public void updateCredit(string userId, double handle)
        {
            string constr = appSettings.Value.DefaultConnection;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "UPDATE Users SET Credit=@Credit where UserId='" + userId + "'";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Connection = con;
                    double finalcredit = validateCredit(userId: userId, handle: handle);
                    cmd.Parameters.AddWithValue("@Credit", finalcredit );
                    con.Open();
                    int i = cmd.ExecuteNonQuery();
                    _logger.LogInformation($"Updated userid {userId} credit (handle {handle.ToString()}):  {finalcredit}");
                    con.Close();
                }
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        //Update roulette profit after placing a bet
        public void updateProfit(int id, double handle)
        {
            string constr = appSettings.Value.DefaultConnection;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "UPDATE Roulette SET Profit=@Profit where Id=" + id;
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Connection = con;
                    double finalprofit = getRouletteProfit(id) + handle;
                    cmd.Parameters.AddWithValue("@Profit", finalprofit );
                    con.Open();
                    int i = cmd.ExecuteNonQuery();
                    _logger.LogInformation($"Updated roulette {id} profit (handle {handle.ToString()}): {finalprofit}  ");
                    con.Close();
                }
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        //Get current Roulette profit
        public double getRouletteProfit(int id)
        {
            string constr = appSettings.Value.DefaultConnection;
            RouletteProfitDTO rouletteObj = new RouletteProfitDTO();
            string query = "SELECT Profit FROM Roulette where Id=" + id;
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
                            rouletteObj = new RouletteProfitDTO
                            {
                                Profit = Convert.ToDouble(sdr["Profit"])
                            };
                        }
                    }
                    con.Close();
                }
            }
            if (rouletteObj == null)
            {
                return -1;
                _logger.LogWarning($"Not found roulette id {id} ");
            }
            else
            {
                _logger.LogInformation($"Current roulette {id} profit: {rouletteObj.Profit}");
                return rouletteObj.Profit;
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
                final= datetime.ToString("yyyy-MM-ddTHH:mm:ssZ");
            }
            if (localZone.DisplayName.Contains("(UTC-05:00)"))
            {
                final= datetime.ToString("yyyy-MM-ddTHH:mm:sszzz");
            }
            return final;
        }
        #endregion

        // POST api/<ActionsController>
        [HttpPost]
        public async Task<ActionResult<ActionModel>> Post(ActionModel ActionModel)
        {
            string constr = appSettings.Value.DefaultConnection;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (SqlConnection con = new SqlConnection(constr))
            {
                //inserting Patient data into database
                string query = "insert into Action values (@CreationDateTime, @BetType,@Bet,@Handle,@UserId,@RouletteId,@IsWinner)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@CreationDateTime", ActionModel.CreationDateTime);
                    cmd.Parameters.AddWithValue("@BetType", ActionModel.BetType);
                    cmd.Parameters.AddWithValue("@Bet", ActionModel.Bet);
                    cmd.Parameters.AddWithValue("@Handle", ActionModel.Handle);
                    cmd.Parameters.AddWithValue("@UserId", ActionModel.UserId);
                    cmd.Parameters.AddWithValue("@RouletteId", ActionModel.RouletteId);
                    cmd.Parameters.AddWithValue("@IsWinner", ActionModel.IsWinner);
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

        // POST api/<ActionsController>/create
        // Create a bet ("action" in gambling terms) based in requerements
        [HttpPost("[action]")]
        public async Task<ActionResult<ActionModel>> create([FromHeader] string userId, CreateActionDTO ActionModel)
        {
            string constr = appSettings.Value.DefaultConnection;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (validateStatusRoulette(ActionModel.RouletteId) && validateCredit(userId:userId, handle:ActionModel.Handle) >= 0 && validateBet(type:ActionModel.BetType, Bet: ActionModel.Bet)&&ActionModel.Handle<=10000)
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "insert into Action values (@CreationDateTime, @BetType,@Bet,@Handle,@UserId,@RouletteId,@IsWinner)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@CreationDateTime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@BetType", ActionModel.BetType);
                        cmd.Parameters.AddWithValue("@Bet", ActionModel.Bet);
                        cmd.Parameters.AddWithValue("@Handle", ActionModel.Handle);
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@RouletteId", ActionModel.RouletteId);
                        cmd.Parameters.AddWithValue("@IsWinner", false);
                        con.Open();
                        int i = cmd.ExecuteNonQuery();
                        if (i > 0)
                        {
                            _logger.LogInformation("Action(bet) created");
                            updateCredit(userId:userId, handle:ActionModel.Handle);
                            updateProfit(id:ActionModel.RouletteId, handle: ActionModel.Handle);
                            return Ok("OK. Bet created");
                        }
                        con.Close();
                    }
                }
            }
            else 
            {
                if (validateStatusRoulette(ActionModel.RouletteId) == false)
                {
                    _logger.LogError($"Error.Roulette {ActionModel.RouletteId} closed");
                    return BadRequest("Error. Roulette closed");
                }
                    
                if (validateCredit(userId: userId, handle: ActionModel.Handle) < 0)
                {
                    _logger.LogError($"Error. User id {userId} Without enough credit");
                    return BadRequest("Error. Without enough credit");
                }
                    
                if (validateBet(type: ActionModel.BetType, Bet: ActionModel.Bet) == false)
                {
                    _logger.LogError($"Error. Incorrect bet format");
                    return BadRequest("Error. Incorrect bet format");
                }
                   
                if (ActionModel.Handle > 10000)
                {
                    _logger.LogError($"Error. Maximum handle exceded (> 10K)");
                    return BadRequest("Error. Maximum handle exceded");
                }
                    
            }
            return BadRequest();
        }

        // PUT api/<ActionsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, ActionModel actionModel)
        {
            string constr = appSettings.Value.DefaultConnection;
            if (id != actionModel.Id)
            {
                return BadRequest();
            }
            ActionModel Action = new ActionModel();
            if (ModelState.IsValid)
            {
                string query = "UPDATE Action SET CreationDateTime = @CreationDateTime," +
                    "BetType=@BetType," +
                    "Bet=@Bet,Handle=@Handle,UserId=@UserId,RouletteId=@RouletteId,IsWinner=@IsWinner Where Id =@Id";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@CreationDateTime", actionModel.CreationDateTime);
                        cmd.Parameters.AddWithValue("@BetType", actionModel.BetType);
                        cmd.Parameters.AddWithValue("@Bet", actionModel.Bet);
                        cmd.Parameters.AddWithValue("@Handle", actionModel.Handle);
                        cmd.Parameters.AddWithValue("@UserId", actionModel.UserId);
                        cmd.Parameters.AddWithValue("@RouletteId", actionModel.RouletteId);
                        cmd.Parameters.AddWithValue("@IsWinner", actionModel.IsWinner);
                        cmd.Parameters.AddWithValue("@Id", actionModel.Id);
                        con.Open();
                        int i = cmd.ExecuteNonQuery();
                        if (i > 0)
                        {
                            return NoContent();
                        }
                        _logger.LogInformation($"Action {actionModel.Id} updated");
                        con.Close();
                    }
                }

            }
            return BadRequest(ModelState);
        }

        // DELETE api/<ActionsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAction(long id)
        {
            string constr = appSettings.Value.DefaultConnection;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "Delete FROM Action where Id='" + id + "'";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    int i = cmd.ExecuteNonQuery();
                    if (i > 0)
                    {
                        return NoContent();
                    }
                    _logger.LogInformation($"Action {id} deleted");
                    con.Close();
                }
            }
            return BadRequest();
        }
    }
}
