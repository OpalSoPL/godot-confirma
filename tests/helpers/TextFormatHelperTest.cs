using System;
using Confirma.Attributes;
using Confirma.Classes;
using Confirma.Extensions;
using Confirma.Helpers;

using static Confirma.Enums.EFormatType;

namespace Confirma.Tests;

[TestClass]
public class TextFormatHelperTest
{
    private static string GetResetCode(bool AddReset)
    {
        return AddReset ? "\x1b[0m" : string.Empty;
    }

    #region FormatText
    [TestCase(false)]
    [TestCase(true)]
    public void FormatText_BoldEFormatType_ReturnsFormattedText(bool reset)
    {
        string TerminalReset = GetResetCode(reset);
        string result = TextFormatHelper.FormatText("hi", Bold, reset);

        _ = result.ConfirmEqual(
            Log.IsHeadless
                ? $"\x1b[1mhi{TerminalReset}"
                : "[b]hi[/b]"
        );
    }

    [TestCase("b", true)]
    [TestCase("bold", true)]
    [TestCase("b", false)]
    [TestCase("bold", false)]
    public void FormatText_BoldString_ReturnsFormattedText(string format, bool reset)
    {
        string TerminalReset = GetResetCode(reset);
        string result = TextFormatHelper.FormatText("hi", format, reset);

        _ = result.ConfirmEqual(
            Log.IsHeadless
                ? $"\x1b[1mhi{TerminalReset}"
                : "[b]hi[/b]"
        );
    }

    [TestCase(false)]
    [TestCase(true)]
    public void FormatText_ItalicEFormatType_ReturnsFormattedText(bool reset)
    {
        string TerminalReset = GetResetCode(reset);
        string result = TextFormatHelper.FormatText("hi", Italic, reset);

        _ = result.ConfirmEqual(
            Log.IsHeadless
                ? $"\x1b[3mhi{TerminalReset}"
                : "[i]hi[/i]"
        );
    }

    [TestCase("i", true)]
    [TestCase("italic", true)]
    [TestCase("i", false)]
    [TestCase("italic", false)]
    public void FormatText_ItalicString_ReturnsFormattedText(string format, bool reset)
    {
        string TerminalReset = GetResetCode(reset);
        string result = TextFormatHelper.FormatText("hi", format, reset);

        _ = result.ConfirmEqual(
            Log.IsHeadless
                ? $"\x1b[3mhi{TerminalReset}"
                : "[i]hi[/i]"
        );
    }

    [TestCase(false)]
    [TestCase(true)]
    public void FormatText_UnderlineEFormatType_ReturnsFormattedText(bool reset)
    {
        string TerminalReset = GetResetCode(reset);
        string result = TextFormatHelper.FormatText("hi", Underline, reset);

        _ = result.ConfirmEqual(
            Log.IsHeadless
                ? $"\x1b[4mhi{TerminalReset}"
                : "[u]hi[/u]"
        );
    }

    [TestCase("u", true)]
    [TestCase("underline", true)]
    [TestCase("u", false)]
    [TestCase("underline", false)]
    public void FormatText_UnderlineString_ReturnsFormattedText(string format, bool reset)
    {
        string TerminalReset = GetResetCode(reset);
        string result = TextFormatHelper.FormatText("hi", format, reset);

        _ = result.ConfirmEqual(
            Log.IsHeadless
                ? $"\x1b[4mhi{TerminalReset}"
                : "[u]hi[/u]"
        );
    }

    [TestCase(true)]
    [TestCase(false)]
    public void FormatText_strikethroughEFormatType(bool reset)
    {
        string TerminalReset = GetResetCode(reset);
        string result = TextFormatHelper.FormatText("hi", Strikethrough, reset);

        _ = result.ConfirmEqual(
            Log.IsHeadless
                ? $"\x1b[9mhi{TerminalReset}"
                : "[s]hi[/s]"
        );
    }

    [TestCase("s", true)]
    [TestCase("s", false)]
    [TestCase("strikethrough", true)]
    [TestCase("strikethrough", false)]
    public void FormatText_strikethroughEFormatType(string format, bool reset)
    {
        string TerminalReset = GetResetCode(reset);
        string result = TextFormatHelper.FormatText("hi", format, reset);

        _ = result.ConfirmEqual(
            Log.IsHeadless
                ? $"\x1b[9mhi{TerminalReset}"
                : "[s]hi[/s]"
        );
    }

    [TestCase]
    public void FormatText_FillEFormatType_ReturnsFormattedText()
    {
        string result = TextFormatHelper.FormatText("hi", Fill);

        _ = result.ConfirmEqual(
            Log.IsHeadless
                ? $"hi{new string(' ', Console.WindowWidth - 2)}"
                : $"hi{new string('\u00A0', 381)}"
        );
    }

