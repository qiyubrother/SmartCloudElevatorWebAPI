using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SmartCloudElevatorDataModel
{
    [Table("User")]
    public class User
    {
        [Required, Key]
        public string UserId { get; set; }
        public string Pwd { get; set; }
        [Required]
        public string Name { get; set; }
        public string HotelId { get; set; }
        public string Mobile { get; set; }
        public string RoleId { get; set; }
        public string Status { get; set; }
    }
}
