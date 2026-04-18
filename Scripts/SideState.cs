using Godot;
using System;

public partial class SideState : Node2D
{
	[Export] AnimatedSprite2D Anim;
	[Export] TextureProgressBar bar;
	[Export] Label OutLabel;
	
	public Player LinkedPlayer;
	private string[] Characters = {"Rumia","Cirno","Bug","Bird"};
	private int[] YMap = {-222,-107,11,127};
	public int CType = 0;
	public int Index = 0;
	public int OutCnt = 0;
	private bool Flag = true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Position = new Vector2(386,YMap[Index]);
		NormalAnim();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		OutLabel.Text = OutCnt.ToString();
		if (LinkedPlayer.Health <= 0){
			if (Flag) {
				OutCnt++;
				Anim.Play();
				LinkedPlayer.Out();
			}
			Flag = false;
			SceneTreeTimer Reborntimer = GetTree().CreateTimer(3);
			Reborntimer.Timeout += () =>{
					NormalAnim();
					Flag = true;
					LinkedPlayer.Born();	
			};
		}
		bar.Value = LinkedPlayer.Health / 160 * 100;
	}
	
	public void NormalAnim(){
		Anim.Play(Characters[CType]);
		SceneTreeTimer StopTimer = GetTree().CreateTimer(0.9);
		StopTimer.Timeout += () => {
			Anim.Stop();
		};
	}
}
