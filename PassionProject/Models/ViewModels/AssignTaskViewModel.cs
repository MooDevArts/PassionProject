namespace PassionProject.Models.ViewModels
{
    public class AssignTaskViewModel
    {
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public List<WorkTaskDto> AvailableTasks { get; set; } = new List<WorkTaskDto>();
    }
}
