using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodenjoyBot.Entity
{
    [Table("LaunchSettings")]
    public class LaunchSettingsModel
    {
        public int Id { get; set; }

        public byte[] Data { get; set; }

        public bool Visibility { get; set; }

        public DateTime CreateTime { get; set; }

        public int HashCode { get; set; }

        public ICollection<LaunchModel> Launches { get; set; }
    }
}