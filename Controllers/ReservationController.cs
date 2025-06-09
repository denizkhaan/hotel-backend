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

        [HttpPost("reservation/make")]
        public IActionResult MakeReservation([FromBody] Reservation reservation)
        {
            try
            {
                _context.Reservations.Add(reservation);
                _context.SaveChanges();
                return Ok("Reservation created");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpGet("user/{userId}")]
        public IActionResult GetReservationsByUser(int userId)
        {
            var reservations = _context.Reservations.Where(r => r.UserId == userId).ToList();
            return Ok(reservations);
        }
    }
}
