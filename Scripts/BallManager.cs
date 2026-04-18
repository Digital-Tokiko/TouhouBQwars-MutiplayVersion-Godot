using Godot;
using System;

public partial class BallManager : Node2D
{
	[Export]public int BallCnt = 0;
	public bool[][] IsBalled;
	[Export]public AudioStreamPlayer BreakS;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		IsBalled = new bool[10][];
		for(int i = 0;i<10;i++){
			IsBalled[i] = new bool[10];
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
