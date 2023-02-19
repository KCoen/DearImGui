using JetBrains.Annotations;

namespace VulkanGenerator.Extensions;

[PublicAPI]
public sealed class AggregateConsoleOut : IDisposable
{
    private readonly TextWriter Out;

    private readonly AggregateTextWriter Writer;

    public AggregateConsoleOut(params TextWriter[] writers)
    {
        Writer = new AggregateTextWriter(writers.Append(Out = Console.Out));
        Console.SetError(Writer);
        Console.SetOut(Writer);
    }

    #region IDisposable Members

    public void Dispose()
    {
        Console.SetOut(Out);

        Writer.Dispose();
    }

    #endregion
}