﻿namespace GamblingApp.Models
{
    public class ActionModel
    {
        public int Id { get; set; }
        public DateTime CreationDateTime { get; set; }
        public bool BetType { get; set; } // 0 = Number 1=Color
        public string Bet { get; set; }
        public double Handle { get; set; }
        public string UserId { get; set; }
        public int RouletteId { get; set; }
        public bool IsWinner { get; set; }
        
    }
}