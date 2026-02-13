using Godot;
using System;

public partial class Laser : Area2D
{
	[Export] AudioStreamPlayer HitS;
	[Export] AudioStreamPlayer BGLS;
	Random rand = new Random();
	private float Damage = 20;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BGLS.Play();
		Damage += rand.Next(-10,11);
		SceneTreeTimer Vanishtimer = GetTree().CreateTimer(1.5);
		Vanishtimer.Timeout += () => {
			QueueFree();
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void Blasted(Node2D body){
		if (body is Player){
			HitS.Play();
			Player player = (Player)body;
			player.CharacterAnimation.Play(player.HitAnim[player.Face]);
			player.Health -= Damage;
			player.IsHit = true;
		}
	}
}
