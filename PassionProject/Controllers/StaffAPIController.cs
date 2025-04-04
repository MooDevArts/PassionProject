using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PassionProject.Interface;
using PassionProject.Models;

namespace PassionProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffAPIController : ControllerBase
    {
        private readonly IStaffService _StaffService;

        // Dependency injection of service interfaces
        public StaffAPIController(IStaffService StaffService)
        {
            _StaffService = StaffService;
        }

        /// <summary>
        /// This endpoint returns a list of all staff members in the system.
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{StaffDto}, {StaffDto}, ...]
        /// </returns>
        /// <example>
        /// GET: api/StaffAPI/List ->
        /// [{"StaffId":1,"FirstName":"John","LastName":"Doe","Position":"Manager","Contact":"John@123gmail.com"},
        /// {"StaffId":2,"FirstName":"Jane","LastName":"Doe","Position":"Salesperson","Contact":"Jane123@gmail.com"}]
        /// </example>
        [HttpGet(template: "List")]
        public async Task<IEnumerable<StaffDto>> ListStaffs()
        {
            return await _StaffService.ListStaffs();
        }

        /// <summary>
        /// This endpoint finds a specific staff member by ID.
        /// </summary>
        /// <param name="id">The Staff ID</param>
        /// <returns>
        /// 200 OK
        /// {StaffDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/StaffAPI/Find/1 -> {StaffDto}
        /// {"StaffId":1,"FirstName":"John","LastName":"Doe","Position":"Manager","Contact":"John123@gmail.com"}
        /// </example>
        [HttpGet(template: "Find/{id}")]
        public async Task<StaffDto> GetStaff(int id)
        {
            return await _StaffService.GetStaff(id);
        }

        /// <summary>
        /// This endpoint updates an existing staff member's information.
        /// </summary>
        /// <param name="id">Staff ID</param>
        /// <param name="staffDto">Updated staff information</param>
        /// <returns>
        /// 404 Not Found
        /// or 
        /// 400 Bad Request
        /// </returns>
        /// <example>
        /// PUT: api/StaffAPI/Update/1
        /// Request Headers: Content-Type: application/json
        /// Request Body: {StaffDto}
        /// -> 
        /// Response Code: 204 No Content
        /// </example>
        [HttpPut("{id}")]
        public async Task<ServiceResponse> UpdateStaff(int id, StaffDto staffDto)
        {
            return await _StaffService.UpdateStaff(id, staffDto);
        }

        /// <summary>
        /// Adds a new staff member.
        /// </summary>
        /// <param name="staffDto">The required information to add the staff member (StaffId, FirstName, LastName, etc.)</param>
        /// <returns>
        /// 201 Created
        /// Location: api/StaffAPI/Find/{StaffId}
        /// {StaffDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// POST: api/StaffAPI/Add
        /// Request Headers: Content-Type: application/json
        /// Request Body: {StaffDto}
        /// -> 
        /// Response Code: 201 Created
        /// Response Headers: Location: api/StaffAPI/Find/{StaffId}
        /// </example>
        [HttpPost(template: "Add")]
        public async Task<ServiceResponse> CreateStaff(StaffDto staffDto)
        {
            return await _StaffService.CreateStaff(staffDto);
        }

        /// <summary>
        /// Deletes a staff member by ID.
        /// </summary>
        /// <param name="id">The ID of the staff member to delete</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/StaffAPI/Delete/1
        /// -> 
        /// Response Code: 204 No Content
        /// </example>
        [HttpDelete("{id}")]
        public async Task<ServiceResponse> DeleteStaff(int id)
        {
            return await _StaffService.DeleteStaff(id);
        }

        /// <summary>
        /// This endpoint returns a list of all work tasks assigned to a specific staff member.
        /// </summary>
        /// <param name="staffId">The Staff ID</param>
        /// <returns>
        /// 200 OK
        /// [{WorkTaskDto}, {WorkTaskDto}, ...]
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/StaffAPI/ListTasks/1 ->
        /// [{"Id":1,"Name":"Inventory Check","Description":"Monthly inventory check"},
        /// {"Id":2,"Name":"Customer Meeting","Description":"Meeting with key client"}]
        /// </example>
        [HttpGet("ListTasks/{staffId}")]
        public async Task<ActionResult<IEnumerable<WorkTaskDto>>> ListWorkTasksForStaff(int staffId)
        {
            var tasks = await _StaffService.ListWorkTasksForStaff(staffId);
            if (tasks == null || !tasks.Any())
            {
                return NotFound("No tasks found for this staff member");
            }
            return Ok(tasks);
        }

        /// <summary>
        /// This endpoint assigns a work task to a staff member.
        /// </summary>
        /// <param name="staffId">The Staff ID</param>
        /// <param name="workTaskId">The WorkTask ID</param>
        /// <returns>
        /// 200 OK
        /// or
        /// 404 Not Found
        /// or
        /// 400 Bad Request
        /// </returns>
        /// <example>
        /// POST: api/StaffAPI/AssignTask/1/3
        /// -> 
        /// Response Code: 200 OK
        /// Response Body: {"Status":1,"Messages":["WorkTask assigned successfully"],"CreatedId":null}
        /// </example>
        [HttpPost("AssignTask/{staffId}/{workTaskId}")]
        public async Task<ActionResult<ServiceResponse>> AssignWorkTaskToStaff(int staffId, int workTaskId)
        {
            var result = await _StaffService.AssignWorkTaskToStaff(staffId, workTaskId);

            if (result.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(result.Messages);
            }
            else if (result.Status == ServiceResponse.ServiceStatus.Error)
            {
                return BadRequest(result.Messages);
            }

            return Ok(result);
        }

        /// <summary>
        /// This endpoint removes a work task from a staff member's assignments.
        /// </summary>
        /// <param name="staffId">The Staff ID</param>
        /// <param name="workTaskId">The WorkTask ID</param>
        /// <returns>
        /// 200 OK
        /// or
        /// 404 Not Found
        /// or
        /// 400 Bad Request
        /// </returns>
        /// <example>
        /// DELETE: api/StaffAPI/RemoveTask/1/3
        /// -> 
        /// Response Code: 200 OK
        /// Response Body: {"Status":1,"Messages":["WorkTask removed from staff successfully"],"CreatedId":null}
        /// </example>
        [HttpDelete("RemoveTask/{staffId}/{workTaskId}")]
        public async Task<ActionResult<ServiceResponse>> RemoveWorkTaskFromStaff(int staffId, int workTaskId)
        {
            var result = await _StaffService.RemoveWorkTaskFromStaff(staffId, workTaskId);

            if (result.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(result.Messages);
            }

            return Ok(result);
        }
    }
}
