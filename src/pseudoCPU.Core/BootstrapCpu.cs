namespace pseudoCPU.Core;

public sealed class BootstrapCpu
{
    private const ushort StackPageBaseAddress = 0x0100;
    private readonly BootstrapMemoryBus _memoryBus = new();

    public BootstrapMemoryBus MemoryBus => _memoryBus;

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
        MemoryBus.LoadProgram(program, startAddress);

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
            case BootstrapOpcode.BitZeroPage:
                BitZeroPage(FetchByte());
                break;
            case BootstrapOpcode.LdaZeroPage:
                LoadAFromZeroPage(FetchByte());
                break;
            case BootstrapOpcode.LdaZeroPageX:
                LoadAFromZeroPageIndexedX(FetchByte());
                break;
            case BootstrapOpcode.LdxZeroPage:
                LoadXFromZeroPage(FetchByte());
                break;
            case BootstrapOpcode.LdxZeroPageY:
                LoadXFromZeroPageIndexedY(FetchByte());
                break;
            case BootstrapOpcode.LdyZeroPage:
                LoadYFromZeroPage(FetchByte());
                break;
            case BootstrapOpcode.LdyZeroPageX:
                LoadYFromZeroPageIndexedX(FetchByte());
                break;
            case BootstrapOpcode.StaZeroPage:
                StoreAIntoZeroPage(FetchByte());
                break;
            case BootstrapOpcode.StaZeroPageX:
                StoreAIntoZeroPageIndexedX(FetchByte());
                break;
            case BootstrapOpcode.StxZeroPage:
                StoreXIntoZeroPage(FetchByte());
                break;
            case BootstrapOpcode.StxZeroPageY:
                StoreXIntoZeroPageIndexedY(FetchByte());
                break;
            case BootstrapOpcode.StyZeroPage:
                StoreYIntoZeroPage(FetchByte());
                break;
            case BootstrapOpcode.StyZeroPageX:
                StoreYIntoZeroPageIndexedX(FetchByte());
                break;
            case BootstrapOpcode.AslAccumulator:
                ShiftAccumulatorLeft();
                break;
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
            case BootstrapOpcode.LsrAccumulator:
                ShiftAccumulatorRight();
                break;
            case BootstrapOpcode.RolAccumulator:
                RotateAccumulatorLeft();
                break;
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
                    MemoryBus.WriteByte(address, A);
                    break;
                }
            case BootstrapOpcode.StxAbsolute:
                {
                    var lowByte = FetchByte();
                    var highByte = FetchByte();
                    var address = (ushort)(lowByte | (highByte << 8));
                    MemoryBus.WriteByte(address, X);
                    break;
                }
            case BootstrapOpcode.StyAbsolute:
                {
                    var lowByte = FetchByte();
                    var highByte = FetchByte();
                    var address = (ushort)(lowByte | (highByte << 8));
                    MemoryBus.WriteByte(address, Y);
                    break;
                }
            case BootstrapOpcode.CmpImmediate:
                CompareWithAccumulator(FetchByte());
                break;
            case BootstrapOpcode.SbcImmediate:
                SubtractWithBorrow(FetchByte());
                break;
            case BootstrapOpcode.RorAccumulator:
                RotateAccumulatorRight();
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
            case BootstrapOpcode.BplRelative:
                BranchRelative(!Negative, FetchByte());
                break;
            case BootstrapOpcode.BmiRelative:
                BranchRelative(Negative, FetchByte());
                break;
            case BootstrapOpcode.BvcRelative:
                BranchRelative(!Status.Overflow, FetchByte());
                break;
            case BootstrapOpcode.BvsRelative:
                BranchRelative(Status.Overflow, FetchByte());
                break;
            case BootstrapOpcode.BccRelative:
                BranchRelative(!Carry, FetchByte());
                break;
            case BootstrapOpcode.BcsRelative:
                BranchRelative(Carry, FetchByte());
                break;
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

    public byte ReadByte(ushort address) => MemoryBus.ReadByte(address);

    public void WriteByte(ushort address, byte value) => MemoryBus.WriteByte(address, value);

    public void PushByte(byte value)
    {
        MemoryBus.WriteByte(GetStackAddress(SP), value);
        SP--;
    }

    public byte PopByte()
    {
        SP++;
        return MemoryBus.ReadByte(GetStackAddress(SP));
    }

    private byte FetchByte() => MemoryBus.ReadByte(PC++);

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

    private void BitZeroPage(byte zeroPageAddress)
    {
        var value = MemoryBus.ReadByte(MemoryBus.ResolveZeroPageAddress(zeroPageAddress));

        Status.Zero = (A & value) == 0;
        Status.Overflow = (value & 0x40) != 0;
        Status.Negative = (value & 0x80) != 0;
    }

    private void LoadAFromZeroPage(byte zeroPageAddress)
    {
        A = MemoryBus.ReadByte(MemoryBus.ResolveZeroPageAddress(zeroPageAddress));
        UpdateZeroAndNegative(A);
    }

    private void LoadAFromZeroPageIndexedX(byte zeroPageAddress)
    {
        A = MemoryBus.ReadByte(MemoryBus.ResolveZeroPageIndexedAddress(zeroPageAddress, X));
        UpdateZeroAndNegative(A);
    }

    private void LoadXFromZeroPage(byte zeroPageAddress)
    {
        X = MemoryBus.ReadByte(MemoryBus.ResolveZeroPageAddress(zeroPageAddress));
        UpdateZeroAndNegative(X);
    }

    private void LoadXFromZeroPageIndexedY(byte zeroPageAddress)
    {
        X = MemoryBus.ReadByte(MemoryBus.ResolveZeroPageIndexedAddress(zeroPageAddress, Y));
        UpdateZeroAndNegative(X);
    }

    private void LoadYFromZeroPage(byte zeroPageAddress)
    {
        Y = MemoryBus.ReadByte(MemoryBus.ResolveZeroPageAddress(zeroPageAddress));
        UpdateZeroAndNegative(Y);
    }

    private void LoadYFromZeroPageIndexedX(byte zeroPageAddress)
    {
        Y = MemoryBus.ReadByte(MemoryBus.ResolveZeroPageIndexedAddress(zeroPageAddress, X));
        UpdateZeroAndNegative(Y);
    }

    private void StoreAIntoZeroPage(byte zeroPageAddress) => MemoryBus.WriteByte(MemoryBus.ResolveZeroPageAddress(zeroPageAddress), A);

    private void StoreAIntoZeroPageIndexedX(byte zeroPageAddress) => MemoryBus.WriteByte(MemoryBus.ResolveZeroPageIndexedAddress(zeroPageAddress, X), A);

    private void StoreXIntoZeroPage(byte zeroPageAddress) => MemoryBus.WriteByte(MemoryBus.ResolveZeroPageAddress(zeroPageAddress), X);

    private void StoreXIntoZeroPageIndexedY(byte zeroPageAddress) => MemoryBus.WriteByte(MemoryBus.ResolveZeroPageIndexedAddress(zeroPageAddress, Y), X);

    private void StoreYIntoZeroPage(byte zeroPageAddress) => MemoryBus.WriteByte(MemoryBus.ResolveZeroPageAddress(zeroPageAddress), Y);

    private void StoreYIntoZeroPageIndexedX(byte zeroPageAddress) => MemoryBus.WriteByte(MemoryBus.ResolveZeroPageIndexedAddress(zeroPageAddress, X), Y);

    private void ShiftAccumulatorLeft()
    {
        Status.Carry = (A & 0x80) != 0;
        A = (byte)(A << 1);
        UpdateZeroAndNegative(A);
    }

    private void ShiftAccumulatorRight()
    {
        Status.Carry = (A & 0x01) != 0;
        A = (byte)(A >> 1);
        UpdateZeroAndNegative(A);
    }

    private void RotateAccumulatorLeft()
    {
        var carryIn = Status.Carry ? 1 : 0;

        Status.Carry = (A & 0x80) != 0;
        A = (byte)((A << 1) | carryIn);
        UpdateZeroAndNegative(A);
    }

    private void RotateAccumulatorRight()
    {
        var carryIn = Status.Carry ? 0x80 : 0;

        Status.Carry = (A & 0x01) != 0;
        A = (byte)((A >> 1) | carryIn);
        UpdateZeroAndNegative(A);
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
