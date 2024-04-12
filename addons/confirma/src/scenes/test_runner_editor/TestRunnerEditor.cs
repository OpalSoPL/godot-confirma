#if TOOLS

using Godot;

namespace Confirma.Scenes;

[Tool]
public partial class TestRunnerEditor : TestRunner
{
	public override void _Ready()
	{
		base._Ready();

		_executor = new();

		ClearOutput();
	}

	public void ClearOutput()
	{
		_output.Clear();
	}
}

#endif
