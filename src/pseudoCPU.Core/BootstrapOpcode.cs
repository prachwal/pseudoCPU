namespace pseudoCPU.Core;

public enum BootstrapOpcode : byte
{
    Php = 0x08,
    Plp = 0x28,
    Pha = 0x48,
    Pla = 0x68,
    JsrAbsolute = 0x20,
    Rts = 0x60,
    LdaImmediate = 0xA9,
    LdyImmediate = 0xA0,
    LdxImmediate = 0xA2,
    Iny = 0xC8,
    Dey = 0x88,
    Tax = 0xAA,
    Inx = 0xE8,
    Dex = 0xCA,
    StaAbsolute = 0x8D,
    StxAbsolute = 0x8E,
    CmpImmediate = 0xC9,
    CpxImmediate = 0xE0,
    JmpAbsolute = 0x4C,
    BeqRelative = 0xF0,
    BneRelative = 0xD0,
    Brk = 0x00,
}

public static class BootstrapOpcodeDecoder
{
    public static BootstrapOpcode Decode(byte opcode) => opcode switch
    {
        0x08 => BootstrapOpcode.Php,
        0x28 => BootstrapOpcode.Plp,
        0x48 => BootstrapOpcode.Pha,
        0x68 => BootstrapOpcode.Pla,
        0x20 => BootstrapOpcode.JsrAbsolute,
        0x60 => BootstrapOpcode.Rts,
        0xA9 => BootstrapOpcode.LdaImmediate,
        0xA0 => BootstrapOpcode.LdyImmediate,
        0xA2 => BootstrapOpcode.LdxImmediate,
        0xC8 => BootstrapOpcode.Iny,
        0x88 => BootstrapOpcode.Dey,
        0xAA => BootstrapOpcode.Tax,
        0xE8 => BootstrapOpcode.Inx,
        0xCA => BootstrapOpcode.Dex,
        0x8D => BootstrapOpcode.StaAbsolute,
        0x8E => BootstrapOpcode.StxAbsolute,
        0xC9 => BootstrapOpcode.CmpImmediate,
        0xE0 => BootstrapOpcode.CpxImmediate,
        0x4C => BootstrapOpcode.JmpAbsolute,
        0xF0 => BootstrapOpcode.BeqRelative,
        0xD0 => BootstrapOpcode.BneRelative,
        0x00 => BootstrapOpcode.Brk,
        _ => throw new NotSupportedException($"Unsupported bootstrap opcode '0x{opcode:X2}'."),
    };
}
