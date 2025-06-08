namespace Hotel_reservation_app.Model
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
