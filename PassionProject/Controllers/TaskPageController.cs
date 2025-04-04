using Microsoft.AspNetCore.Mvc;
using PassionProject.Interface;
using PassionProject.Models.ViewModels;
using PassionProject.Models;
using PassionProject.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PassionProject.Controllers
{
    public class TaskPageController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly IStaffService _staffService;
        private readonly ApplicationDbContext _context;

        public TaskPageController(
            ApplicationDbContext context,
            ITaskService taskService,
            IStaffService staffService)
        {
            _taskService = taskService;
            _staffService = staffService;
            _context = context;
        }

        /// <summary>
        /// Retrieves a list of all tasks.
        /// </summary>
        /// <returns>Task list view</returns>
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var tasks = await _context.WorkTasks
                .Select(t => new WorkTaskDto
                {
                    id = t.id,
                    TaskName = t.TaskName,
                    Description = t.Description
                })
                .ToListAsync();

            var assignedStaffs = await _context.WorkTasks
                .Include(t => t.Staffs)
                .ToDictionaryAsync(
                    t => t.id,
                    t => t.Staffs.Select(s => $"{s.FirstName} {s.LastName}").ToList()
                );

            ViewBag.AssignedStaffs = assignedStaffs;

            return View(tasks);
        }
        /// <summary>
        /// Retrieves details of a specific task by ID.
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <returns>Task details view</returns>
       	[HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var task = await _context.WorkTasks.FindAsync(id);
            if (task == null) return NotFound();

            ViewBag.AvailableStaffs = await _context.Staffs
                .Where(s => !s.WorkTasks.Any(t => t.id == id))
                .Select(s => new SelectListItem
                {
                    Value = s.StaffId.ToString(),
                    Text = $"{s.FirstName} {s.LastName}"
                })
                .ToListAsync();

            ViewBag.AssignedStaffs = await _context.Staffs
                .Where(s => s.WorkTasks.Any(t => t.id == id))
                .Select(s => new StaffDto
                {
                    StaffId = s.StaffId,
                    FirstName = s.FirstName,
                    LastName = s.LastName
                })
                .ToListAsync();

            return View(new WorkTaskDto
            {
                id = task.id,
                TaskName = task.TaskName,
                Description = task.Description
            });
        }
        /// <summary>
        /// Loads the form to create a new task.
        /// </summary>
        /// <returns>Create task form view</returns>
        [HttpGet]
        public async Task<IActionResult> New()
        {
            return View(new WorkTaskDto());
        }

		/// <summary>
		/// Creates a new task.
		/// </summary>
		/// <param name="taskDto">Task data</param>
		/// <returns>Redirect to list or error view</returns>
		[HttpPost]
		public async Task<IActionResult> Create(WorkTaskDto taskDto)
		{
			var response = await _taskService.CreateTask(taskDto);
			if (response.Status == ServiceResponse.ServiceStatus.Created)
			{
				return RedirectToAction("List");
			}
			return View("Error", new ErrorViewModel { Errors = response.Messages });
		}

		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			var task = await _context.WorkTasks
				.Where(t => t.id == id)
				.Select(t => new WorkTaskDto
				{
					id = t.id,
					TaskName = t.TaskName,
					Description = t.Description
				})
				.FirstOrDefaultAsync();

			if (task == null) return NotFound();
			return View(task);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(int id, WorkTaskDto taskDto)
		{
			var response = await _taskService.UpdateTask(id, taskDto);
			if (response.Status == ServiceResponse.ServiceStatus.Updated)
			{
				return RedirectToAction("Details", new { id });
			}
			return View("Error", new ErrorViewModel { Errors = response.Messages });
		}

		/// <summary>
		/// Loads the confirmation to delete a task.
		/// </summary>
		/// <param name="id">Task ID</param>
		/// <returns>Delete confirmation view</returns>
		[HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            WorkTaskDto? task = await _taskService.GetTask(id);
            if (task == null)
            {
                return View("Error", new ErrorViewModel { Errors = new List<string> { "Task not found" } });
            }
            return View(task);
        }

        /// <summary>
        /// Deletes a task.
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <returns>Redirect to list or error view</returns>
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ServiceResponse response = await _taskService.DeleteTask(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List");
            }

            return View("Error", new ErrorViewModel { Errors = response.Messages });
        }

        /// <summary>
        /// Retrieves a list of staff assigned to a specific task.
        /// </summary>
        /// <param name="taskId">Task ID</param>
        /// <returns>Task staff list view</returns>
        [HttpGet]
        public async Task<IActionResult> TaskStaff(int taskId)
        {
            var task = await _taskService.GetTask(taskId);
            if (task == null)
            {
                return View("Error", new ErrorViewModel { Errors = new List<string> { "Task not found" } });
            }

            var staffs = await _taskService.ListStaffForWorkTask(taskId);

            var viewModel = new WorkTaskDetails
            {
                Task = task,
                Staffs = staffs.ToList()
            };

            return View(viewModel);
        }

        /// <summary>
        /// Loads the form to assign a staff member to a task.
        /// </summary>
        /// <param name="taskId">Task ID</param>
        /// <returns>Assign staff form view</returns>
        [HttpGet]
        public async Task<IActionResult> AssignStaff(int taskId)
        {
            var task = await _taskService.GetTask(taskId);
            if (task == null)
            {
                return View("Error", new ErrorViewModel { Errors = new List<string> { "Task not found" } });
            }

            var allStaffs = await _staffService.ListStaffs();
            var assignedStaffs = await _taskService.ListStaffForWorkTask(taskId);
            var availableStaffs = allStaffs.ExceptBy(assignedStaffs.Select(s => s.StaffId), s => s.StaffId);

            var viewModel = new AssignStaffViewModel
            {
                TaskId = taskId,
                TaskName = task.TaskName,
                AvailableStaffs = availableStaffs.ToList()
            };

            return View(viewModel);
        }

		/// <summary>
		/// Assigns a staff member to a task.
		/// </summary>
		/// <param name="taskId">Task ID</param>
		/// <param name="staffId">Staff ID</param>
		/// <returns>Redirect to task staff list or error view</returns>
		[HttpPost]
		public async Task<IActionResult> AssignStaffToTask(int taskId, int staffId)
		{
			var response = await _taskService.AssignStaffToTask(taskId, staffId);
			if (response.Status != ServiceResponse.ServiceStatus.Created)
			{
				TempData["Error"] = string.Join(", ", response.Messages);
			}
			return RedirectToAction("Details", new { id = taskId });
		}

		[HttpPost]
		public async Task<IActionResult> RemoveStaffFromTask(int taskId, int staffId)
		{
			var response = await _taskService.RemoveStaffFromTask(taskId, staffId);
			if (response.Status != ServiceResponse.ServiceStatus.Deleted)
			{
				TempData["Error"] = string.Join(", ", response.Messages);
			}
			return RedirectToAction("Details", new { id = taskId });
		}
	}
}