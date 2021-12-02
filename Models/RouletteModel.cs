namespace GamblingApp.Models
{
    public class RouletteModel
    {
        public int Id { get; set; }
        public bool Status { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime ClousureDateTime { get; set; }
        public int WinnerNumber { get; set; }
        public double Profit { get; set; }
    }
}
