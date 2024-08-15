using System;
using System.Globalization;
using System.Threading.Tasks;
using Confirma.Classes.Executors;
using Confirma.Helpers;
using Confirma.Types;
using Confirma.Enums;

namespace Confirma.Classes;

public static class TestManager
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

    public static void Run()
    {
        bool isCs = false;
        int totalClasses = 0;
        DateTime startTimeStamp = DateTime.Now;

        _props.ResetStats();

        if (!_props.DisableCsharp)
        {
            CsTestExecutor csExecutor = new(_props);
            int csClasses = csExecutor.Execute(out TestResult? res);

            if (csClasses == -1)
            {
                if (_props.ClassName?.Length == 0)
                {
                    return;
                }
            }
            else if (_props.ClassName?.Length != 0)
            {
                isCs = true;
            }

            totalClasses += csClasses;
            _props.Result += res!;
        }

        if (!_props.DisableGdScript && !isCs)
        {
            GdTestExecutor gdExecutor = new(_props);
            int gdClasses = gdExecutor.Execute(out TestResult? res);

            if (gdClasses == -1)
            {
                return;
            }

            totalClasses += gdClasses;
            _props.Result += res!;
        }

        _props.Result.TotalTime = (DateTime.Now - startTimeStamp).TotalSeconds;
        _props.Result.TotalClasses = (uint)totalClasses;

        if ((_props.OutputType & ELogOutputType.Log) == ELogOutputType.Log)
        {
            PrintTestLogs();
            PrintSummary();
        }

        if ((_props.OutputType & ELogOutputType.Json) == ELogOutputType.Json)
        {
            DumpJson();
        }
    }

    private static void PrintTestLogs()
    {
        foreach (TestLog log in _props.Result.TestLogs)
        {
            log.PrintOutput(_props.IsVerbose);
        }
    }

    private static void PrintSummary()
    {
        Log.PrintLine(
            string.Format(
                CultureInfo.InvariantCulture,
                "\nConfirma ran {0} tests in {1} test classes. Tests took {2}s.\n{3}, {4}, {5}, {6}{7}.",
                _props.Result.TotalTests,
                _props.Result.TotalClasses,
                _props.Result.TotalTime,
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

    public static void DumpJson()
    {
        Task task = Task.Run(
            static async () => await Json.DumpToFileAsync(
                "test_results.json",
                _props.Result,
                true
            )
        );
        task.Wait();
    }
}
