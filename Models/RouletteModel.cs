namespace GamblingApp.Models
{
    public class RouletteModel
    {
        public int Id { get; set; }
        public bool Status { get; set; }
        public string CreationDateTime { get; set; }
        public string OpenDateTime { get; set; }
        public string ClousureDateTime { get; set; }
        public int WinnerNumber { get; set; }
        public double Profit { get; set; }
    }
}
