using PassionProject.Data;
using PassionProject.Interface;
using PassionProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PassionProject.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;

        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lists all tasks.
        /// </summary>
        /// <returns>A collection of task DTOs.</returns>
        public async Task<IEnumerable<WorkTaskDto>> ListTasks()
        {
            return await _context.WorkTasks.Select(task => new WorkTaskDto
            {
                id = task.id,
                TaskName = task.TaskName,
                Description = task.Description,
            }).ToListAsync();
        }

        /// <summary>
        /// Fetches a specific task by ID.
        /// </summary>
        /// <param name="id">The ID of the task.</param>
        /// <returns>The task DTO or null if not found.</returns>
        public async Task<WorkTaskDto?> GetTask(int id)
        {
            var task = await _context.WorkTasks
                .Include(t => t.Staffs) // Include the related Staffs
                .FirstOrDefaultAsync(t => t.id == id);

            if (task == null) return null;

            return new WorkTaskDto
            {
                id = task.id,
                TaskName = task.TaskName,
                Description = task.Description,
                Staffs = task.Staffs?.Select(s => new StaffDto
                {
                    StaffId = s.StaffId,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Position = s.Position,
                    Contact = s.Contact
                    // Add other StaffDto properties as needed
                }).ToList() ?? new List<StaffDto>()
            };
        }

		public async Task<ServiceResponse> CreateTask(WorkTaskDto taskDto)
		{
			var serviceResponse = new ServiceResponse();

			if (string.IsNullOrEmpty(taskDto.TaskName))
			{
				serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
				serviceResponse.Messages.Add("Task name is required.");
				return serviceResponse;
			}

			var task = new WorkTask
			{
				TaskName = taskDto.TaskName,
				Description = taskDto.Description,
				Staffs = new List<Staff>() 
			};

			_context.WorkTasks.Add(task);
			await _context.SaveChangesAsync();

			serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
			serviceResponse.CreatedId = task.id;
			serviceResponse.Messages.Add("Task created successfully.");

			return serviceResponse;
		}
		/// <summary>
		/// Updates an existing task.
		/// </summary>
		/// <param name="id">The ID of the task to update.</param>
		/// <param name="taskDto">The updated task DTO.</param>
		/// <returns>A response indicating the result of the update.</returns>
		public async Task<ServiceResponse> UpdateTask(int id, WorkTaskDto taskDto)
		{
			var serviceResponse = new ServiceResponse();

			var existingTask = await _context.WorkTasks.FindAsync(id);
			if (existingTask == null)
			{
				serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
				serviceResponse.Messages.Add("Task not found.");
				return serviceResponse;
			}

			existingTask.TaskName = taskDto.TaskName;
			existingTask.Description = taskDto.Description;

			await _context.SaveChangesAsync();

			serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
			serviceResponse.Messages.Add("Task updated successfully.");

			return serviceResponse;
		}

       
        /// <summary>
        /// Deletes a task by ID.
        /// </summary>
        /// <param name="id">The ID of the task to delete.</param>
        /// <returns>A response indicating the result of the deletion.</returns>
        public async Task<ServiceResponse> DeleteTask(int id)
        {
            ServiceResponse response = new();

            var task = await _context.WorkTasks.FindAsync(id);
            if (task == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Task not found.");
                return response;
            }

            _context.WorkTasks.Remove(task);
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Deleted;
            response.Messages.Add("Task deleted successfully.");

            return response;
        }

        public async Task<IEnumerable<StaffDto>> ListStaffForWorkTask(int workTaskId)
        {
            return await _context.WorkTasks
                .Where(wt => wt.id == workTaskId)
                .SelectMany(wt => wt.Staffs)
                .Select(s => new StaffDto
                {
                    StaffId = s.StaffId,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Position = s.Position,
                    Contact = s.Contact
                })
                .ToListAsync();
        }

		public async Task<ServiceResponse> AssignStaffToTask(int taskId, int staffId)
		{
			var response = new ServiceResponse();

			var task = await _context.WorkTasks
				.Include(t => t.Staffs)
				.FirstOrDefaultAsync(t => t.id == taskId);

			var staff = await _context.Staffs.FindAsync(staffId);

			if (task == null || staff == null)
			{
				response.Status = ServiceResponse.ServiceStatus.Error;
				response.Messages.Add(task == null ? "Task not found" : "Staff not found");
				return response;
			}

			task.Staffs.Add(staff);
			await _context.SaveChangesAsync();

			response.Status = ServiceResponse.ServiceStatus.Created;
			return response;
		}

		public async Task<ServiceResponse> RemoveStaffFromTask(int taskId, int staffId)
		{
			var response = new ServiceResponse();

			var task = await _context.WorkTasks
				.Include(t => t.Staffs)
				.FirstOrDefaultAsync(t => t.id == taskId);

			if (task == null)
			{
				response.Status = ServiceResponse.ServiceStatus.Error;
				response.Messages.Add("Task not found");
				return response;
			}

			var staff = task.Staffs.FirstOrDefault(s => s.StaffId == staffId);
			if (staff != null)
			{
				task.Staffs.Remove(staff);
				await _context.SaveChangesAsync();
			}

			response.Status = ServiceResponse.ServiceStatus.Deleted;
			return response;
		}
	}
}