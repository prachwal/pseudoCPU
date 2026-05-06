namespace pseudoCPU.Core;

public sealed class BootstrapStatusRegister
{
    private const byte CarryBit = 1 << 0;
    private const byte ZeroBit = 1 << 1;
    private const byte InterruptDisableBit = 1 << 2;
    private const byte DecimalBit = 1 << 3;
    private const byte BreakBit = 1 << 4;
    private const byte OverflowBit = 1 << 6;
    private const byte NegativeBit = 1 << 7;

    public bool Carry { get; set; }

    public bool Zero { get; set; }

    public bool InterruptDisable { get; set; }

    public bool Decimal { get; set; }

    public bool Break { get; set; }

    public bool Overflow { get; set; }

    public bool Negative { get; set; }

    public byte ToByte()
    {
        byte status = 0;

        if (Carry)
        {
            status |= CarryBit;
        }

        if (Zero)
        {
            status |= ZeroBit;
        }

        if (InterruptDisable)
        {
            status |= InterruptDisableBit;
        }

        if (Decimal)
        {
            status |= DecimalBit;
        }

        if (Break)
        {
            status |= BreakBit;
        }

        if (Overflow)
        {
            status |= OverflowBit;
        }

        if (Negative)
        {
            status |= NegativeBit;
        }

        return status;
    }

    public static BootstrapStatusRegister FromByte(byte status) => new()
    {
        Carry = (status & CarryBit) != 0,
        Zero = (status & ZeroBit) != 0,
        InterruptDisable = (status & InterruptDisableBit) != 0,
        Decimal = (status & DecimalBit) != 0,
        Break = (status & BreakBit) != 0,
        Overflow = (status & OverflowBit) != 0,
        Negative = (status & NegativeBit) != 0,
    };
}
