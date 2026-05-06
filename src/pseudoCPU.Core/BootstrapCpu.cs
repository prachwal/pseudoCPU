namespace pseudoCPU.Core;

public sealed class BootstrapCpu
{
    private const ushort StackPageBaseAddress = 0x0100;
    private const byte CarryStatusBit = 1 << 0;
    private const byte ZeroStatusBit = 1 << 1;
    private const byte NegativeStatusBit = 1 << 7;
    private readonly byte[] _memory = new byte[ushort.MaxValue + 1];

    public byte A { get; private set; }

    public byte X { get; private set; }

    public byte Y { get; private set; }

    public ushort PC { get; private set; }

    public byte SP { get; private set; }

    public bool Zero { get; private set; }

    public bool Negative { get; private set; }

    public bool Carry { get; private set; }

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
        Y = 0;
        PC = startAddress;
        SP = 0xFF;
        Zero = false;
        Negative = false;
        Carry = false;
        IsHalted = false;
    }

    public void Run(int maxSteps = 10_000)
    {
        RunUntilHalt(maxSteps);
    }

    public int RunSteps(int stepCount)
    {
        if (stepCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stepCount));
        }

        var executedSteps = 0;
        for (; executedSteps < stepCount && !IsHalted; executedSteps++)
        {
            Step();
        }

        return executedSteps;
    }

    public int RunUntilHalt(int maxSteps = 10_000)
    {
        if (maxSteps <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxSteps));
        }

        var executedSteps = RunSteps(maxSteps);

        if (!IsHalted)
        {
            throw new BootstrapCpuStepLimitExceededException("Program did not halt within the configured step limit.");
        }

        return executedSteps;
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
            case BootstrapOpcode.Php:
                PushByte(CreateStatusSnapshot());
                break;
            case BootstrapOpcode.Plp:
                RestoreStatusSnapshot(PopByte());
                break;
            case BootstrapOpcode.Pha:
                PushByte(A);
                break;
            case BootstrapOpcode.Pla:
                A = PopByte();
                UpdateZeroAndNegative(A);
                break;
            case BootstrapOpcode.JsrAbsolute:
                {
                    var targetAddress = FetchWord();
                    var returnAddress = (ushort)(PC - 1);
                    PushByte((byte)(returnAddress >> 8));
                    PushByte((byte)(returnAddress & 0xFF));
                    PC = targetAddress;
                    break;
                }
            case BootstrapOpcode.Rts:
                {
                    var lowByte = PopByte();
                    var highByte = PopByte();
                    var returnAddress = (ushort)(lowByte | (highByte << 8));
                    PC = unchecked((ushort)(returnAddress + 1));
                    break;
                }
            case BootstrapOpcode.LdaImmediate:
                A = FetchByte();
                UpdateZeroAndNegative(A);
                break;
            case BootstrapOpcode.LdxImmediate:
                X = FetchByte();
                UpdateZeroAndNegative(X);
                break;
            case BootstrapOpcode.Tax:
                X = A;
                UpdateZeroAndNegative(X);
                break;
            case BootstrapOpcode.Inx:
                X++;
                UpdateZeroAndNegative(X);
                break;
            case BootstrapOpcode.Dex:
                X--;
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
            case BootstrapOpcode.StxAbsolute:
                {
                    var lowByte = FetchByte();
                    var highByte = FetchByte();
                    var address = (ushort)(lowByte | (highByte << 8));
                    _memory[address] = X;
                    break;
                }
            case BootstrapOpcode.CmpImmediate:
                CompareWithAccumulator(FetchByte());
                break;
            case BootstrapOpcode.CpxImmediate:
                CompareWithX(FetchByte());
                break;
            case BootstrapOpcode.JmpAbsolute:
                {
                    var lowByte = FetchByte();
                    var highByte = FetchByte();
                    PC = (ushort)(lowByte | (highByte << 8));
                    break;
                }
            case BootstrapOpcode.BeqRelative:
                BranchRelative(Zero, FetchByte());
                break;
            case BootstrapOpcode.BneRelative:
                BranchRelative(!Zero, FetchByte());
                break;
            case BootstrapOpcode.Brk:
                IsHalted = true;
                break;
            default:
                throw new InvalidOperationException($"Unsupported bootstrap opcode '{opcode:X2}'.");
        }
    }

    public byte ReadByte(ushort address) => _memory[address];

    public void WriteByte(ushort address, byte value) => _memory[address] = value;

    public void PushByte(byte value)
    {
        _memory[GetStackAddress(SP)] = value;
        SP--;
    }

    public byte PopByte()
    {
        SP++;
        return _memory[GetStackAddress(SP)];
    }

    private byte FetchByte() => _memory[PC++];

    private ushort FetchWord()
    {
        var lowByte = FetchByte();
        var highByte = FetchByte();
        return (ushort)(lowByte | (highByte << 8));
    }

    private void UpdateZeroAndNegative(byte value)
    {
        Zero = value == 0;
        Negative = (value & 0x80) != 0;
    }

    private void CompareWithAccumulator(byte value)
    {
        var result = unchecked((byte)(A - value));
        Zero = A == value;
        Negative = (result & 0x80) != 0;
        Carry = A >= value;
    }

    private void CompareWithX(byte value)
    {
        var result = unchecked((byte)(X - value));
        Zero = X == value;
        Negative = (result & 0x80) != 0;
        Carry = X >= value;
    }

    private void BranchRelative(bool condition, byte offsetByte)
    {
        if (!condition)
        {
            return;
        }

        var offset = unchecked((sbyte)offsetByte);
        PC = unchecked((ushort)(PC + offset));
    }

    private byte CreateStatusSnapshot()
    {
        byte status = 0;

        if (Carry)
        {
            status |= CarryStatusBit;
        }

        if (Zero)
        {
            status |= ZeroStatusBit;
        }

        if (Negative)
        {
            status |= NegativeStatusBit;
        }

        return status;
    }

    private void RestoreStatusSnapshot(byte status)
    {
        Carry = (status & CarryStatusBit) != 0;
        Zero = (status & ZeroStatusBit) != 0;
        Negative = (status & NegativeStatusBit) != 0;
    }

    private static ushort GetStackAddress(byte stackPointer) => (ushort)(StackPageBaseAddress | stackPointer);
}
