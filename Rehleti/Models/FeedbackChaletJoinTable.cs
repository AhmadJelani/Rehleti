namespace Rehleti.Models
{
    public class FeedbackChaletJoinTable
    {
        public UserGuest user { get; set; }
        public Chalet chalet { get; set; }
        public ChaletFeedback feedback { get; set; }
    }
}
