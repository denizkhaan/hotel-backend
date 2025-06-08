using Hotel_reservation_app.Dto;
using Hotel_reservation_app.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_reservation_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelController : ControllerBase
    {
        private readonly HotelContext _context;
        public HotelController(HotelContext context) => _context = context;

        [HttpPost("add-hotel")]
        public IActionResult AddHotel(Hotel hotel)
        {
            _context.Hotels.Add(hotel);
            _context.SaveChanges();
            return Ok("Hotel added");
        }

        // ➕ Plain Room Addition
        [HttpPost("add-room")]
        public IActionResult AddRoom(Room room)
        {
            _context.Rooms.Add(room);
            _context.SaveChanges();
            return Ok(new { id = room.Id });
        }

        // 🔐 Admin-Only User Edit
        [Authorize(Roles = "Admin")]
        [HttpPut("edit-user/{id}")]
        public IActionResult EditUser(int id, User updatedUser)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            user.Username = updatedUser.Username;
            user.Email = updatedUser.Email;
            user.PasswordHash = updatedUser.PasswordHash;
            user.Role = updatedUser.Role;

            _context.SaveChanges();
            return Ok("User updated");
        }

        // 📋 Hotel List with Filters
        [HttpGet("list")]
        public IActionResult ListHotels([FromQuery] string location, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] int? capacity)
        {
            var hotels = _context.Hotels.Include(h => h.Rooms).AsQueryable();

            if (!string.IsNullOrEmpty(location))
                hotels = hotels.Where(h => h.Location.Contains(location));

            if (minPrice.HasValue)
                hotels = hotels.Where(h => h.Rooms.Any(r => r.PricePerNight >= minPrice));

            if (maxPrice.HasValue)
                hotels = hotels.Where(h => h.Rooms.Any(r => r.PricePerNight <= maxPrice));

            if (capacity.HasValue)
                hotels = hotels.Where(h => h.Rooms.Any(r => r.Capacity >= capacity));

            return Ok(hotels.ToList());
        }

        // ℹ️ Room + Hotel Info
        [HttpGet("room-info")]
        public IActionResult GetRoomWithHotel([FromQuery] int roomId, [FromQuery] int hotelId)
        {
            var room = _context.Rooms
                .Include(r => r.Hotel)
                .FirstOrDefault(r => r.Id == roomId && r.HotelId == hotelId);

            if (room == null)
                return NotFound("Room or hotel not found.");

            var result = new RoomWithHotelDto
            {
                RoomType = room.RoomType,
                Price = room.PricePerNight,
                HotelName = room.Hotel.Name,
            };

            return Ok(result);
        }

        // ℹ️ Room by ID
        [HttpGet("room/{id}")]
        public async Task<IActionResult> GetRoomById(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
                return NotFound("Room not found.");

            return Ok(new
            {
                room.Id,
                room.RoomType,
                room.Capacity,
                room.PricePerNight,
                room.RoomName,
                room.Description,
                room.ImageUrl,
                Hotel = new
                {
                    room.Hotel.Id,
                    room.Hotel.Name,
                    room.Hotel.Location,
                    room.Hotel.Description
                }
            });
        }

        // ⛔ Hidden: Upload image to existing room
        [HttpPost("upload-room-image/{roomId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UploadRoomImage(
            int roomId,
            [FromForm] IFormFile imageFile,
            [FromForm] string roomName,
            [FromForm] string description)
        {
            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("No image provided.");

            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null)
                return NotFound("Room not found.");

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "room-images");
            Directory.CreateDirectory(folderPath);

            var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            room.RoomName = roomName;
            room.Description = description;
            room.ImageUrl = $"/room-images/{fileName}";

            await _context.SaveChangesAsync();

            return Ok(new { message = "✅ Uploaded successfully", room.ImageUrl });
        }

        // 🖼 Room Add + Image Upload in one request
        [HttpPost("add-room-with-image")]
        [Consumes("multipart/form-data")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> AddRoomWithImage(
            [FromForm] int hotelId,
            [FromForm] string roomType,
            [FromForm] int capacity,
            [FromForm] decimal pricePerNight,
            [FromForm] string roomName,
            [FromForm] string description,
            [FromForm] IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("No image provided.");

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "room-images");
            Directory.CreateDirectory(folderPath);

            var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            var newRoom = new Room
            {
                HotelId = hotelId,
                RoomType = roomType,
                Capacity = capacity,
                PricePerNight = pricePerNight,
                RoomName = roomName,
                Description = description,
                ImageUrl = $"/room-images/{fileName}"
            };

            _context.Rooms.Add(newRoom);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Room added with image", roomId = newRoom.Id });
        }
    }
}
