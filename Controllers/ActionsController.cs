﻿using GamblingApp.Models;
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
        public ActionsController(IOptions<ConnectionStrings> app)
        {
            appSettings = app;
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
                                CreationDateTime = Convert.ToDateTime(sdr["CreationDateTime"]),
                                BetType = Convert.ToBoolean(sdr["BetType"]),
                                Bet = Convert.ToString(sdr["Bet"]),
                                Handle = Convert.ToDouble(sdr["Handle"]),
                                UserId = Convert.ToString(sdr["UserId"]),
                                RouletteId = Convert.ToInt32(sdr["RouletteId"]),
                                IsWinner = Convert.ToBoolean(sdr["IsWinner"]),
                            };
                        }
                    }
                    con.Close();
                }
            }
            if (actionObj == null)
            {
                return NotFound();
            }
            return actionObj;
        }

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
                    con.Close();
                }
            }
            return BadRequest();
        }
    }
}