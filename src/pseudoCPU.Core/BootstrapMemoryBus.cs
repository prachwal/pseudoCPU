namespace pseudoCPU.Core;

public sealed class BootstrapMemoryBus
{
    private readonly byte[] _ram = new byte[ushort.MaxValue + 1];

    public void LoadProgram(ReadOnlySpan<byte> program, ushort startAddress = 0x0000)
    {
        if (program.Length > _ram.Length - startAddress)
        {
            throw new ArgumentOutOfRangeException(nameof(program), "Program does not fit in memory.");
        }

        Array.Clear(_ram);
        program.CopyTo(_ram.AsSpan(startAddress));
    }

    public byte ReadByte(ushort address) => _ram[address];

    public void WriteByte(ushort address, byte value) => _ram[address] = value;

    public ushort ResolveZeroPageAddress(byte zeroPageAddress) => zeroPageAddress;

    public ushort ResolveZeroPageIndexedAddress(byte zeroPageAddress, byte index) => (ushort)(byte)(zeroPageAddress + index);

    public ushort ResolveAbsoluteIndexedAddress(ushort baseAddress, byte index) => unchecked((ushort)(baseAddress + index));
}
