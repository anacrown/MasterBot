﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBaseDataProvider.Model
{
    [Table("DataFrames")]
    public class DataFrameModel
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public uint FrameNumber { get; set; }
        public string Board { get; set; }
        public string Response { get; set; }

        public bool IsDead { get; set; }
        
        public int? LaunchModelId { get; set; }
        public LaunchModel LaunchModel { get; set; }
    }
}
