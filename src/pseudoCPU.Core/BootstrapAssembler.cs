using System.Globalization;

namespace pseudoCPU.Core;

public static class BootstrapAssembler
{
    public static byte[] Assemble(string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var bytes = new List<byte>();

        foreach (var rawLine in ReadLines(source))
        {
            var line = StripComment(rawLine).Trim();

            if (line.Length == 0)
            {
                continue;
            }

            var (mnemonic, operand) = ParseInstruction(line);

            switch (mnemonic)
            {
                case "LDA":
                    RequireOperand(mnemonic, operand, '#');
                    bytes.Add((byte)BootstrapOpcode.LdaImmediate);
                    bytes.Add(ParseByteLiteral(operand[1..], mnemonic));
                    break;
                case "TAX":
                    RequireNoOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.Tax);
                    break;
                case "INX":
                    RequireNoOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.Inx);
                    break;
                case "STA":
                    RequireOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.StaAbsolute);
                    bytes.AddRange(ParseWordLiteral(operand, mnemonic));
                    break;
                case "BRK":
                    RequireNoOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.Brk);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported bootstrap mnemonic '{mnemonic}'.");
            }
        }

        return bytes.ToArray();
    }

    private static IEnumerable<string> ReadLines(string source)
    {
        using var reader = new StringReader(source);

        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            yield return line;
        }
    }

    private static string StripComment(string line)
    {
        var commentIndex = line.IndexOf(';');
        return commentIndex < 0 ? line : line[..commentIndex];
    }

    private static (string Mnemonic, string Operand) ParseInstruction(string line)
    {
        var separatorIndex = line.IndexOfAny([' ', '\t']);

        if (separatorIndex < 0)
        {
            return (line.ToUpperInvariant(), string.Empty);
        }

        var mnemonic = line[..separatorIndex].Trim().ToUpperInvariant();
        var operand = line[separatorIndex..].Trim();

        while (operand.Length > 0 && char.IsWhiteSpace(operand[0]))
        {
            operand = operand[1..];
        }

        return (mnemonic, operand);
    }

    private static void RequireNoOperand(string mnemonic, string operand)
    {
        if (operand.Length != 0)
        {
            throw new FormatException($"Instruction '{mnemonic}' does not take an operand.");
        }
    }

    private static void RequireOperand(string mnemonic, string operand, char requiredPrefix = '\0')
    {
        if (operand.Length == 0)
        {
            throw new FormatException($"Instruction '{mnemonic}' requires an operand.");
        }

        if (requiredPrefix != '\0' && operand[0] != requiredPrefix)
        {
            throw new FormatException($"Instruction '{mnemonic}' requires an operand starting with '{requiredPrefix}'.");
        }
    }

    private static byte ParseByteLiteral(string literal, string mnemonic)
    {
        var value = ParseUnsignedLiteral(literal, 0xFF, mnemonic);
        return (byte)value;
    }

    private static IEnumerable<byte> ParseWordLiteral(string literal, string mnemonic)
    {
        var value = ParseUnsignedLiteral(literal, 0xFFFF, mnemonic);
        yield return (byte)(value & 0xFF);
        yield return (byte)(value >> 8);
    }

    private static ushort ParseUnsignedLiteral(string literal, ushort maxValue, string mnemonic)
    {
        var trimmed = literal.Trim();

        if (trimmed.StartsWith("$", StringComparison.Ordinal))
        {
            trimmed = trimmed[1..];
        }
        else if (trimmed.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            trimmed = trimmed[2..];
        }

        if (trimmed.Length == 0 || !ushort.TryParse(trimmed, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var value))
        {
            throw new FormatException($"Instruction '{mnemonic}' contains an invalid hexadecimal literal '{literal}'.");
        }

        if (value > maxValue)
        {
            throw new OverflowException($"Instruction '{mnemonic}' literal '{literal}' does not fit in the expected size.");
        }

        return value;
    }
}
