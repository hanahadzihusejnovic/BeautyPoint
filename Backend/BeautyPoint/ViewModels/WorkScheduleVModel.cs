using BeautyPoint.ViewModels;

namespace BeautyPoint.ViewModels
{
    public class WorkScheduleVModel
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
