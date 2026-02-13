using Godot;
using System;

public partial class Bullet : Area2D
{
	[Export] CollisionShape2D NC;
	[Export] CollisionShape2D BC;
	[Export] CollisionShape2D NuC;
	[Export] AudioStreamPlayer HitS;
	[Export] AudioStreamPlayer NuclearS;
	
	Random rand = new Random();
	private float Damage = 20;
	
	public int BulletType = 0; //0,1,2,3对应文件里面的1,2,3,4
	private string[] BulletTypes = {"Yellow","Blue","Green","Red","Yellow","Yellow","Bat","Nuclear"};
	[Export] private AnimatedSprite2D BulletAnim;
	private float[] BulletSpeeds = {200,200,200,200,200,200,300,0};
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Damage += rand.Next(-10,11);
		BulletAnim.Play(BulletTypes[BulletType]);
		switch (BulletType){
			case 6:
			NC.Disabled = true;
			BC.Disabled = false;
			break;
			case 7:
			NuclearS.Play();
			NC.Disabled = true;
			NuC.Disabled = false;
			break;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Position += Vector2.Right.Rotated(Rotation) * BulletSpeeds[BulletType] * (float)delta;
	}
	
	public void Hit(Node2D body){
		if (body is Player){
			HitS.Play();
			Player player = (Player)body;
			if (BulletType == 7){
				player.CharacterAnimation.Play(player.HitAnim[player.Face]);
				player.Health -= Damage;
				player.IsHit = true;
				SceneTreeTimer ExplodeTimer = GetTree().CreateTimer(1.5);
				ExplodeTimer.Timeout += () =>{
					QueueFree();
				};
			}
			if (player.CType == BulletType || BulletType == 7) return;
			player.CharacterAnimation.Play(player.HitAnim[player.Face]);
			player.Health -= Damage;
			player.IsHit = true;
			QueueFree();
		}
	}
}
