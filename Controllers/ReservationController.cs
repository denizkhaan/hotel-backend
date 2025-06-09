using Hotel_reservation_app.Model;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_reservation_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly HotelContext _context;
        public ReservationController(HotelContext context) => _context = context;

        [HttpPost("make")]
        public IActionResult MakeReservation([FromBody] Reservation res)
        {
            var user = _context.Users.Find(res.UserId);
            var hotel = _context.Hotels.Find(res.HotelId);
            var room = _context.Rooms.Find(res.RoomId);

            if (user == null || hotel == null || room == null)
                return BadRequest("Invalid userId, hotelId, or roomId.");

            // 🛠 Force UTC
            res.StartDate = DateTime.SpecifyKind(res.StartDate, DateTimeKind.Utc);
            res.EndDate = DateTime.SpecifyKind(res.EndDate, DateTimeKind.Utc);

            res.User = user;
            res.Hotel = hotel;
            res.Room = room;

            _context.Reservations.Add(res);
            _context.SaveChanges();

            return Ok("Reservation made.");
        }






        [HttpGet("user/{userId}")]
        public IActionResult GetReservationsByUser(int userId)
        {
            var reservations = _context.Reservations.Where(r => r.UserId == userId).ToList();
            return Ok(reservations);
        }
    }
}
