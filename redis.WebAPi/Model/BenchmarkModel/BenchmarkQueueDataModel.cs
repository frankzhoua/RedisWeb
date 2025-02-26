using Azure.Core;
using OfficeOpenXml.Drawing.Chart;
using System.ComponentModel.DataAnnotations;

namespace redis.WebAPi.Model.BenchmarkModel
{
    public class BenchmarkQueueDataModel
    {
        public BenchmarkQueueDataModel() { }
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Name is required.")]
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


        public BenchmarkQueueDataModel(BenchmarkRequestModel request)
        {
            Id = request.Id;
            Name = request.Name;
            pw = request.pw;
            Region = request.Region;
            Description = request.Description;
            Clients = request.Clients;
            Threads = request.Threads;
            Size = request.Size;
            Requests = request.Requests;
            Pipeline = request.Pipeline;
            Times = request.Times;
            Status = request.Status;
            TimeStamp = request.TimeStamp;
        }

        public BenchmarkRequestModel ToBenchmarkRequestModel()
        {
            return new BenchmarkRequestModel
            {
                Id = Id,
                Name = Name,
                pw = pw,
                Region = Region,
                Description = Description,
                Clients = Clients,
                Threads = Threads,
                Size = Size,
                Requests = Requests,
                Pipeline = Pipeline,
                Times = Times,
                Status = Status,
                TimeStamp = TimeStamp
            };
        }

    }



}
