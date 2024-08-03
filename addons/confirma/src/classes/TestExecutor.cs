using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Confirma.Attributes;
using Confirma.Classes.Discovery;
using Confirma.Helpers;
using Confirma.Types;

namespace Confirma.Classes;

public static class TestExecutor
{
    public static TestsProps Props
    {
        get => _props;
        set
        {
            _props.ExitOnFailure -= static () => { };

            _props = value;

            _props.ExitOnFailure += static () =>
            {
                // GetTree().Quit() doesn't close the program immediately
                // and allows all the remaining tests to run.
                // This is a workaround to close the program immediately,
                // at the cost of Godot displaying a lot of errors.
                Environment.Exit(1);
            };
        }
    }

    private static TestsProps _props;

    public static void ExecuteTests(Assembly assembly, string className)
    {
        IEnumerable<TestingClass> testClasses = CsTestDiscovery.DiscoverTestClasses(assembly);
        DateTime startTimeStamp = DateTime.Now;

        if (!string.IsNullOrEmpty(className))
        {
            testClasses = testClasses.Where(tc => tc.Type.Name == className);

            if (!testClasses.Any())
            {
                Log.PrintError($"No test class found with the name '{className}'.");
                return;
            }
        }

        _props.ResetStats();

        if (_props.DisableParallelization)
        {
            foreach (TestingClass testClass in testClasses)
            {
                ExecuteSingleClass(testClass, _props.Result);
            }
        }
        else
        {
            var (parallelTestClasses, sequentialTestClasses) = ClassifyTests(testClasses);
            ConcurrentBag<TestResult> results = new();

            parallelTestClasses.AsParallel().ForAll(testClass =>
            {
                TestResult localResult = new();
                ExecuteSingleClass(testClass, localResult);
                results.Add(localResult);
            });

            foreach (TestingClass testClass in sequentialTestClasses)
            {
                ExecuteSingleClass(testClass, _props.Result);
            }

            // Aggregate results
            foreach (TestResult result in results)
            {
                _props.Result += result;
            }
        }

        _props.Result.TotalOrphans += (uint)Godot.Performance.GetMonitor(
            Godot.Performance.Monitor.ObjectOrphanNodeCount
        );

        PrintSummary(testClasses.Count(), startTimeStamp);
    }

    private static (IEnumerable<TestingClass>, IEnumerable<TestingClass>)
    ClassifyTests(IEnumerable<TestingClass> tests)
    {
        return (
          CsTestDiscovery.GetParallelTestClasses(tests),
          CsTestDiscovery.GetSequentialTestClasses(tests)
        );
    }

    private static void ExecuteSingleClass(TestingClass testClass, TestResult result)
    {
        Log.Print($"> {testClass.Type.Name}...");

        IgnoreAttribute? attr = testClass.Type.GetCustomAttribute<IgnoreAttribute>();
        if (attr?.IsIgnored() == true)
        {
            _props.Result.TestsIgnored += (uint)testClass.TestMethods.Sum(
                static m => m.TestCases.Count()
            );

            Log.PrintWarning(" ignored.\n");

            if (string.IsNullOrEmpty(attr.Reason))
            {
                return;
            }

            Log.PrintWarning($"- {attr.Reason}\n");
        }

        Log.PrintLine();

        TestClassResult classResult = testClass.Run(_props);

        result += classResult;
    }

    private static void PrintSummary(int count, DateTime startTimeStamp)
    {
        Log.PrintLine(
            string.Format(
                CultureInfo.InvariantCulture,
                "\nConfirma ran {0} tests in {1} test classes. Tests took {2}s.\n{3}, {4}, {5}, {6}{7}.",
                _props.Result.TotalTests,
                count,
                (DateTime.Now - startTimeStamp).TotalSeconds,
                Colors.ColorText($"{_props.Result.TestsPassed} passed", Colors.Success),
                Colors.ColorText($"{_props.Result.TestsFailed} failed", Colors.Error),
                Colors.ColorText($"{_props.Result.TestsIgnored} ignored", Colors.Warning),
                _props.MonitorOrphans
                    ? Colors.ColorText($"{_props.Result.TotalOrphans} orphans, ", Colors.Warning)
                    : string.Empty,
                Colors.ColorText($"{_props.Result.Warnings} warnings", Colors.Warning)
            )
        );
    }
}
