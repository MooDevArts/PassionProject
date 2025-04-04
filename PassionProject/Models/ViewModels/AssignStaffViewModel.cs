namespace PassionProject.Models.ViewModels
{
    public class AssignStaffViewModel
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public List<StaffDto> AvailableStaffs { get; set; }

        public List<int> SelectedStaffIds { get; set; } = new List<int>();
    }
}
