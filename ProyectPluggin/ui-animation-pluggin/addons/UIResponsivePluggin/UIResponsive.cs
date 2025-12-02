#if TOOLS
using Godot;
using System;

[Tool]
public partial class UIResponsive : EditorPlugin
{
    public override void _EnterTree()
    {
        AddCustomType("UIAnimation", "Panel", GD.Load<CSharpScript>("res://addons/UIResponsivePluggin/AnimationControl.cs"), GD.Load<Texture2D>("res://addons/UIResponsivePluggin/LogoPluggin.png"));
    }

    public override void _ExitTree()
    {
        RemoveCustomType("UIAnimation");
    }
}
#endif
