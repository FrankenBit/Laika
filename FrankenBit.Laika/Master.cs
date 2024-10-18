using System.Diagnostics;

namespace FrankenBit.Laika;

/// <summary>
///     Represents the Stryker.NET master process.
/// </summary>
internal interface IMaster
{
    event Action? HasFinished;

    bool IsBusy { get; }

    /// <summary>
    ///     Alerts the master that another mutation testing process should be started.
    /// </summary>
    void Alert();

    /// <summary>
    ///     Interrupts the current mutation testing process.
    /// </summary>
    void Interrupt();
}

/// <summary>
///     Defines the soul of the <see cref="Doggy" />.
/// </summary>
internal enum Soul
{
    /// <summary>
    ///     A patient soul, that waits for the master to finish.
    /// </summary>
    Patient,

    /// <summary>
    ///     An impatient soul, that interrupts the master when it is busy.
    /// </summary>
    Impatient
}

internal sealed class MasterProcess : IMaster
{
    private readonly ProcessStartInfo _startInfo = new("dotnet", ["stryker", "--open-report"]);

    private Process? _currentProcess;

    public event Action? HasFinished;

    public bool IsBusy =>
        _currentProcess?.HasExited == false;

    public void Alert()
    {
        if (IsBusy) throw new InvalidOperationException("The master is busy.");

        _currentProcess = Process.Start(_startInfo);
        if (_currentProcess != null) _currentProcess.Exited += HandleProcessExit;
    }

    public void Interrupt() =>
        _currentProcess?.Kill();

    private void HandleProcessExit(object? sender, EventArgs e)
    {
        _currentProcess = default;
        HasFinished?.Invoke();
    }
}

/// <summary>
///     The Laika itself.
/// </summary>
internal sealed class Doggy
{
    private readonly IMaster _master;

    private readonly Soul _soul;

    private bool _shouldNotify;

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
        master.HasFinished += HandleMasterHasFinished;
        territory.HasChanged += HandleTerritoryHasChanged;
    }

    internal void Watch()
    {

    }

    private void HandleTerritoryHasChanged()
    {
        if (!_master.IsBusy) _master.Alert();
        else AlertLater();
    }

    private void AlertLater()
    {
        _shouldNotify = true;
        if (_soul == Soul.Impatient) _master.Interrupt();
    }

    private void HandleMasterHasFinished()
    {
        if (_shouldNotify) _master.Alert();
        _shouldNotify = false;
    }
}

/// <summary>
///    Represents the target project for mutation testing.
/// </summary>
internal interface ITerritory
{
    /// <summary>
    ///     Occurs when the <see cref="ITerritory" /> has changed.
    /// </summary>
    event Action? HasChanged;
}

/// <summary>
///     A territory to watch for changes in a C# project.
/// </summary>
internal sealed class CSharpProjectTerritory : IDisposable, ITerritory
{
    private readonly FileSystemWatcher _watcher;

    /// <summary>
    ///     Creates a new instance of <see cref="CSharpProjectTerritory" />.
    /// </summary>
    /// <param name="directory">
    ///     The directory of the C# project to watch.
    /// </param>
    internal CSharpProjectTerritory(DirectoryInfo directory)
    {
        _watcher = new FileSystemWatcher(directory.FullName, "*.cs");
        _watcher.Changed += HandleFileChanged;
        _watcher.Created += HandleFileChanged;
        _watcher.EnableRaisingEvents = true;
    }

    /// <inheritdoc />
    public event Action? HasChanged;

    /// <inheritdoc />
    public void Dispose() =>
        _watcher.Dispose();

    private void HandleFileChanged(object sender, FileSystemEventArgs e) =>
        HasChanged?.Invoke();
}
