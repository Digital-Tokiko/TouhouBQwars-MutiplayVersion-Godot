using Godot;
using System;

public partial class GameTimer : Timer
{
	private int Cnt = 120;
	[Export] Label RCnt;
	[Export] Label Over;
	[Export] AudioStreamPlayer OverSound;
	[Export] AudioStreamPlayer TickSound;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		RCnt.Text = Cnt.ToString();
		if (Cnt == 0){
			OverSound.Play();
			SceneTreeTimer OverTimer = GetTree().CreateTimer(0.8);
			OverTimer.Timeout += () =>	Over.Visible = true;
			GetTree().Paused = true
			;
		}
	}
	
	public void Tick(){
		TickSound.Play();
		Cnt--;
	}
}
