namespace FrankenBit.Laika;

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
    internal CSharpProjectTerritory(string directory)
    {
        _watcher = new FileSystemWatcher(directory, "*.cs");
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
