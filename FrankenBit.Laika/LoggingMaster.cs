using Microsoft.Extensions.Logging;

namespace FrankenBit.Laika;

/// <summary>
///     A logging decorator for the master that logs the alerting and handling of the master.
/// </summary>
/// <param name="master">
///     The actual master to decorate.
/// </param>
/// <param name="logger">
///     A logger to log the alerting and handling of the master.
/// </param>
internal sealed class LoggingMaster(IMaster master, ILogger<IMaster> logger) : IMaster
{
    private int _alertCount;

    /// <inheritdoc />
    public async Task AlertAsync(CancellationToken cancellation)
    {
        int count = Interlocked.Increment(ref _alertCount);
        logger.LogDebug("Alerting the master (alert #{Count}).", count);

        await master.AlertAsync(cancellation);

        logger.LogDebug("Master has finished handling the alert #{Count}.", count);
    }
}
