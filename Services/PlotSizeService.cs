using Microsoft.EntityFrameworkCore;
using SMS.Data;
using SMS.Models;

namespace SMS.Services;

public interface IPlotSizeService
{
    Task<List<PlotSize>> GetAllPlotSizesAsync();
    Task<List<PlotSize>> GetPlotSizesByCategoryAsync(int categoryId);
}

public class PlotSizeService : IPlotSizeService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PlotSizeService> _logger;
    private List<PlotSize>? _plotSizeCache;

    public PlotSizeService(ApplicationDbContext context, ILogger<PlotSizeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<PlotSize>> GetAllPlotSizesAsync()
    {
        try
        {
            if (_plotSizeCache != null)
            {
                return _plotSizeCache;
            }

            _plotSizeCache = await _context.PlotSizes
                .Where(p => p.isShow == true)
                .OrderBy(p => p.PlotSizeValue)
                .ToListAsync();

            _logger.LogInformation($"Loaded {_plotSizeCache.Count} plot sizes from database");
            return _plotSizeCache;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading plot sizes");
            throw;
        }
    }

    public async Task<List<PlotSize>> GetPlotSizesByCategoryAsync(int categoryId)
    {
        try
        {
            var plotSizes = await GetAllPlotSizesAsync();

            if (categoryId <= 0)
            {
                return plotSizes;
            }

            return plotSizes
                .Where(p => p.PlotCategoryID == categoryId)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error loading plot sizes for category {categoryId}");
            throw;
        }
    }
}
