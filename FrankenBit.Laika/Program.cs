using FrankenBit.Laika;
using Microsoft.Extensions.Logging;

string[] help = ["-?", "-h", "--help"];
const string impatient = "--impatient";
const string patient = "--patient";

using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

Soul soul = args.Contains(impatient) ? Soul.Impatient : Soul.Patient;
var cts = new CancellationTokenSource();

var master = new MasterProcess(args.Except([patient, impatient]));
await (help.Any(args.Contains)
    ? ShowHelpAsync(master, cts)
    : LaunchLaikaAsync(master, loggerFactory, soul, cts.Token));
return;

async Task ShowHelpAsync(MasterProcess masterProcess, CancellationTokenSource cancellationTokenSource)
{
    const string instructions =
        """
        Laika usage:
          -?|-h|--help                        Display this help message.
        
          --impatient                         Set Laika to be impatient (restart Stryker as soon as any source file changes).
                                              This could prevent Stryker to generate any report at all because it will be
                                              restarted all the time as soon as any source file changes.
        
          --patient                           Set Laika to be patient (do not restart Stryker as soon as any file changes).
                                              Default behavior.
        
                                              This behavior could lead to outdated reports being displayed if some other changes
                                              were made after the start of the last Stryker run. However, after the last Stryker
                                              run, the reports will be up-to-date.
        """;

    await masterProcess.AlertAsync(cancellationTokenSource.Token);
    Console.WriteLine();
    Console.WriteLine(instructions);
}

async Task LaunchLaikaAsync(MasterProcess master1, ILoggerFactory loggerFactory1, Soul soul1, CancellationToken cancellation)
{
    var loggingMaster = new LoggingMaster(master1, loggerFactory1.CreateLogger<IMaster>());
    var territory = new CSharpProjectTerritory(Environment.CurrentDirectory);
    using var laika = new Doggy(loggingMaster, territory, soul1);

    await laika.WatchAsync(cancellation);
}
