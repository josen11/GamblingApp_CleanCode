namespace GamblingApp.DTO
{
    public class CreateActionDTO
    {
        public bool BetType { get; set; } // False = Number True= Color
        public string Bet { get; set; }
        public double Handle { get; set; }
        public int RouletteId { get; set; }
    }
}
