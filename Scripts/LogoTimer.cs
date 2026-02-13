using Godot;
using System;

public partial class LogoTimer : Timer
{
	[Export] private AnimationPlayer Anim;
	[Export] private AnimatedSprite2D AnimLogo;
	[Export] private Sprite2D SL;
	bool ZEnabled = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void Logoin(){
		AnimLogo.Visible = true;
		SL.Visible = true;
		Anim.Play("SubLogoin");
		AnimLogo.Play("Logo");
	}
	
	public void ButtonUp(String AnimName){
		if (AnimName=="SubLogoin") {
			Anim.Play("ButtonIn");
			}
	}
}
