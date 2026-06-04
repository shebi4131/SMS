using Microsoft.EntityFrameworkCore;
using SMS.Data;
using SMS.Models;

namespace SMS.Services;

public interface IProjectService
{
    Task<List<Project>> GetAllProjectsAsync();
    Task<Project?> GetProjectByIdAsync(long proID);
    Task<string?> GetProjectNameAsync(long proID);
}

public class ProjectService : IProjectService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProjectService> _logger;
    private List<Project>? _projectCache;

    public ProjectService(ApplicationDbContext context, ILogger<ProjectService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Project>> GetAllProjectsAsync()
    {
        try
        {
            if (_projectCache != null)
            {
                return _projectCache;
            }

            _projectCache = await _context.Projects
                .OrderBy(p => p.ProName)
                .ToListAsync();

            _logger.LogInformation($"Loaded {_projectCache.Count} projects from database");
            return _projectCache;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading projects");
            throw;
        }
    }

    public async Task<Project?> GetProjectByIdAsync(long proID)
    {
        try
        {
            var projects = await GetAllProjectsAsync();
            return projects.FirstOrDefault(p => p.ProID == proID);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving project with ID: {proID}");
            throw;
        }
    }

    public async Task<string?> GetProjectNameAsync(long proID)
    {
        try
        {
            var project = await GetProjectByIdAsync(proID);
            return project?.ProName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving project name for ID: {proID}");
            throw;
        }
    }
}
