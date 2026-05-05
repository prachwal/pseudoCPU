using pseudoCPU.Core;

namespace pseudoCPU.Bootstrap.Tests;

public class BootstrapOpcodeDecoderTests
{
    [Trait("Category", "OpcodeDecoder")]
    [Theory]
    [InlineData(0xA9, BootstrapOpcode.LdaImmediate)]
    [InlineData(0xAA, BootstrapOpcode.Tax)]
    [InlineData(0xE8, BootstrapOpcode.Inx)]
    [InlineData(0x8D, BootstrapOpcode.StaAbsolute)]
    [InlineData(0x00, BootstrapOpcode.Brk)]
    public void DecodesBootstrapSliceOpcodes(byte opcode, BootstrapOpcode expected)
    {
        Assert.Equal(expected, BootstrapOpcodeDecoder.Decode(opcode));
    }
}
