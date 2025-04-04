using System.ComponentModel.DataAnnotations;

namespace PassionProject.Models
{
    public class WorkTask
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string TaskName { get; set; }

        public string Description { get; set; }

        public ICollection<Staff> Staffs { get; set; } = new List<Staff>();
    }

    public class WorkTaskDto
    {
        public int id { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public List<StaffDto> Staffs { get; set; } = new List<StaffDto>();
        public string Error { get; set; }
    }
}