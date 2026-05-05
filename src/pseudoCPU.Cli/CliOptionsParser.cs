using System.Globalization;

namespace pseudoCPU.Cli;

internal static class CliOptionsParser
{
    public static IReadOnlyDictionary<string, string> Parse(ReadOnlySpan<string> args)
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

    public static string RequireOption(IReadOnlyDictionary<string, string> options, string name)
        => options.TryGetValue(name, out var value) ? value : throw new ArgumentException($"Missing required option '--{name}'.");

    public static ushort ParseHexUShort(string value)
    {
        value = value.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? value[2..] : value;
        return ushort.Parse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
    }

    public static int ParseIntOption(IReadOnlyDictionary<string, string> options, string name, int defaultValue)
        => options.TryGetValue(name, out var value) ? int.Parse(value, CultureInfo.InvariantCulture) : defaultValue;
}
