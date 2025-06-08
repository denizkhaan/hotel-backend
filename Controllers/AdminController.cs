using Hotel_reservation_app.Dto;
using Hotel_reservation_app.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_reservation_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")] 
    public class AdminController : ControllerBase
    {
        private readonly HotelContext _context;

        public AdminController(HotelContext context)
        {
            _context = context;
        }

        [HttpPut("edit-user/{id}")]
        public IActionResult EditUser(int id, [FromBody] EditUserDto dto)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound("User not found.");

            user.Username = dto.Username;
            user.Email = dto.Email;
            user.PasswordHash = dto.PasswordHash;
            user.Role = dto.Role;

            _context.SaveChanges();
            return Ok("User updated successfully.");
        }

        [HttpPut("edit-reservation/{id}")]
        public IActionResult EditReservation(int id, [FromBody] EditReservationDto dto)
        {
            var reservation = _context.Reservations.Find(id);
            if (reservation == null)
                return NotFound("Reservation not found.");

            reservation.StartDate = dto.StartDate;
            reservation.EndDate = dto.EndDate;
            reservation.RoomId = dto.RoomId;
            reservation.HotelId = dto.HotelId;

            _context.SaveChanges();
            return Ok("Reservation updated successfully.");
        }
  
        [HttpDelete("delete-user/{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound("User not found.");

          
            var userReservations = _context.Reservations.Where(r => r.UserId == id).ToList();
            _context.Reservations.RemoveRange(userReservations);

            _context.Users.Remove(user);
            _context.SaveChanges();

            return Ok("User and their reservations deleted successfully.");
        }

      
        [HttpDelete("delete-reservation/{id}")]
        public IActionResult DeleteReservation(int id)
        {
            var reservation = _context.Reservations.Find(id);
            if (reservation == null)
                return NotFound("Reservation not found.");

            _context.Reservations.Remove(reservation);
            _context.SaveChanges();

            return Ok("Reservation deleted successfully.");
        }
        [HttpGet("all-users")]
        public IActionResult GetAllUsers()
        {
            var users = _context.Users.Select(u => new
            {
                u.Id,
                u.Username,
                u.Email,
                u.Role,
                u.PasswordHash
            }).ToList();

            return Ok(users);
        }
        [HttpGet("all")]
        public IActionResult GetAllReservations()
        {
            var reservations = _context.Reservations.Select(r => new
            {
                r.Id,
                r.UserId,
                r.HotelId,
                r.RoomId,
                r.StartDate,
                r.EndDate
            }).ToList();

            return Ok(reservations);
        }
        [HttpGet("search-users")]
        public IActionResult SearchUsers([FromQuery] string username)
        {
            var users = _context.Users
                .Where(u => u.Username.Contains(username))
                .Select(u => new {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.Role
                })
                .ToList();

            return Ok(users);
        }


    }
}
