using Microsoft.EntityFrameworkCore;
using SMS.Data;
using SMS.Models;

namespace SMS.Services;

public interface IPlotCategoryService
{
    Task<List<PlotCategory>> GetAllCategoriesAsync();
}

public class PlotCategoryService : IPlotCategoryService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PlotCategoryService> _logger;
    private List<PlotCategory>? _categoryCache;

    public PlotCategoryService(ApplicationDbContext context, ILogger<PlotCategoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<PlotCategory>> GetAllCategoriesAsync()
    {
        try
        {
            if (_categoryCache != null)
            {
                return _categoryCache;
            }

            _categoryCache = await _context.PlotCategories
                .OrderBy(c => c.Record)
                .ToListAsync();

            _logger.LogInformation($"Loaded {_categoryCache.Count} plot categories from database");
            return _categoryCache;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading plot categories");
            throw;
        }
    }
}
