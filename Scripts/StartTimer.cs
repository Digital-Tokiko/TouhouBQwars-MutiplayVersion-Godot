using Godot;
using System;

public partial class StartTimer : Timer
{
	[Export] Label GameS;
	[Export] Label ReverseCnt;
	[Export] Timer GT;
	[Export] AudioStreamPlayer StartSound;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GameStart();
	}
	
	public void GameStart(){
		if (TimeLeft >= 1) ReverseCnt.Text = ((int)TimeLeft).ToString();
		else {
			SceneTreeTimer OverTimer = GetTree().CreateTimer(0.6);
			OverTimer.Timeout += () =>	{
				GameS.Visible = true;
				ReverseCnt.Visible = false;
				};
			StartSound.Play();
			}
	}
	
	public void STFree(){
		GetTree().Paused = false;
		GT.Start();
		GameS.QueueFree();
		ReverseCnt.QueueFree();
		QueueFree();
	}
}
