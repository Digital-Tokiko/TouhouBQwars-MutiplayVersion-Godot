using Godot;
using System;

public partial class BulletCreator : Node2D
{
	public int BCType = 0; //0代表扇形，1代表八卦炉激光，2代表五角星，3代表核弹爆炸
	public int BuType = 0;
	private float SpreadAngle = MathF.PI/2;
	private int Num = 5;
	[Export] private PackedScene Ammo;
	[Export] private PackedScene Laser;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (BCType == 0){
			FanCreator();
		}
		if (BCType == 1) 
		{
			LaserCreator();
		}
		if (BCType == 2)
		{
			PentaCreator();
		}
		if (BCType == 3)
		{
			NuCCreator();
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void FanCreator(){
		float StartAngle = GlobalRotation - SpreadAngle/2;
		for (int i=0;i<Num;i++){
			var FiredAmmo = Ammo.Instantiate() as Bullet;
			float CurrentAngle = StartAngle + SpreadAngle / (Num-1) * i;
			FiredAmmo.Rotation = CurrentAngle;
			FiredAmmo.Position = Position;
			FiredAmmo.BulletType = BuType;
			GetTree().Root.CallDeferred(Node.MethodName.AddChild, FiredAmmo);
		}
		QueueFree();
	}
	
	public void LaserCreator(){
		var FiredLaser = Laser.Instantiate() as Laser;
		FiredLaser.Position = Position;
		FiredLaser.Rotation = Rotation;
		GetTree().Root.CallDeferred(Node.MethodName.AddChild,FiredLaser);
	}
	
	public void PentaCreator(){
		float StartAngle = GlobalRotation;
		for (int i=0;i<Num;i++){
		var FiredAmmo = Ammo.Instantiate() as Bullet;
		float CurrentAngle = StartAngle + MathF.PI * 2 / Num * i;
		FiredAmmo.Position = Position;
		FiredAmmo.Rotation = CurrentAngle;
		FiredAmmo.BulletType = BuType;
		GetTree().Root.CallDeferred(Node.MethodName.AddChild, FiredAmmo);
		}
	}
	
	public void NuCCreator(){
		var FiredAmmo = Ammo.Instantiate() as Bullet;
		FiredAmmo.Position = Position;
		FiredAmmo.BulletType = BuType;
		GetTree().Root.CallDeferred(Node.MethodName.AddChild, FiredAmmo);
	}
}
