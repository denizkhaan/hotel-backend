namespace Hotel_reservation_app.Dto
{
    public class RoomWithHotelDto
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string RoomType { get; set; }
        public decimal Price { get; set; }

        public int HotelId { get; set; }
        public string HotelName { get; set; }
        public string HotelCity { get; set; }
    }

}
