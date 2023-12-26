﻿using StaySphere.Domain.Common;

namespace StaySphere.Domain.Entities
{
    public class Booking : EntityBase
    {
        public int GuestId { get; set; }
        public int EmployeeId { get; set; }
        public int RoomId { get; set; }
        public DateTime Check_In_Data { get; set; }
        public DateTime Check_Out_Data { get; set; }
        public decimal Total_Price { get; set; }
        public Guest Guest { get; set; }
        public Employee Employee { get; set; }
        public Room Room { get; set; }
    }
}