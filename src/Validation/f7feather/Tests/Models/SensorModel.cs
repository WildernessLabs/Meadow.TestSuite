﻿using System;
using SQLite;

namespace F7Feather.Tests.Models
{
    [Table("SensorReadings")]
    public class SensorModel
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
    }
}