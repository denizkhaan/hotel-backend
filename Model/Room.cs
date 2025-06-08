using Hotel_reservation_app.Model;

public class Room
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public Hotel Hotel { get; set; }

    public string RoomType { get; set; }
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }

    public string RoomName { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }

    public ICollection<Reservation> Reservations { get; set; }
}
