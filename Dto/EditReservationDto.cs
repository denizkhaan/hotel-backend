using System;

namespace Hotel_reservation_app.Dto
{
    public class EditReservationDto
    {
        public int RoomId { get; set; }
        public int HotelId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
