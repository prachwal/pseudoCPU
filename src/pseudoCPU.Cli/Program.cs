using pseudoCPU.Core;

if (args.Length == 0 || args[0] is "--help" or "-h")
{
    PrintHelp();
    return;
}

var command = args[0];
var options = ParseOptions(args[1..]);

try
{
    switch (command)
    {
        case "run-asm":
            RunAssemblerProgram(options);
            break;
        case "run-bin":
            RunBinaryProgram(options);
            break;
        default:
            Console.Error.WriteLine($"Unknown command '{command}'.");
            Environment.ExitCode = 1;
            break;
    }
}
catch (Exception exception) when (exception is FormatException or InvalidOperationException or NotSupportedException or ArgumentException or ArgumentOutOfRangeException or OverflowException)
{
    Console.Error.WriteLine(exception.Message);
    Environment.ExitCode = 1;
}

static void RunAssemblerProgram(Dictionary<string, string> options)
{
    var sourcePath = RequireOption(options, "source");
    var startAddress = ParseHexUShort(RequireOption(options, "start"));
    var maxSteps = ParseIntOption(options, "max-steps", 10_000);
    var trace = options.ContainsKey("trace");

    var source = File.ReadAllText(sourcePath);
    var program = BootstrapAssembler.Assemble(source);
    ExecuteProgram(program, startAddress, maxSteps, trace);
}

static void RunBinaryProgram(Dictionary<string, string> options)
{
    var binaryPath = RequireOption(options, "source");
    var startAddress = ParseHexUShort(RequireOption(options, "start"));
    var maxSteps = ParseIntOption(options, "max-steps", 10_000);
    var trace = options.ContainsKey("trace");

    var program = File.ReadAllBytes(binaryPath);
    ExecuteProgram(program, startAddress, maxSteps, trace);
}

static void ExecuteProgram(byte[] program, ushort startAddress, int maxSteps, bool trace)
{
    var cpu = new BootstrapCpu();
    cpu.LoadProgram(program, startAddress);

    if (trace)
    {
        Console.WriteLine($"Trace: start=0x{startAddress:X4}");
    }

    int steps;
    try
    {
        steps = cpu.RunUntilHalt(maxSteps);
        Console.WriteLine("Status: Halted");
    }
    catch (InvalidOperationException)
    {
        steps = maxSteps;
        Console.WriteLine("Status: StepLimitReached");
    }

    Console.WriteLine($"Steps: {steps}");
    Console.WriteLine($"PC: 0x{cpu.PC:X4}");
    Console.WriteLine($"A: 0x{cpu.A:X2}");
    Console.WriteLine($"X: 0x{cpu.X:X2}");
    Console.WriteLine($"Zero: {cpu.Zero.ToString().ToLowerInvariant()}");
    Console.WriteLine($"Negative: {cpu.Negative.ToString().ToLowerInvariant()}");
}

static Dictionary<string, string> ParseOptions(ReadOnlySpan<string> args)
{
    var options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    for (var i = 0; i < args.Length; i++)
    {
        var arg = args[i];
        if (arg == "--trace")
        {
            options["trace"] = "true";
            continue;
        }

        if (!arg.StartsWith("--", StringComparison.Ordinal))
        {
            continue;
        }

        var key = arg[2..];
        if (i + 1 >= args.Length)
        {
            throw new ArgumentException($"Missing value for option '{arg}'.");
        }

        options[key] = args[++i];
    }

    return options;
}

static string RequireOption(Dictionary<string, string> options, string name)
    => options.TryGetValue(name, out var value) ? value : throw new ArgumentException($"Missing required option '--{name}'.");

static ushort ParseHexUShort(string value)
{
    value = value.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? value[2..] : value;
    return ushort.Parse(value, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
}

static int ParseIntOption(Dictionary<string, string> options, string name, int defaultValue)
    => options.TryGetValue(name, out var value) ? int.Parse(value, System.Globalization.CultureInfo.InvariantCulture) : defaultValue;

static void PrintHelp()
{
    Console.WriteLine("pseudoCPU.Cli");
    Console.WriteLine("Usage:");
    Console.WriteLine("  pseudoCPU.Cli run-asm --source <file.asm> --start 0x0600 --max-steps 10000 [--trace]");
    Console.WriteLine("  pseudoCPU.Cli run-bin --source <file.bin> --start 0x0600 --max-steps 10000 [--trace]");
}
