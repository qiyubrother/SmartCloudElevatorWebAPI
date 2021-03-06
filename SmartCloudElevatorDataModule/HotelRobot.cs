﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCloudElevatorDataModel
{
    [Table("HotelRobot")]
    public class HotelRobot
    {
        /// <summary>
        /// HotelID
        /// </summary>
        [Required]
        public string HotelId { get; set; }
        /// <summary>
        /// Unique RobotSN
        /// </summary>
        [Required]
        public string UniqueRobotSN { get; set; }
    }
}
