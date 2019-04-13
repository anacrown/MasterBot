
using System.ComponentModel.DataAnnotations.Schema;

namespace CodenjoyBot.Entity
{
    [Table("Exceptions")]
    public class ExceptionModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }

        public int? LaunchModelId { get; set; }
        public LaunchModel LaunchModel { get; set; }
    }
}
