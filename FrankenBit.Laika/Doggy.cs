namespace FrankenBit.Laika;

/// <summary>
///     The Laika itself.
/// </summary>
internal sealed class Doggy : IDisposable
{
    private static readonly Func<Task> BeLazy = () => Task.CompletedTask;

    private readonly IMaster _master;

    private readonly AutoResetEvent _somethingSuspicious = new(true);

    private readonly Soul _soul;

    private Func<Task> _impatientBehavior = BeLazy;

    /// <summary>
    ///     Initializes a new instance of <see cref="Doggy" />.
    /// </summary>
    /// <param name="master">
    ///     The Stryker.NET master process.
    /// </param>
    /// <param name="territory">
    ///     The territory to watch for changes.
    /// </param>
    /// <param name="soul">
    ///     The soul of the <see cref="Doggy" />.
    /// </param>
    internal Doggy(IMaster master, ITerritory territory, Soul soul)
    {
        _master = master;
        _soul = soul;
        territory.HasChanged += HandleTerritoryHasChanged;
    }

    /// <inheritdoc />
    public void Dispose() =>
        _somethingSuspicious.Dispose();

    /// <summary>
    ///     Watches the territory for changes and alerts the master when something suspicious happens.
    /// </summary>
    /// <param name="cancellation">
    ///     A token to cancel the watching process.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous watching operation.
    /// </returns>
    internal async Task WatchAsync(CancellationToken cancellation)
    {
        WaitHandle[] interestingThings = [cancellation.WaitHandle, _somethingSuspicious];

        while (WaitHandle.WaitAny(interestingThings) != 0)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
            if (_soul == Soul.Impatient) _impatientBehavior = cts.CancelAsync;

            await _master.AlertAsync(cts.Token);

            _impatientBehavior = BeLazy;
        }
    }

    private async void HandleTerritoryHasChanged()
    {
        _somethingSuspicious.Set();
        await _impatientBehavior.Invoke();
    }
}
