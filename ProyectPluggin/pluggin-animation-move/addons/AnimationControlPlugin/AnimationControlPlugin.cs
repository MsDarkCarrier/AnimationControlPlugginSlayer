using Godot;


[Tool]
public partial class AnimationControlPlugin : EditorPlugin
{
	public override void _EnterTree()
	{
		AddCustomType("AnimationControl", "Panel", GD.Load<Script>("res://addons/AnimationControlPlugin/AnimationControl.cs"), GD.Load<Texture2D>("res://addons/LogoPluggin.svg"));
	}

	public override void _ExitTree()
	{
		RemoveCustomType("AnimationControl");
	}

}