    [TestCase("f")]
    [TestCase("fill")]
    public void FormatText_FillString_ReturnsFormattedText(string format)
    {
        string result = TextFormatHelper.FormatText("hi", format);

        _ = result.ConfirmEqual(
            Log.IsHeadless
                ? $"hi{new string(' ', Console.WindowWidth - 2)}"
                : $"hi{new string('\u00A0', 381)}"
        );
    }

    [TestCase]
    public void FormatText_CenterEFormatType_ReturnsFormattedText()
    {
        string result = TextFormatHelper.FormatText("hi", Center);

        _ = result.ConfirmEqual(
            Log.IsHeadless
                ? $"{new string(' ', (Console.WindowWidth / 2) - 1)}hi{new string(' ', (Console.WindowWidth / 2) - 1)}"
                : "[center]hi[/center]"
        );
    }

    [TestCase("c")]
    [TestCase("center")]
    public void FormatText_CenterString_ReturnsFormattedText(string format)
    {
        string result = TextFormatHelper.FormatText("hi", format);

        _ = result.ConfirmEqual(
            Log.IsHeadless
                ? $"{new string(' ', (Console.WindowWidth / 2) - 1)}hi{new string(' ', (Console.WindowWidth / 2) - 1)}"
                : "[center]hi[/center]"
        );
    }
    #endregion FormatText

    #region ToGodot
    [TestCase]
    public void ToGodot_Center_ReturnsFormattedText()
    {
        string result = TextFormatHelper.ToGodot("hi", Center);
        _ = result.ConfirmEqual("[center]hi[/center]");
    }
    [TestCase]

    [Ignore(Enums.EIgnoreMode.InHeadless)]
    public void ToGodot_Fill_ReturnsFormattedText()
    {
        string result = TextFormatHelper.ToGodot("hi", Fill);

        _ = result.ConfirmEqual($"hi{new string('\u00A0', 381)}");
    }

    [TestCase]
    public void ToGodot_Underline_ReturnsFormattedText()
    {
        string result = TextFormatHelper.ToGodot("hi", Underline);

        _ = result.ConfirmEqual("[u]hi[/u]");
    }

    [TestCase]
    public void ToGodot_Strikethrough_ReturnsFormattedText()
    {
        string result = TextFormatHelper.ToGodot("hi", Strikethrough);

        _ = result.ConfirmEqual("[s]hi[/s]");
    }

    [TestCase]
    public void ToGodot_Italic_ReturnsFormattedText()
    {
        string result = TextFormatHelper.ToGodot("hi", Italic);

        _ = result.ConfirmEqual("[i]hi[/i]");
    }

    [TestCase]
    public void ToGodot_Bold_ReturnsFormattedText()
    {
        string result = TextFormatHelper.ToGodot("hi", Bold);

        _ = result.ConfirmEqual("[b]hi[/b]");
    }
    #endregion ToGodot

    #region ToTerminal
    [TestCase]
    public void ToTerminal_Center_ReturnsFormattedText()
    {
        string result = TextFormatHelper.ToTerminal("hi", Center);

        _ = result.ConfirmEqual(
            $"{new string(' ', (Console.WindowWidth / 2) - 1)}hi{new string(' ', (Console.WindowWidth / 2) - 1)}"
        );
    }

    [TestCase]
    public void ToTerminal_Fill_ReturnsFormattedText()
    {
        string result = TextFormatHelper.ToTerminal("hi", Fill);

        _ = result.ConfirmEqual($"hi{new string(' ', Console.WindowWidth - 2)}");
    }

    [TestCase(true)]
    [TestCase(false)]
    public void ToTerminal_Underline_ReturnsFormattedText(bool reset)
    {
        string TerminalReset = GetResetCode(reset);
        string result = TextFormatHelper.ToTerminal("hi", Underline, reset);

        _ = result.ConfirmEqual($"\x1b[4mhi{TerminalReset}");
    }

    [TestCase(true)]
    [TestCase(false)]
    public void ToTerminal_Strikethrough_ReturnsFormattedText(bool reset)
    {
        string TerminalReset = GetResetCode(reset);
        string result = TextFormatHelper.ToTerminal("hi", Strikethrough, reset);

        _ = result.ConfirmEqual($"\x1b[9mhi{TerminalReset}");
    }

    [TestCase(true)]
    [TestCase(false)]
    public void ToTerminal_Italic_ReturnsFormattedText(bool reset)
    {
        string TerminalReset = GetResetCode(reset);
        string result = TextFormatHelper.ToTerminal("hi", Italic, reset);

        _ = result.ConfirmEqual($"\x1b[3mhi{TerminalReset}");
    }

    [TestCase(true)]
    [TestCase(false)]
    public void ToTerminal_Bold_ReturnsFormattedText(bool reset)
    {
        string TerminalReset = GetResetCode(reset);
        string result = TextFormatHelper.ToTerminal("hi", Bold, reset);

        _ = result.ConfirmEqual($"\x1b[1mhi{TerminalReset}");
    }
    #endregion ToTerminal
}
