using System;
using System.Reflection;
using Confirma.Attributes;
using Confirma.Exceptions;
using Confirma.Helpers;

namespace Confirma.Classes;

public class TestCase
{
    public MethodInfo Method { get; init; }
    public object?[]? Parameters { get; init; }
    public string Params { get; init; }
    public RepeatAttribute? Repeat { get; init; }

    public TestCase(
        MethodInfo method,
        object?[]? parameters,
        RepeatAttribute? repeat
    )
    {
        Method = method;
        Parameters = parameters;
        Params = parameters is not null && parameters.Length != 0
            ? CollectionHelper.ToString(parameters, addBrackets: false)
            : string.Empty;

        Repeat = repeat;
    }

    public void Run()
    {
        try
        {
            _ = Method.Invoke(null, Parameters);
        }
        catch (TargetInvocationException tie)
        {
            throw new ConfirmAssertException(
                tie.InnerException?.Message ?? tie.Message
            );
        }
        catch (Exception e) when (e is ArgumentException or ArgumentNullException)
        {
            throw new ConfirmAssertException(
                $"- Failed: Invalid test case parameters: {Params}."
            );
        }
        catch (Exception e)
        {
            throw new ConfirmAssertException($"- Failed: {e.Message}");
        }
    }
}
