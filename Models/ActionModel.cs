namespace GamblingApp.Models
{
    public class ActionModel
    {
        public int Id { get; set; }
        public string CreationDateTime { get; set; }
        public bool BetType { get; set; } // false = Number true=Color
        public string Bet { get; set; }
        public double Handle { get; set; }
        public string UserId { get; set; }
        public int RouletteId { get; set; }
        public bool IsWinner { get; set; }
        
    }
}
