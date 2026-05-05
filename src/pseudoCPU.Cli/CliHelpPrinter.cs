namespace pseudoCPU.Cli;

internal static class CliHelpPrinter
{
    public static void Print()
    {
        Console.WriteLine("pseudoCPU.Cli");
        Console.WriteLine("Usage:");
        Console.WriteLine("  pseudoCPU.Cli run-asm --source <file.asm> --start 0x0600 --max-steps 10000 [--trace]");
        Console.WriteLine("  pseudoCPU.Cli run-bin --source <file.bin> --start 0x0600 --max-steps 10000 [--trace]");
    }
}
