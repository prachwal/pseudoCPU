namespace pseudoCPU.Core;

public sealed class BootstrapCpu
{
    private readonly byte[] _memory = new byte[ushort.MaxValue + 1];

    public byte A { get; private set; }

    public byte X { get; private set; }

    public ushort PC { get; private set; }

    public bool Zero { get; private set; }

    public bool Negative { get; private set; }

    public bool IsHalted { get; private set; }

    public void LoadProgram(ReadOnlySpan<byte> program, ushort startAddress = 0x0000)
    {
        if (program.Length > _memory.Length - startAddress)
        {
            throw new ArgumentOutOfRangeException(nameof(program), "Program does not fit in memory.");
        }

        Array.Clear(_memory);
        program.CopyTo(_memory.AsSpan(startAddress));

        A = 0;
        X = 0;
        PC = startAddress;
        Zero = false;
        Negative = false;
        IsHalted = false;
    }

    public void Run(int maxSteps = 10_000)
    {
        if (maxSteps <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxSteps));
        }

        for (var step = 0; step < maxSteps && !IsHalted; step++)
        {
            Step();
        }

        if (!IsHalted)
        {
            throw new InvalidOperationException("Program did not halt within the configured step limit.");
        }
    }

    public void Step()
    {
        if (IsHalted)
        {
            return;
        }

        var opcode = FetchByte();

        switch (BootstrapOpcodeDecoder.Decode(opcode))
        {
            case BootstrapOpcode.LdaImmediate:
                A = FetchByte();
                UpdateZeroAndNegative(A);
                break;
            case BootstrapOpcode.Tax:
                X = A;
                UpdateZeroAndNegative(X);
                break;
            case BootstrapOpcode.Inx:
                X++;
                UpdateZeroAndNegative(X);
                break;
            case BootstrapOpcode.StaAbsolute:
            {
                var lowByte = FetchByte();
                var highByte = FetchByte();
                var address = (ushort)(lowByte | (highByte << 8));
                _memory[address] = A;
                break;
            }
            case BootstrapOpcode.Brk:
                IsHalted = true;
                break;
            default:
                throw new InvalidOperationException($"Unsupported bootstrap opcode '{opcode:X2}'.");
        }
    }

    public byte ReadByte(ushort address) => _memory[address];

    public void WriteByte(ushort address, byte value) => _memory[address] = value;

    private byte FetchByte() => _memory[PC++];

    private void UpdateZeroAndNegative(byte value)
    {
        Zero = value == 0;
        Negative = (value & 0x80) != 0;
    }
}
