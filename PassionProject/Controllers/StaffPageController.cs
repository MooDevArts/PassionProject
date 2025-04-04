using Microsoft.AspNetCore.Mvc;
using PassionProject.Interface;
using PassionProject.Models.ViewModels;
using PassionProject.Models;
using PassionProject.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;


namespace PassionProject.Controllers
{
    public class StaffPageController : Controller
    {
        private readonly IStaffService _StaffService;
        private readonly ICarService _carService;
        private readonly ITaskService _taskService;
        private readonly ApplicationDbContext _context;

        // Dependency injection of service interfaces
        public StaffPageController(ApplicationDbContext context, IStaffService StaffService, ICarService carService,
ITaskService taskService)
        {
            _StaffService = StaffService;
            _carService = carService;
            _taskService = taskService;
            _context = context;
        }

        /// <summary>
        /// Retrieves a list of staff members.
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{StaffDto}, {StaffDto}, ...]
        /// </returns>
        /// <example>
        /// GET StaffPage/List -> [{StaffDto}, {StaffDto}, ...]
        /// </example>
        [HttpGet]
        public async Task<IActionResult> List()
        {
            IEnumerable<StaffDto?> staffDtos = await _StaffService.ListStaffs();
            return View(staffDtos);
        }

        /// <summary>
        /// Retrieves details of a specific staff member by ID.
        /// </summary>
        /// <param name="id">The ID of the staff member.</param>
        /// <returns>
        /// 200 OK
        /// {StaffDto}
        /// </returns>
        /// <example>
        /// GET StaffPage/Details/{id} -> {StaffDto}
        /// </example>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            StaffDto? staffDto = await _StaffService.GetStaff(id);
            return View(staffDto);
        }

        /// <summary>
        /// Loads the form to create a new staff member.
        /// </summary>
        /// <returns>
        /// 200 OK
        /// {StaffDto with Cars}
        /// </returns>
        /// <example>
        /// GET StaffPage/New -> {StaffDto}
        /// </example>
        public async Task<ActionResult> New()
        {
            var staffDto = new StaffDto();
            var cars = await _carService.ListCars();
            staffDto.Cars = cars.ToList();
            return View("new", staffDto);
        }

        /// <summary>
        /// Creates a new staff member.
        /// </summary>
        /// <param name="staffDto">The data transfer object containing staff details.</param>
        /// <returns>
        /// 201 Created
        /// Redirect to List
        /// </returns>
        /// <example>
        /// POST StaffPage/Create -> Redirect to List
        /// </example>
        [HttpPost]
        public async Task<IActionResult> Create(StaffDto staffDto)
        {
            ServiceResponse response = await _StaffService.CreateStaff(staffDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("List", "StaffPage");
            }
            else
            {
                return View("Error", new ErrorViewModel { Errors = response.Messages });
            }
        }

        /// <summary>
        /// Retrieves details for editing a specific staff member by ID.
        /// </summary>
        /// <param name="id">The ID of the staff member.</param>
        /// <returns>
        /// 200 OK
        /// {StaffDto}
        /// </returns>
        /// <example>
        /// GET StaffPage/Edit/{id} -> {StaffDto}
        /// </example>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            StaffDto? staffDto = await _StaffService.GetStaff(id);
            if (staffDto == null)
            {
                return View("Error");
            }
            return View(staffDto);
        }

        /// <summary>
        /// Updates the details of a specific staff member by ID.
        /// </summary>
        /// <param name="id">The ID of the staff member.</param>
        /// <param name="staffDto">The updated data transfer object containing staff details.</param>
        /// <returns>
        /// 200 OK
        /// Redirect to List
        /// </returns>
        /// <example>
        /// POST StaffPage/Edit/{id} -> Redirect to List
        /// </example>
        [HttpPost]
        public async Task<IActionResult> Edit(int id, StaffDto staffDto)
        {
            if (id != staffDto.StaffId)
            {
                return BadRequest();
            }

            ServiceResponse response = await _StaffService.UpdateStaff(id, staffDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("List");
            }
            else
            {
                ModelState.AddModelError("", response.Messages.FirstOrDefault() ?? "Error updating staff member.");
            }

            return View(staffDto);
        }

        /// <summary>
        /// Retrieves a staff member for deletion confirmation by ID.
        /// </summary>
        /// <param name="id">The ID of the staff member.</param>
        /// <returns>
        /// 200 OK
        /// {StaffDto}
        /// </returns>
        /// <example>
        /// GET StaffPage/Delete/{id} -> {StaffDto}
        /// </example>
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            StaffDto? staffDto = await _StaffService.GetStaff(id);
            if (staffDto == null)
            {
                return View("Error");
            }
            return View(staffDto);
        }

        /// <summary>
        /// Confirms and deletes a specific staff member by ID.
        /// </summary>
        /// <param name="id">The ID of the staff member.</param>
        /// <returns>
        /// 200 OK
        /// Redirect to List
        /// </returns>
        /// <example>
        /// POST StaffPage/DeleteConfirmed/{id} -> Redirect to List
        /// </example>
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ServiceResponse response = await _StaffService.DeleteStaff(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List", "StaffPage");
            }
            else
            {
                return View("Error", new ErrorViewModel { Errors = response.Messages });
            }
        }

        /// <summary>
        /// Retrieves a list of tasks assigned to a specific staff member.
        /// </summary>
        /// <param name="staffId">The ID of the staff member.</param>
        /// <returns>
        /// 200 OK
        /// {StaffWithTasksViewModel}
        /// </returns>
        /// <example>
        /// GET StaffPage/StaffTasks/1 -> {StaffWithTasksViewModel}
        /// </example>
        [HttpGet]
        public async Task<IActionResult> StaffTasks(int staffId)
        {

            var staff = await _StaffService.GetStaff(staffId);
            if (staff == null)
            {
                return View("Error", new ErrorViewModel { Errors = new List<string> { "Staff not found" } });
            }

            var tasks = await _StaffService.ListWorkTasksForStaff(staffId);

            var viewModel = new StaffDetails
            {
                Staff = staff,
                Tasks = tasks.ToList()
            };

            return View(viewModel);
        }

        /// <summary>
        /// Loads the form to assign a new task to a staff member.
        /// </summary>
        /// <param name="staffId">The ID of the staff member.</param>
        /// <returns>
        /// 200 OK
        /// {AssignTaskViewModel}
        /// </returns>
        /// <example>
        /// GET StaffPage/AssignTask/1 -> {AssignTaskViewModel}
        /// </example>
        [HttpGet]
        public async Task<IActionResult> AssignTask(int staffId)
        {
            var staff = await _StaffService.GetStaff(staffId);
            if (staff == null) return NotFound();

            var allTasks = await _taskService.ListTasks();

            var assignedTaskIds = await _context.Staffs
                .Where(s => s.StaffId == staffId)
                .SelectMany(s => s.WorkTasks)
                .Select(t => t.id)
                .ToListAsync();

            var availableTasks = allTasks
                .Where(t => !assignedTaskIds.Contains(t.id))
                .ToList();

            var viewModel = new AssignTaskViewModel
            {
                StaffId = staffId,
                StaffName = $"{staff.FirstName} {staff.LastName}",
                AvailableTasks = availableTasks
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AssignTask(int staffId, List<int> selectedTaskIds)
        {
            if (selectedTaskIds == null || !selectedTaskIds.Any())
            {
                return View("Error", new ErrorViewModel { Errors = new List<string> { "No tasks selected" } });
            }

            foreach (var taskId in selectedTaskIds)
            {
                var response = await _StaffService.AssignWorkTaskToStaff(staffId, taskId);
                if (response.Status != ServiceResponse.ServiceStatus.Created)
                {
                    return View("Error", new ErrorViewModel { Errors = response.Messages });
                }
            }

            return RedirectToAction("Details", new { id = staffId });
        }

        /// <summary>
        /// Removes a task assignment from a staff member.
        /// </summary>
        /// <param name="staffId">The ID of the staff member.</param>
        /// <param name="taskId">The ID of the task to remove.</param>
        /// <returns>
        /// 200 OK
        /// Redirect to StaffTasks
        /// or
        /// Error view
        /// </returns>
        /// <example>
        /// POST StaffPage/RemoveTask/1/2 -> Redirect to StaffTasks
        /// </example>
        [HttpPost]
        public async Task<IActionResult> RemoveTask(int staffId, int taskId)
        {
            var response = await _StaffService.RemoveWorkTaskFromStaff(staffId, taskId);

            if (response.Status != ServiceResponse.ServiceStatus.Deleted)
            {
                return View("Error", new ErrorViewModel { Errors = response.Messages });
            }

            return RedirectToAction("Details", new { id = staffId });
        }
    }
}


