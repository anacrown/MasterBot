using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBaseDataProvider.Model
{
    [Table("Launches")]
    public class LaunchModel
    {
        public int Id { get; set; }
        public DateTime LaunchTime { get; set; }
        public string BotInstanceName { get; set; }
        public string BotInstanceTitle { get; set; }

        public int? LaunchSettingsModelId { get; set; }
        public LaunchSettingsModel LaunchSettingsModel { get; set; }
        
        public ICollection<DataFrameModel> Frames { get; set; }
        public ICollection<ExceptionModel> Exceptions { get; set; }
    }
}