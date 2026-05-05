namespace pseudoCPU.Core;

public sealed class BootstrapCpuStepLimitExceededException : InvalidOperationException
{
    public BootstrapCpuStepLimitExceededException(string message) : base(message)
    {
    }
}
