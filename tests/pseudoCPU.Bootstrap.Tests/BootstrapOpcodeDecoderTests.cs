using pseudoCPU.Core;

namespace pseudoCPU.Bootstrap.Tests;

public class BootstrapOpcodeDecoderTests
{
    [Trait("Category", "OpcodeDecoder")]
    [Theory]
    [InlineData(0xA9, BootstrapOpcode.LdaImmediate)]
    [InlineData(0xA2, BootstrapOpcode.LdxImmediate)]
    [InlineData(0xAA, BootstrapOpcode.Tax)]
    [InlineData(0xE8, BootstrapOpcode.Inx)]
    [InlineData(0xCA, BootstrapOpcode.Dex)]
    [InlineData(0x8D, BootstrapOpcode.StaAbsolute)]
    [InlineData(0x8E, BootstrapOpcode.StxAbsolute)]
    [InlineData(0xC9, BootstrapOpcode.CmpImmediate)]
    [InlineData(0xE0, BootstrapOpcode.CpxImmediate)]
    [InlineData(0x4C, BootstrapOpcode.JmpAbsolute)]
    [InlineData(0xF0, BootstrapOpcode.BeqRelative)]
    [InlineData(0xD0, BootstrapOpcode.BneRelative)]
    [InlineData(0x00, BootstrapOpcode.Brk)]
    public void DecodesBootstrapSliceOpcodes(byte opcode, BootstrapOpcode expected)
    {
        Assert.Equal(expected, BootstrapOpcodeDecoder.Decode(opcode));
    }
}
