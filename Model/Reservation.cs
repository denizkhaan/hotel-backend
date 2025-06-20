﻿using Hotel_reservation_app.Model;

public class Reservation
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int HotelId { get; set; }
    public Hotel? Hotel { get; set; }

    public int RoomId { get; set; }
    public Room? Room { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
