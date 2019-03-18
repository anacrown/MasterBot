﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodenjoyBot.DataProvider.DataBaseModel
{
    public class LaunchModel
    {
        public int Id { get; set; }
        public DateTime LaunchTime { get; set; }
        public string BotInstanceName { get; set; }
        
        public ICollection<DataFrameModel> Frames { get; set; }
        public ICollection<ExceptionModel> Exceptions { get; set; }
    }
}