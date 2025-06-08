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
            _context.Attach(new User { Id = res.UserId });
            _context.Attach(new Hotel { Id = res.HotelId });
            _context.Attach(new Room { Id = res.RoomId });

            _context.Reservations.Add(res);
            _context.SaveChanges();

            return Ok("Reservation made");
        }


        [HttpGet("user/{userId}")]
        public IActionResult GetReservationsByUser(int userId)
        {
            var reservations = _context.Reservations.Where(r => r.UserId == userId).ToList();
            return Ok(reservations);
        }
    }
}
