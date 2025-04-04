using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PassionProject.Interface;
using PassionProject.Models;

namespace PassionProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskAPIController : ControllerBase
    {
        private readonly ITaskService _taskService;

        // Dependency injection of service interfaces
        public TaskAPIController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        /// <summary>
        /// This endpoint returns a list of all work tasks in the system.
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{WorkTaskDto}, {WorkTaskDto}, ...]
        /// </returns>
        /// <example>
        /// GET: api/TaskAPI/List ->
        /// [{"id":1,"TaskName":"Inventory Check","Description":"Monthly inventory check"},
        /// {"id":2,"TaskName":"Customer Meeting","Description":"Meeting with key client"}]
        /// </example>
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<WorkTaskDto>>> ListTasks()
        {
            var tasks = await _taskService.ListTasks();
            return Ok(tasks);
        }

        /// <summary>
        /// This endpoint finds a specific work task by ID.
        /// </summary>
        /// <param name="id">The Task ID</param>
        /// <returns>
        /// 200 OK
        /// {WorkTaskDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/TaskAPI/Find/1 -> 
        /// {
        ///   "id":1,
        ///   "TaskName":"Inventory Check",
        ///   "Description":"Monthly inventory check",
        ///   "Staffs":[
        ///     {"StaffId":1,"FirstName":"John","LastName":"Doe","Position":"Manager","Contact":"john@example.com"}
        ///   ]
        /// }
        /// </example>
        [HttpGet("Find/{id}")]
        public async Task<ActionResult<WorkTaskDto>> GetTask(int id)
        {
            var task = await _taskService.GetTask(id);
            if (task == null)
            {
                return NotFound("Task not found");
            }
            return Ok(task);
        }

		/// <summary>
		/// This endpoint creates a new work task.
		/// </summary>
		/// <param name="taskDto">The task data</param>
		/// <returns>
		/// 201 Created
		/// Location: api/TaskAPI/Find/{id}
		/// {ServiceResponse}
		/// or
		/// 400 Bad Request
		/// </returns>
		/// <example>
		/// POST: api/TaskAPI/Add
		/// Request Body: 
		/// {
		///   "TaskName":"New Task",
		///   "Description":"Task description",
		///   "Staffs":[{"StaffId":1}]
		/// }
		/// ->
		/// Response Code: 201 Created
		/// Response Body: {"Status":1,"Messages":["Task created successfully"],"CreatedId":3}
		/// </example>
		[HttpPost("Add")]
		public async Task<ActionResult<ServiceResponse>> CreateTask([FromBody] WorkTaskDto taskDto)
		{
			var result = await _taskService.CreateTask(taskDto);
			if (result.Status == ServiceResponse.ServiceStatus.Created)
			{
				return CreatedAtAction(nameof(GetTask), new { id = result.CreatedId }, result);
			}
			return BadRequest(result);
		}

		[HttpPut("Update/{id}")]
		public async Task<ActionResult<ServiceResponse>> UpdateTask(int id, [FromBody] WorkTaskDto taskDto)
		{
			var result = await _taskService.UpdateTask(id, taskDto);
			if (result.Status == ServiceResponse.ServiceStatus.NotFound)
			{
				return NotFound(result.Messages);
			}
			return Ok(result);
		}

		/// <summary>
		/// This endpoint deletes a work task by ID.
		/// </summary>
		/// <param name="id">The ID of the task to delete</param>
		/// <returns>
		/// 200 OK
		/// {ServiceResponse}
		/// or
		/// 404 Not Found
		/// </returns>
		/// <example>
		/// DELETE: api/TaskAPI/Delete/1
		/// ->
		/// Response Code: 200 OK
		/// Response Body: {"Status":3,"Messages":["Task deleted successfully"],"CreatedId":null}
		/// </example>
		[HttpDelete("Delete/{id}")]
        public async Task<ActionResult<ServiceResponse>> DeleteTask(int id)
        {
            var result = await _taskService.DeleteTask(id);

            if (result.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(result.Messages);
            }

            return Ok(result);
        }

        /// <summary>
        /// This endpoint returns a list of all staff assigned to a specific work task.
        /// </summary>
        /// <param name="workTaskId">The WorkTask ID</param>
        /// <returns>
        /// 200 OK
        /// [{StaffDto}, {StaffDto}, ...]
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/TaskAPI/ListStaff/1 ->
        /// [{"StaffId":1,"FirstName":"John","LastName":"Doe","Position":"Manager","Contact":"john@example.com"}]
        /// </example>
        [HttpGet("ListStaff/{workTaskId}")]
        public async Task<ActionResult<IEnumerable<StaffDto>>> ListStaffForWorkTask(int workTaskId)
        {
            var staff = await _taskService.ListStaffForWorkTask(workTaskId);
            if (staff == null || !staff.Any())
            {
                return NotFound("No staff assigned to this task");
            }
            return Ok(staff);
        }

        /// <summary>
        /// This endpoint assigns a staff member to a work task.
        /// </summary>
        /// <param name="workTaskId">The WorkTask ID</param>
        /// <param name="staffId">The Staff ID</param>
        /// <returns>
        /// 200 OK
        /// {ServiceResponse}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// POST: api/TaskAPI/AssignStaff/1/2
        /// ->
        /// Response Code: 200 OK
        /// Response Body: {"Status":1,"Messages":["Staff assigned to WorkTask successfully"],"CreatedId":null}
        /// </example>
        [HttpPost("AssignStaff/{workTaskId}/{staffId}")]
        public async Task<ActionResult<ServiceResponse>> AssignStaffToWorkTask(int workTaskId, int staffId)
        {
            var result = await _taskService.AssignStaffToTask(workTaskId, staffId);

            if (result.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(result.Messages);
            }

            return Ok(result);
        }

        /// <summary>
        /// This endpoint removes a staff member from a work task assignment.
        /// </summary>
        /// <param name="workTaskId">The WorkTask ID</param>
        /// <param name="staffId">The Staff ID</param>
        /// <returns>
        /// 200 OK
        /// {ServiceResponse}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/TaskAPI/RemoveStaff/1/2
        /// ->
        /// Response Code: 200 OK
        /// Response Body: {"Status":3,"Messages":["Staff removed from work task successfully"],"CreatedId":null}
        /// </example>
        [HttpDelete("RemoveStaff/{workTaskId}/{staffId}")]
        public async Task<ActionResult<ServiceResponse>> RemoveStaffFromWorkTask(int workTaskId, int staffId)
        {
            var result = await _taskService.RemoveStaffFromTask(workTaskId, staffId);

            if (result.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(result.Messages);
            }

            return Ok(result);
        }
    }
}