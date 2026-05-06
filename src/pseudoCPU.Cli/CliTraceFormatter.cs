using pseudoCPU.Core;

namespace pseudoCPU.Cli;

internal static class CliTraceFormatter
{
    public static string FormatStep(int stepNumber, ushort pc, byte opcode, BootstrapCpu cpu)
    {
        var mnemonic = DescribeInstruction(pc, opcode, cpu);
        return $"#{stepNumber:D4} PC=0x{pc:X4} OPC=0x{opcode:X2} {mnemonic} A=0x{cpu.A:X2} X=0x{cpu.X:X2} Z={cpu.Zero.ToString().ToLowerInvariant()} N={cpu.Negative.ToString().ToLowerInvariant()} C={cpu.Carry.ToString().ToLowerInvariant()}";
    }

    private static string DescribeInstruction(ushort pc, byte opcode, BootstrapCpu cpu)
    {
        return opcode switch
        {
            (byte)BootstrapOpcode.JsrAbsolute => $"JSR ${ReadWord(cpu, (ushort)(pc + 1)):X4}",
            (byte)BootstrapOpcode.Rts => "RTS",
            (byte)BootstrapOpcode.LdaImmediate => $"LDA #${cpu.ReadByte((ushort)(pc + 1)):X2}",
            (byte)BootstrapOpcode.LdxImmediate => $"LDX #${cpu.ReadByte((ushort)(pc + 1)):X2}",
            (byte)BootstrapOpcode.Tax => "TAX",
            (byte)BootstrapOpcode.Inx => "INX",
            (byte)BootstrapOpcode.Dex => "DEX",
            (byte)BootstrapOpcode.StaAbsolute => $"STA ${ReadWord(cpu, (ushort)(pc + 1)):X4}",
            (byte)BootstrapOpcode.StxAbsolute => $"STX ${ReadWord(cpu, (ushort)(pc + 1)):X4}",
            (byte)BootstrapOpcode.CmpImmediate => $"CMP #${cpu.ReadByte((ushort)(pc + 1)):X2}",
            (byte)BootstrapOpcode.CpxImmediate => $"CPX #${cpu.ReadByte((ushort)(pc + 1)):X2}",
            (byte)BootstrapOpcode.JmpAbsolute => $"JMP ${ReadWord(cpu, (ushort)(pc + 1)):X4}",
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
