namespace FrankenBit.Laika;

/// <summary>
///     Represents the Stryker.NET master process.
/// </summary>
internal interface IMaster
{
    /// <summary>
    ///     Alerts the master that another mutation testing process should be started.
    /// </summary>
    /// <param name="cancellation">
    ///     A token to cancel the processing of the alert.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous alert processing operation.
    /// </returns>
    Task AlertAsync(CancellationToken cancellation);
}
