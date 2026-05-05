namespace pseudoCPU.Core;

public enum BootstrapOpcode : byte
{
    LdaImmediate = 0xA9,
    Tax = 0xAA,
    Inx = 0xE8,
    StaAbsolute = 0x8D,
    Brk = 0x00,
}

public static class BootstrapOpcodeDecoder
{
    public static BootstrapOpcode Decode(byte opcode) => opcode switch
    {
        0xA9 => BootstrapOpcode.LdaImmediate,
        0xAA => BootstrapOpcode.Tax,
        0xE8 => BootstrapOpcode.Inx,
        0x8D => BootstrapOpcode.StaAbsolute,
        0x00 => BootstrapOpcode.Brk,
        _ => throw new NotSupportedException($"Unsupported bootstrap opcode '0x{opcode:X2}'."),
    };
}
