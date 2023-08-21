using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TTicket.DAL;

public class DbHealthCheck : IHealthCheck
{
    private readonly ApplicationDbContext _context;

    public DbHealthCheck(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var test = await _context.Product.
                OrderBy(p => p.Name).
                Take(1).
                ToListAsync();

            return HealthCheckResult.Healthy();
        }
        catch(Exception ex)
        {
            return HealthCheckResult.Unhealthy(exception: ex);
        }
    }
}
