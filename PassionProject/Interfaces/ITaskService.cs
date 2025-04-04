using PassionProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PassionProject.Interface
{
    public interface ITaskService
    {
        // Fetch all Task members
        Task<IEnumerable<WorkTaskDto>> ListTasks();

        // Fetch a specific Task member by ID
        Task<WorkTaskDto> GetTask(int id);

        // Update an existing Task member
        Task<ServiceResponse> UpdateTask(int id, WorkTaskDto TaskDto);

        // Create a new Task member
        Task<ServiceResponse> CreateTask(WorkTaskDto TaskDto);

        // Delete a Task member by ID
        Task<ServiceResponse> DeleteTask(int id);

        // related methods
        Task<IEnumerable<StaffDto>> ListStaffForWorkTask(int workTaskId);
		Task<ServiceResponse> AssignStaffToTask(int taskId, int staffId);
		Task<ServiceResponse> RemoveStaffFromTask(int taskId, int staffId);
		//Task<IEnumerable<TaskDto>> ListTasksForStaff(int id);
		// Task<ServiceResponse> ListTasksForStaff(int categoryId, int productId);

		// Task<ServiceResponse> UnlinkTasksForCar(int categoryId, int productId);
	}
}
