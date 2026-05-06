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
                case "PHP":
                    RequireNoOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.Php);
                    break;
                case "CLC":
                    RequireNoOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.Clc);
                    break;
                case "SEC":
                    RequireNoOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.Sec);
                    break;
                case "CLI":
                    RequireNoOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.Cli);
                    break;
                case "SEI":
                    RequireNoOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.Sei);
                    break;
                case "CLD":
                    RequireNoOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.Cld);
                    break;
                case "SED":
                    RequireNoOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.Sed);
                    break;
                case "CLV":
                    RequireNoOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.Clv);
                    break;
                case "PLP":
                    RequireNoOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.Plp);
                    break;
                case "PHA":
                    RequireNoOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.Pha);
                    break;
                case "PLA":
                    RequireNoOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.Pla);
                    break;
                case "ASL":
                    RequireNoOperandOrAccumulator(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.AslAccumulator);
                    break;
                case "LSR":
                    RequireNoOperandOrAccumulator(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.LsrAccumulator);
                    break;
                case "ROL":
                    RequireNoOperandOrAccumulator(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.RolAccumulator);
                    break;
                case "ROR":
                    RequireNoOperandOrAccumulator(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.RorAccumulator);
                    break;
                case "JSR":
                    RequireOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.JsrAbsolute);
                    bytes.AddRange(ParseWordLiteral(operand, mnemonic));
                    break;
                case "RTS":
                    RequireNoOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.Rts);
                    break;
                case "LDA":
                    bytes.AddRange(AssembleLda(operand));
                    break;
                case "LDX":
                    bytes.AddRange(AssembleLdx(operand));
                    break;
                case "LDY":
                    bytes.AddRange(AssembleLdy(operand));
                    break;
                case "TAX":
                    RequireNoOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.Tax);
                    break;
                case "INY":
                    RequireNoOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.Iny);
                    break;
                case "INX":
                    RequireNoOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.Inx);
                    break;
                case "DEY":
                    RequireNoOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.Dey);
                    break;
                case "DEX":
                    RequireNoOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.Dex);
                    break;
                case "STA":
                    bytes.AddRange(AssembleSta(operand));
                    break;
                case "STX":
                    bytes.AddRange(AssembleStx(operand));
                    break;
                case "STY":
                    bytes.AddRange(AssembleSty(operand));
                    break;
                case "CMP":
                    RequireOperand(mnemonic, operand, '#');
                    bytes.Add((byte)BootstrapOpcode.CmpImmediate);
                    bytes.Add(ParseByteLiteral(operand[1..], mnemonic));
                    break;
                case "ADC":
                    RequireOperand(mnemonic, operand, '#');
                    bytes.Add((byte)BootstrapOpcode.AdcImmediate);
                    bytes.Add(ParseByteLiteral(operand[1..], mnemonic));
                    break;
                case "SBC":
                    RequireOperand(mnemonic, operand, '#');
                    bytes.Add((byte)BootstrapOpcode.SbcImmediate);
                    bytes.Add(ParseByteLiteral(operand[1..], mnemonic));
                    break;
                case "CPY":
                    RequireOperand(mnemonic, operand, '#');
                    bytes.Add((byte)BootstrapOpcode.CpyImmediate);
                    bytes.Add(ParseByteLiteral(operand[1..], mnemonic));
                    break;
                case "CPX":
                    RequireOperand(mnemonic, operand, '#');
                    bytes.Add((byte)BootstrapOpcode.CpxImmediate);
                    bytes.Add(ParseByteLiteral(operand[1..], mnemonic));
                    break;
                case "JMP":
                    bytes.AddRange(AssembleJmp(operand));
                    break;
                case "BEQ":
                    RequireOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.BeqRelative);
                    bytes.Add(unchecked((byte)ParseRelativeOffset(operand, mnemonic)));
                    break;
                case "BNE":
                    RequireOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.BneRelative);
                    bytes.Add(unchecked((byte)ParseRelativeOffset(operand, mnemonic)));
                    break;
                case "BPL":
                    RequireOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.BplRelative);
                    bytes.Add(unchecked((byte)ParseRelativeOffset(operand, mnemonic)));
                    break;
                case "BMI":
                    RequireOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.BmiRelative);
                    bytes.Add(unchecked((byte)ParseRelativeOffset(operand, mnemonic)));
                    break;
                case "BVC":
                    RequireOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.BvcRelative);
                    bytes.Add(unchecked((byte)ParseRelativeOffset(operand, mnemonic)));
                    break;
                case "BVS":
                    RequireOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.BvsRelative);
                    bytes.Add(unchecked((byte)ParseRelativeOffset(operand, mnemonic)));
                    break;
                case "BCC":
                    RequireOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.BccRelative);
                    bytes.Add(unchecked((byte)ParseRelativeOffset(operand, mnemonic)));
                    break;
                case "BCS":
                    RequireOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.BcsRelative);
                    bytes.Add(unchecked((byte)ParseRelativeOffset(operand, mnemonic)));
                    break;
                case "BIT":
                    RequireOperand(mnemonic, operand);
                    bytes.Add((byte)BootstrapOpcode.BitZeroPage);
                    bytes.Add(ParseByteLiteral(operand, mnemonic));
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

    private static void RequireNoOperandOrAccumulator(string mnemonic, string operand)
    {
        if (operand.Length == 0 || string.Equals(operand, "A", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        throw new FormatException($"Instruction '{mnemonic}' does not take an operand.");
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

    private static List<byte> AssembleLda(string operand)
    {
        if (operand.StartsWith("#", StringComparison.Ordinal))
        {
            return AssembleImmediate(operand, "LDA", BootstrapOpcode.LdaImmediate);
        }

        if (TryAssembleParenthesizedIndexedOperand("LDA", operand, BootstrapOpcode.LdaIndexedIndirect, BootstrapOpcode.LdaIndirectIndexed, out var bytes))
        {
            return bytes;
        }

        if (TryAssembleIndexedOperand("LDA", operand, 'X', BootstrapOpcode.LdaZeroPageX, BootstrapOpcode.LdaAbsoluteX, out bytes))
        {
            return bytes;
        }

        if (TryAssembleWordIndexedOperand("LDA", operand, 'Y', BootstrapOpcode.LdaAbsoluteY, out bytes))
        {
            return bytes;
        }

        if (TryAssembleZeroPageOperand("LDA", operand, BootstrapOpcode.LdaZeroPage, out bytes))
        {
            return bytes;
        }

        throw new NotSupportedException($"Unsupported bootstrap addressing mode for mnemonic 'LDA' with operand '{operand}'.");
    }

    private static List<byte> AssembleLdx(string operand)
    {
        if (operand.StartsWith("#", StringComparison.Ordinal))
        {
            return AssembleImmediate(operand, "LDX", BootstrapOpcode.LdxImmediate);
        }

        if (TryAssembleIndexedOperand("LDX", operand, 'Y', BootstrapOpcode.LdxZeroPageY, BootstrapOpcode.LdxAbsoluteY, out var bytes))
        {
            return bytes;
        }

        if (TryAssembleZeroPageOperand("LDX", operand, BootstrapOpcode.LdxZeroPage, out bytes))
        {
            return bytes;
        }

        throw new NotSupportedException($"Unsupported bootstrap addressing mode for mnemonic 'LDX' with operand '{operand}'.");
    }

    private static List<byte> AssembleLdy(string operand)
    {
        if (operand.StartsWith("#", StringComparison.Ordinal))
        {
            return AssembleImmediate(operand, "LDY", BootstrapOpcode.LdyImmediate);
        }

        if (TryAssembleIndexedOperand("LDY", operand, 'X', BootstrapOpcode.LdyZeroPageX, BootstrapOpcode.LdyAbsoluteX, out var bytes))
        {
            return bytes;
        }

        if (TryAssembleZeroPageOperand("LDY", operand, BootstrapOpcode.LdyZeroPage, out bytes))
        {
            return bytes;
        }

        throw new NotSupportedException($"Unsupported bootstrap addressing mode for mnemonic 'LDY' with operand '{operand}'.");
    }

    private static List<byte> AssembleSta(string operand)
    {
        if (TryAssembleParenthesizedIndexedOperand("STA", operand, BootstrapOpcode.StaIndexedIndirect, BootstrapOpcode.StaIndirectIndexed, out var bytes))
        {
            return bytes;
        }

        if (TryAssembleIndexedOperand("STA", operand, 'X', BootstrapOpcode.StaZeroPageX, BootstrapOpcode.StaAbsoluteX, out bytes))
        {
            return bytes;
        }

        if (TryAssembleWordIndexedOperand("STA", operand, 'Y', BootstrapOpcode.StaAbsoluteY, out bytes))
        {
            return bytes;
        }

        if (TryAssembleZeroPageOperand("STA", operand, BootstrapOpcode.StaZeroPage, out bytes))
        {
            return bytes;
        }

        if (TryAssembleWordOperand("STA", operand, BootstrapOpcode.StaAbsolute, out bytes))
        {
            return bytes;
        }

        throw new NotSupportedException($"Unsupported bootstrap addressing mode for mnemonic 'STA' with operand '{operand}'.");
    }

    private static List<byte> AssembleStx(string operand)
    {
        if (TryAssembleZeroPageIndexedOperand("STX", operand, 'Y', BootstrapOpcode.StxZeroPageY, out var bytes))
        {
            return bytes;
        }

        if (TryAssembleZeroPageOperand("STX", operand, BootstrapOpcode.StxZeroPage, out bytes))
        {
            return bytes;
        }

        if (TryAssembleWordOperand("STX", operand, BootstrapOpcode.StxAbsolute, out bytes))
        {
            return bytes;
        }

        throw new NotSupportedException($"Unsupported bootstrap addressing mode for mnemonic 'STX' with operand '{operand}'.");
    }

    private static List<byte> AssembleSty(string operand)
    {
        if (TryAssembleZeroPageIndexedOperand("STY", operand, 'X', BootstrapOpcode.StyZeroPageX, out var bytes))
        {
            return bytes;
        }

        if (TryAssembleZeroPageOperand("STY", operand, BootstrapOpcode.StyZeroPage, out bytes))
        {
            return bytes;
        }

        if (TryAssembleWordOperand("STY", operand, BootstrapOpcode.StyAbsolute, out bytes))
        {
            return bytes;
        }

        throw new NotSupportedException($"Unsupported bootstrap addressing mode for mnemonic 'STY' with operand '{operand}'.");
    }

    private static List<byte> AssembleJmp(string operand)
    {
        if (TryAssembleParenthesizedWordOperand("JMP", operand, BootstrapOpcode.JmpIndirect, out var bytes))
        {
            return bytes;
        }

        if (TryAssembleWordOperand("JMP", operand, BootstrapOpcode.JmpAbsolute, out bytes))
        {
            return bytes;
        }

        throw new NotSupportedException($"Unsupported bootstrap addressing mode for mnemonic 'JMP' with operand '{operand}'.");
    }

    private static List<byte> AssembleImmediate(string operand, string mnemonic, BootstrapOpcode opcode)
    {
        var bytes = new List<byte> { (byte)opcode, ParseByteLiteral(operand[1..], mnemonic) };
        return bytes;
    }

    private static bool TryAssembleZeroPageOperand(string mnemonic, string operand, BootstrapOpcode zeroPageOpcode, out List<byte> bytes)
    {
        if (IsWordLiteral(operand))
        {
            bytes = [];
            return false;
        }

        bytes = [(byte)zeroPageOpcode, ParseByteLiteral(operand, mnemonic)];
        return true;
    }

    private static bool TryAssembleWordOperand(string mnemonic, string operand, BootstrapOpcode wordOpcode, out List<byte> bytes)
    {
        if (!IsWordLiteral(operand))
        {
            bytes = [];
            return false;
        }

        bytes = [(byte)wordOpcode];
        bytes.AddRange(ParseWordLiteral(operand, mnemonic));
        return true;
    }

    private static bool TryAssembleIndexedOperand(string mnemonic, string operand, char indexRegister, BootstrapOpcode zeroPageOpcode, BootstrapOpcode wordOpcode, out List<byte> bytes)
    {
        var suffix = $",{indexRegister}";

        if (!operand.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
        {
            bytes = [];
            return false;
        }

        var addressOperand = operand[..^suffix.Length];

        if (IsWordLiteral(addressOperand))
        {
            bytes = [(byte)wordOpcode];
            bytes.AddRange(ParseWordLiteral(addressOperand, mnemonic));
            return true;
        }

        bytes = [(byte)zeroPageOpcode, ParseByteLiteral(addressOperand, mnemonic)];
        return true;
    }

    private static bool TryAssembleZeroPageIndexedOperand(string mnemonic, string operand, char indexRegister, BootstrapOpcode zeroPageOpcode, out List<byte> bytes)
    {
        var suffix = $",{indexRegister}";

        if (!operand.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
        {
            bytes = [];
            return false;
        }

        var addressOperand = operand[..^suffix.Length];

        if (IsWordLiteral(addressOperand))
        {
            bytes = [];
            return false;
        }

        bytes = [(byte)zeroPageOpcode, ParseByteLiteral(addressOperand, mnemonic)];
        return true;
    }

    private static bool TryAssembleWordIndexedOperand(string mnemonic, string operand, char indexRegister, BootstrapOpcode wordOpcode, out List<byte> bytes)
    {
        var suffix = $",{indexRegister}";

        if (!operand.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
        {
            bytes = [];
            return false;
        }

        var addressOperand = operand[..^suffix.Length];

        if (!IsWordLiteral(addressOperand))
        {
            bytes = [];
            return false;
        }

        bytes = [(byte)wordOpcode];
        bytes.AddRange(ParseWordLiteral(addressOperand, mnemonic));
        return true;
    }

    private static bool TryAssembleParenthesizedIndexedOperand(string mnemonic, string operand, BootstrapOpcode indexedIndirectOpcode, BootstrapOpcode indirectIndexedOpcode, out List<byte> bytes)
    {
        if (operand.StartsWith("(", StringComparison.Ordinal) && operand.EndsWith(",X)", StringComparison.OrdinalIgnoreCase))
        {
            var addressOperand = operand[1..^3];
            bytes = [(byte)indexedIndirectOpcode, ParseByteLiteral(addressOperand, mnemonic)];
            return true;
        }

        if (operand.StartsWith("(", StringComparison.Ordinal) && operand.EndsWith("),Y", StringComparison.OrdinalIgnoreCase))
        {
            var addressOperand = operand[1..^3];
            bytes = [(byte)indirectIndexedOpcode, ParseByteLiteral(addressOperand, mnemonic)];
            return true;
        }

        bytes = [];
        return false;
    }

    private static bool TryAssembleParenthesizedWordOperand(string mnemonic, string operand, BootstrapOpcode opcode, out List<byte> bytes)
    {
        if (!operand.StartsWith("(", StringComparison.Ordinal) || !operand.EndsWith(")", StringComparison.Ordinal))
        {
            bytes = [];
            return false;
        }

        var addressOperand = operand[1..^1];
        bytes = [(byte)opcode];
        bytes.AddRange(ParseWordLiteral(addressOperand, mnemonic));
        return true;
    }

    private static bool IsWordLiteral(string literal)
    {
        var digits = GetLiteralDigits(literal);
        return digits.Length > 2;
    }

    private static string GetLiteralDigits(string literal)
    {
        var trimmed = literal.Trim();

        if (trimmed.StartsWith("$", StringComparison.Ordinal))
        {
            return trimmed[1..];
        }

        if (trimmed.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            return trimmed[2..];
        }

        return trimmed;
    }

    private static sbyte ParseRelativeOffset(string literal, string mnemonic)
    {
        var trimmed = literal.Trim();

        if (trimmed.Length == 0)
        {
            throw new FormatException($"Instruction '{mnemonic}' contains an invalid relative offset literal '{literal}'.");
        }

        if (trimmed.StartsWith("$", StringComparison.Ordinal))
        {
            trimmed = trimmed[1..];
        }
        else if (trimmed.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            trimmed = trimmed[2..];
        }

        if (!sbyte.TryParse(trimmed, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var signedValue))
        {
            throw new FormatException($"Instruction '{mnemonic}' contains an invalid relative offset literal '{literal}'.");
        }

        return signedValue;
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
