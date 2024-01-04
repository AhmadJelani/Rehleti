namespace Rehleti.Models
{
    public class FeedbackTripJoinTable
    {
        public UserGuest user { get; set; }
        public TripFeedback feedback { get; set; }
        public AdventureTrip trip { get; set; }
    }
}
