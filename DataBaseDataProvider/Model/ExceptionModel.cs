
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBaseDataProvider.Model
{
    [Table("Exceptions")]
    public class ExceptionModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }

        public int? DataFrameModelId { get; set; }
        public DataFrameModel Frame { get; set; }

        public int? LaunchModelId { get; set; }
        public LaunchModel Launch { get; set; }
    }
}
