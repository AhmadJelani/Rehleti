namespace Rehleti.Models
{
    public class ChaletReport
    {
        public Chalet? chalet { get; set; }
        public BookChalet? booking { get; set; }
        public UserGuest? user { get; set; }
        public ListOfDatesForChalet? dates { get; set; }
    }
}
