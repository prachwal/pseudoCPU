namespace pseudoCPU.Core;

public sealed class BootstrapCpu
{
    private const ushort StackPageBaseAddress = 0x0100;
    private readonly byte[] _memory = new byte[ushort.MaxValue + 1];

    public byte A { get; private set; }

    public byte X { get; private set; }

    public byte Y { get; private set; }

    public ushort PC { get; private set; }

    public byte SP { get; private set; }

    public BootstrapStatusRegister Status { get; private set; } = new();

    public bool Zero => Status.Zero;

    public bool Negative => Status.Negative;

    public bool Carry => Status.Carry;

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
        Status = new BootstrapStatusRegister();
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
            case BootstrapOpcode.Clc:
                Status.Carry = false;
                break;
            case BootstrapOpcode.Sec:
                Status.Carry = true;
                break;
            case BootstrapOpcode.Cli:
                Status.InterruptDisable = false;
                break;
            case BootstrapOpcode.Sei:
                Status.InterruptDisable = true;
                break;
            case BootstrapOpcode.Cld:
                Status.Decimal = false;
                break;
            case BootstrapOpcode.Sed:
                Status.Decimal = true;
                break;
            case BootstrapOpcode.Clv:
                Status.Overflow = false;
                break;
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
            case BootstrapOpcode.AdcImmediate:
                AddWithCarry(FetchByte());
                break;
            case BootstrapOpcode.LdyImmediate:
                Y = FetchByte();
                UpdateZeroAndNegative(Y);
                break;
            case BootstrapOpcode.LdxImmediate:
                X = FetchByte();
                UpdateZeroAndNegative(X);
                break;
            case BootstrapOpcode.Iny:
                Y++;
                UpdateZeroAndNegative(Y);
                break;
            case BootstrapOpcode.Dey:
                Y--;
                UpdateZeroAndNegative(Y);
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
            case BootstrapOpcode.StyAbsolute:
                {
                    var lowByte = FetchByte();
                    var highByte = FetchByte();
                    var address = (ushort)(lowByte | (highByte << 8));
                    _memory[address] = Y;
                    break;
                }
            case BootstrapOpcode.CmpImmediate:
                CompareWithAccumulator(FetchByte());
                break;
            case BootstrapOpcode.SbcImmediate:
                SubtractWithBorrow(FetchByte());
                break;
            case BootstrapOpcode.CpxImmediate:
                CompareWithX(FetchByte());
                break;
            case BootstrapOpcode.CpyImmediate:
                CompareWithY(FetchByte());
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
        Status.Zero = value == 0;
        Status.Negative = (value & 0x80) != 0;
    }

    private void AddWithCarry(byte value)
    {
        var carryIn = Status.Carry ? 1 : 0;
        var sum = A + value + carryIn;
        var result = (byte)sum;

        Status.Carry = sum > byte.MaxValue;
        Status.Overflow = (~(A ^ value) & (A ^ result) & 0x80) != 0;

        A = result;
        UpdateZeroAndNegative(A);
    }

    private void SubtractWithBorrow(byte value)
    {
        var borrow = Status.Carry ? 0 : 1;
        var difference = A - value - borrow;
        var result = (byte)difference;

        Status.Carry = difference >= 0;
        Status.Overflow = ((A ^ result) & (A ^ value) & 0x80) != 0;

        A = result;
        UpdateZeroAndNegative(A);
    }

    private void CompareWithAccumulator(byte value)
    {
        var result = unchecked((byte)(A - value));
        Status.Zero = A == value;
        Status.Negative = (result & 0x80) != 0;
        Status.Carry = A >= value;
    }

    private void CompareWithX(byte value)
    {
        var result = unchecked((byte)(X - value));
        Status.Zero = X == value;
        Status.Negative = (result & 0x80) != 0;
        Status.Carry = X >= value;
    }

    private void CompareWithY(byte value)
    {
        var result = unchecked((byte)(Y - value));
        Status.Zero = Y == value;
        Status.Negative = (result & 0x80) != 0;
        Status.Carry = Y >= value;
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
        return (byte)(Status.ToByte() & 0x83);
    }

    private void RestoreStatusSnapshot(byte status)
    {
        Status.Carry = (status & 0x01) != 0;
        Status.Zero = (status & 0x02) != 0;
        Status.Negative = (status & 0x80) != 0;
    }

    private static ushort GetStackAddress(byte stackPointer) => (ushort)(StackPageBaseAddress | stackPointer);
}
