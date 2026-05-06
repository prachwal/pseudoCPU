using pseudoCPU.Core;

namespace pseudoCPU.Bootstrap.Tests;

public class BootstrapOpcodeDecoderTests
{
    [Trait("Category", "OpcodeDecoder")]
    [Theory]
    [InlineData(0x24, BootstrapOpcode.BitZeroPage)]
    [InlineData(0x18, BootstrapOpcode.Clc)]
    [InlineData(0x08, BootstrapOpcode.Php)]
    [InlineData(0x28, BootstrapOpcode.Plp)]
    [InlineData(0x48, BootstrapOpcode.Pha)]
    [InlineData(0x68, BootstrapOpcode.Pla)]
    [InlineData(0x0A, BootstrapOpcode.AslAccumulator)]
    [InlineData(0x38, BootstrapOpcode.Sec)]
    [InlineData(0x58, BootstrapOpcode.Cli)]
    [InlineData(0x78, BootstrapOpcode.Sei)]
    [InlineData(0x4A, BootstrapOpcode.LsrAccumulator)]
    [InlineData(0x2A, BootstrapOpcode.RolAccumulator)]
    [InlineData(0x20, BootstrapOpcode.JsrAbsolute)]
    [InlineData(0x60, BootstrapOpcode.Rts)]
    [InlineData(0x6A, BootstrapOpcode.RorAccumulator)]
    [InlineData(0xA9, BootstrapOpcode.LdaImmediate)]
    [InlineData(0x69, BootstrapOpcode.AdcImmediate)]
    [InlineData(0xA0, BootstrapOpcode.LdyImmediate)]
    [InlineData(0xA2, BootstrapOpcode.LdxImmediate)]
    [InlineData(0xC0, BootstrapOpcode.CpyImmediate)]
    [InlineData(0xC8, BootstrapOpcode.Iny)]
    [InlineData(0x88, BootstrapOpcode.Dey)]
    [InlineData(0xAA, BootstrapOpcode.Tax)]
    [InlineData(0xE8, BootstrapOpcode.Inx)]
    [InlineData(0xCA, BootstrapOpcode.Dex)]
    [InlineData(0x8D, BootstrapOpcode.StaAbsolute)]
    [InlineData(0x8C, BootstrapOpcode.StyAbsolute)]
    [InlineData(0x8E, BootstrapOpcode.StxAbsolute)]
    [InlineData(0xC9, BootstrapOpcode.CmpImmediate)]
    [InlineData(0xE0, BootstrapOpcode.CpxImmediate)]
    [InlineData(0x4C, BootstrapOpcode.JmpAbsolute)]
    [InlineData(0x10, BootstrapOpcode.BplRelative)]
    [InlineData(0x30, BootstrapOpcode.BmiRelative)]
    [InlineData(0x50, BootstrapOpcode.BvcRelative)]
    [InlineData(0x70, BootstrapOpcode.BvsRelative)]
    [InlineData(0x90, BootstrapOpcode.BccRelative)]
    [InlineData(0xB0, BootstrapOpcode.BcsRelative)]
    [InlineData(0xF0, BootstrapOpcode.BeqRelative)]
    [InlineData(0xD0, BootstrapOpcode.BneRelative)]
    [InlineData(0xE9, BootstrapOpcode.SbcImmediate)]
    [InlineData(0xD8, BootstrapOpcode.Cld)]
    [InlineData(0xB8, BootstrapOpcode.Clv)]
    [InlineData(0xF8, BootstrapOpcode.Sed)]
    [InlineData(0x00, BootstrapOpcode.Brk)]
    public void DecodesBootstrapSliceOpcodes(byte opcode, BootstrapOpcode expected)
    {
        Assert.Equal(expected, BootstrapOpcodeDecoder.Decode(opcode));
    }
}
