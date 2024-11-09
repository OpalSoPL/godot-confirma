using System.Collections.Generic;
using System.Reflection;
using Confirma.Attributes;
using Confirma.Classes.Discovery;
using Confirma.Enums;
using Confirma.Exceptions;
using Confirma.Helpers;
using Confirma.Types;

using static Confirma.Enums.ETestCaseState;

namespace Confirma.Classes;

public class TestingMethod
{
    public MethodInfo Method { get; }
    public IEnumerable<TestCase> TestCases { get; }
    public string Name { get; }
    public TestMethodResult Result { get; }

    public TestingMethod(MethodInfo method)
    {
        Result = new();
        Method = method;
        TestCases = DiscoverTestCases();
        Name = Method.GetCustomAttribute<TestNameAttribute>()?.Name ?? Method.Name;
    }

    public TestMethodResult Run(TestsProps props, object? instance)
    {
        foreach (TestCase test in TestCases)
        {
            int iterations = test.Repeat?.IsFlaky == true
                ? 0
                : test.Repeat?.Repeat ?? 0;

            for (ushort i = 0; i <= iterations; i++)
            {
                IgnoreAttribute? attr = test.Method.GetCustomAttribute<IgnoreAttribute>();

                if (attr?.IsIgnored(props.Target) == true)
                {
                    if (attr.HideFromResults == true)
                    {
                        continue;
                    }

                    Result.TestsIgnored++;

                    TestLog log = new(
                        ELogType.Method,
                        Name,
                        Ignored,
                        test.Params,
                        attr.Reason
                    );
                    Result.TestLogs.Add(log);
                    continue;
                }

                int repeats = 0;
                int maxRepeats = test.Repeat?.GetFlakyRetries ?? 0;
                int backoff = test.Repeat?.Backoff.Milliseconds ?? 0;

                do
                {
                    try
                    {
                        test.Run(instance);
                        Result.TestsPassed++;

                        TestLog log = new(
                            ELogType.Method,
                            Name,
                            Passed,
                            test.Params,
                            null,
                            ELangType.CSharp
                        );
                        Result.TestLogs.Add(log);
                        break;
                    }
                    catch (ConfirmAssertException e)
                    {
                        if (maxRepeats != 0 && repeats != maxRepeats)
                        {
                            repeats++;
                            // if (backoff != 0)
                            // {
                            //     Task.Delay(backoff).RunSynchronously();
                            // }
                            continue;
                        }

                        Result.TestsFailed++;

                        TestLog log = new(
                            ELogType.Method,
                            Name,
                            Failed,
                            test.Params,
                            e.Message,
                            ELangType.CSharp
                        );
                        Result.TestLogs.Add(log);

                        if (test.Repeat?.FailFast == true)
                        {
                            break;
                        }

                        if (props.ExitOnFail)
                        {
                            props.CallExitOnFailure();
                        }
                    }
                } while (repeats < maxRepeats);
            }
        }

        return Result;
    }

    private List<TestCase> DiscoverTestCases()
    {
        List<TestCase> cases = new();
        using IEnumerator<System.Attribute> discovered = CsTestDiscovery
            .GetTestCasesFromMethod(Method)
            .GetEnumerator();

        while (discovered.MoveNext())
        {
            switch (discovered.Current)
            {
                case TestCaseAttribute testCase:
                    cases.Add(new(Method, testCase.Parameters, null));
                    continue;
                // I rely on the order in which the attributes are defined
                // to determine which TestCase attributes should be assigned values
                // from the Repeat attributes.
                case RepeatAttribute when !discovered.MoveNext():
                    Log.PrintWarning(
                        $"The Repeat attribute for the \"{Method.Name}\" method will be ignored " +
                        "because it does not have the TestCase attribute after it.\n"
                    );
                    Result.Warnings++;
                    continue;
                case RepeatAttribute when discovered.Current is RepeatAttribute:
                    Log.PrintWarning(
                        $"The Repeat attributes for the \"{Method.Name}\" cannot occur in succession.\n"
                    );
                    Result.Warnings++;
                    continue;
                case RepeatAttribute repeat:
                    {
                        if (discovered.Current is not TestCaseAttribute tc)
                        {
                            continue;
                        }

                        cases.Add(new(Method, tc.Parameters, repeat));
                        break;
                    }
            }
        }

        return cases;
    }
}
