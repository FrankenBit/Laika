using System.Diagnostics;

namespace FrankenBit.Laika;

/// <summary>
///     The master STRYKER.NET process for the mutation testing.
/// </summary>
/// <param name="arguments">
///     Additional arguments to pass to STRYKER.NET.
/// </param>
internal sealed class MasterProcess(IEnumerable<string> arguments) : IMaster
{
    private const string DotNetTool = "dotnet";

    private readonly ProcessStartInfo _startInfo = new(DotNetTool, ["stryker", ..arguments]);

    /// <inheritdoc />
    public async Task AlertAsync(CancellationToken cancellation)
    {
        using var process = new Process();
        process.StartInfo = _startInfo;

        if (!process.Start()) return;

        try
        {
            await process.WaitForExitAsync(cancellation);
        }
        catch (OperationCanceledException)
        {
            process.Kill();
        }
    }
}
