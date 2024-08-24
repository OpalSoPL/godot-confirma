using System;
using Confirma.Exceptions;
using Godot;

namespace Confirma.Wrappers;

public partial class WrapperBase : CSharpScript
{
    public static event Action<string>? GdAssertionFailed;

    protected static void CallGdAssertionFailed(in ConfirmAssertException e)
    {
        GdAssertionFailed?.Invoke(
            string.IsNullOrEmpty(e.Message)
                ? e.InnerException?.Message ?? string.Empty
                : e.Message
        );
    }

    /// <summary>
    /// 'null' in GDScript is not the same as null in C#.
    /// That's why this 'parser' is necessary.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    protected static string? ParseMessage(string? message)
    {
        return string.IsNullOrEmpty(message) ? null : message;
    }
}
