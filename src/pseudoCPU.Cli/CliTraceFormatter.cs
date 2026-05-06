using pseudoCPU.Core;

namespace pseudoCPU.Cli;

internal static class CliTraceFormatter
{
    public static string FormatStep(int stepNumber, ushort pc, byte opcode, BootstrapCpu cpu)
    {
        var mnemonic = DescribeInstruction(pc, opcode, cpu);
        return $"#{stepNumber:D4} PC=0x{pc:X4} OPC=0x{opcode:X2} {mnemonic} A=0x{cpu.A:X2} X=0x{cpu.X:X2} Y=0x{cpu.Y:X2} P=0x{cpu.Status.ToByte():X2} N={cpu.Negative.ToString().ToLowerInvariant()} V={cpu.Status.Overflow.ToString().ToLowerInvariant()} B={cpu.Status.Break.ToString().ToLowerInvariant()} D={cpu.Status.Decimal.ToString().ToLowerInvariant()} I={cpu.Status.InterruptDisable.ToString().ToLowerInvariant()} Z={cpu.Zero.ToString().ToLowerInvariant()} C={cpu.Carry.ToString().ToLowerInvariant()}";
    }

    private static string DescribeInstruction(ushort pc, byte opcode, BootstrapCpu cpu)
    {
        return opcode switch
        {
            (byte)BootstrapOpcode.Php => "PHP",
            (byte)BootstrapOpcode.Plp => "PLP",
            (byte)BootstrapOpcode.Pha => "PHA",
            (byte)BootstrapOpcode.Pla => "PLA",
            (byte)BootstrapOpcode.Clc => "CLC",
            (byte)BootstrapOpcode.Sec => "SEC",
            (byte)BootstrapOpcode.Cli => "CLI",
            (byte)BootstrapOpcode.Sei => "SEI",
            (byte)BootstrapOpcode.Cld => "CLD",
            (byte)BootstrapOpcode.Sed => "SED",
            (byte)BootstrapOpcode.Clv => "CLV",
            (byte)BootstrapOpcode.AslAccumulator => "ASL A",
            (byte)BootstrapOpcode.LsrAccumulator => "LSR A",
            (byte)BootstrapOpcode.RolAccumulator => "ROL A",
            (byte)BootstrapOpcode.RorAccumulator => "ROR A",
            (byte)BootstrapOpcode.JsrAbsolute => $"JSR ${ReadWord(cpu, (ushort)(pc + 1)):X4}",
            (byte)BootstrapOpcode.Rts => "RTS",
            (byte)BootstrapOpcode.LdaImmediate => $"LDA #${cpu.ReadByte((ushort)(pc + 1)):X2}",
            (byte)BootstrapOpcode.AdcImmediate => $"ADC #${cpu.ReadByte((ushort)(pc + 1)):X2}",
            (byte)BootstrapOpcode.LdyImmediate => $"LDY #${cpu.ReadByte((ushort)(pc + 1)):X2}",
            (byte)BootstrapOpcode.LdxImmediate => $"LDX #${cpu.ReadByte((ushort)(pc + 1)):X2}",
            (byte)BootstrapOpcode.CpyImmediate => $"CPY #${cpu.ReadByte((ushort)(pc + 1)):X2}",
            (byte)BootstrapOpcode.SbcImmediate => $"SBC #${cpu.ReadByte((ushort)(pc + 1)):X2}",
            (byte)BootstrapOpcode.Tax => "TAX",
            (byte)BootstrapOpcode.Inx => "INX",
            (byte)BootstrapOpcode.Iny => "INY",
            (byte)BootstrapOpcode.Dex => "DEX",
            (byte)BootstrapOpcode.Dey => "DEY",
            (byte)BootstrapOpcode.StaAbsolute => $"STA ${ReadWord(cpu, (ushort)(pc + 1)):X4}",
            (byte)BootstrapOpcode.StyAbsolute => $"STY ${ReadWord(cpu, (ushort)(pc + 1)):X4}",
            (byte)BootstrapOpcode.StxAbsolute => $"STX ${ReadWord(cpu, (ushort)(pc + 1)):X4}",
            (byte)BootstrapOpcode.CmpImmediate => $"CMP #${cpu.ReadByte((ushort)(pc + 1)):X2}",
            (byte)BootstrapOpcode.CpxImmediate => $"CPX #${cpu.ReadByte((ushort)(pc + 1)):X2}",
            (byte)BootstrapOpcode.BitZeroPage => $"BIT ${cpu.ReadByte((ushort)(pc + 1)):X2}",
            (byte)BootstrapOpcode.JmpAbsolute => $"JMP ${ReadWord(cpu, (ushort)(pc + 1)):X4}",
            (byte)BootstrapOpcode.BplRelative => $"BPL {FormatRelativeOffset(cpu.ReadByte((ushort)(pc + 1)))}",
            (byte)BootstrapOpcode.BmiRelative => $"BMI {FormatRelativeOffset(cpu.ReadByte((ushort)(pc + 1)))}",
            (byte)BootstrapOpcode.BvcRelative => $"BVC {FormatRelativeOffset(cpu.ReadByte((ushort)(pc + 1)))}",
            (byte)BootstrapOpcode.BvsRelative => $"BVS {FormatRelativeOffset(cpu.ReadByte((ushort)(pc + 1)))}",
            (byte)BootstrapOpcode.BccRelative => $"BCC {FormatRelativeOffset(cpu.ReadByte((ushort)(pc + 1)))}",
            (byte)BootstrapOpcode.BcsRelative => $"BCS {FormatRelativeOffset(cpu.ReadByte((ushort)(pc + 1)))}",
            (byte)BootstrapOpcode.BeqRelative => $"BEQ {FormatRelativeOffset(cpu.ReadByte((ushort)(pc + 1)))}",
            (byte)BootstrapOpcode.BneRelative => $"BNE {FormatRelativeOffset(cpu.ReadByte((ushort)(pc + 1)))}",
            (byte)BootstrapOpcode.Brk => "BRK",
            _ => $"DB 0x{opcode:X2}",
        };
    }

    private static ushort ReadWord(BootstrapCpu cpu, ushort address)
    {
        var lowByte = cpu.ReadByte(address);
        var highByte = cpu.ReadByte((ushort)(address + 1));
        return (ushort)(lowByte | (highByte << 8));
    }

    private static string FormatRelativeOffset(byte offset) => $"${offset:X2}";
}
