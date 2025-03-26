using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace redis.WebAPi.Model
{

    public class BenchmarkRequestModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "pw is required.")]
        public string pw { get; set; }

        [Required(ErrorMessage = "Region is required.")]
        public string Region { get; set; }
        public string? Description { get; set; }

        [Required(ErrorMessage = "Clients is required.")]
        public int Clients { get; set; }

        [Required(ErrorMessage = "Threads is required.")]
        public int Threads { get; set; }

        [Required(ErrorMessage = "Size is required.")]
        public int Size { get; set; }

        [Required(ErrorMessage = "Requests is required.")]
        public int Requests { get; set; }

        [Required(ErrorMessage = "Pipeline is required.")]
        public int Pipeline { get; set; }
        [Required(ErrorMessage = "Times is required.")]
        public int Times { get; set; }

        public int Status { get; set; } // 1 is running, 2 is pending, 3 is successful, 4 is failed
        
        public DateTime TimeStamp { get; set; }
    }
}



