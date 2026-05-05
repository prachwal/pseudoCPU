namespace pseudoCPU.Core;

public enum BootstrapOpcode : byte
{
    LdaImmediate = 0xA9,
    Tax = 0xAA,
    Inx = 0xE8,
    StaAbsolute = 0x8D,
    CmpImmediate = 0xC9,
    JmpAbsolute = 0x4C,
    BeqRelative = 0xF0,
    BneRelative = 0xD0,
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
        0xC9 => BootstrapOpcode.CmpImmediate,
        0x4C => BootstrapOpcode.JmpAbsolute,
        0xF0 => BootstrapOpcode.BeqRelative,
        0xD0 => BootstrapOpcode.BneRelative,
        0x00 => BootstrapOpcode.Brk,
        _ => throw new NotSupportedException($"Unsupported bootstrap opcode '0x{opcode:X2}'."),
    };
}
