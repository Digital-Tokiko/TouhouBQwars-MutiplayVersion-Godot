using Godot;
using System;

public partial class Balls : CharacterBody2D
{
	[Export]
	private AnimatedSprite2D Animate;
	[Export]
	private CollisionShape2D Collide;
	[Export]
	private CollisionShape2D Collide2;
	[Export]
	private CollisionShape2D Collide3;
	[Export]
	private Area2D Area;
	[Export] PackedScene BuCreator;
	[Export] AudioStreamPlayer HitS;
	
	private float[] AngleMap = {MathF.PI/2,-MathF.PI/2,MathF.PI,0};
	
	Random rand = new Random();
	private float Damage = 20;
	
	public int BornX = 0;
	public int BornY = 0;
	
	private bool IsHited = false;
	public int BaDirection = 0;//0下，1上，2左，3右
	public bool CanSummonBullet = false;
	public bool Frozen = true;
	public bool IsKicked = false;//顺带表示能否触发激光，毕竟只有阴阳玉特殊(与能否触发激光相反)
	public float MoveSpeed = 300;
	private string[] Types = {"Yellow","Blue","Green","Red","YinYangYu","BaGuaLu","Bat","Nuclear"};
	[Export] public int BType;
	private bool IsFallen = false;
	KinematicCollision2D Collision;
	//double Delta = 0;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (Position == new Vector2(99999,99999)) QueueFree();
		Damage += rand.Next(-10,11);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Velocity.Y>0) BaDirection = 0;
		else if (Velocity.Y<0) BaDirection = 1;
		else if (Velocity.X<0) BaDirection = 2;
		else if (Velocity.X>0) BaDirection = 3;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (Frozen) return;
		//Delta = delta;
		if (BType == 4 && IsHited)  Velocity = Vector2.Right.Rotated(Rotation) * MoveSpeed;
	 	Collision = MoveAndCollide(Velocity * (float)delta);
		
		if (Collision != null){
			if (Collision.GetCollider() is StaticBody2D)
			{
				switch(BType){
					case 6: CreateBat(); Frozen = true; Break();  break;
					case 7: CreateNuc(); Frozen = true; Break();  break;
					case 0:case 1:case 2:case 3:case 5: Break();  break;	
					case 4:
						IsHited = true;
						Position -= Velocity.Normalized() * 4;
						Rotation += MathF.PI/2;
						break;
				}
			}
		}
	}
	
	public void Hit(Node2D body){
		if (Frozen) return;
		
		if (body is Player){
			Player player = (Player)body;
			player.CharacterAnimation.Play(player.HitAnim[player.Face]);
			player.IsHit = true;
			player.Health -= Damage;
			HitS.Play();
			Break();
		}
		else if (body is Balls)
		{
			TrySummonBullet();
			Balls ball = (Balls)body;
			switch (ball.BType){
			case 5:
				ball.BaDirection = BaDirection;
				ball.CreateLaser();
				ball.Break();
				break;
			case 6:	
				ball.BaDirection = BaDirection;
				ball.CreateBat();
				ball.Break();
				break;
			case 7:
				ball.BaDirection = BaDirection;
				ball.CreateNuc();
				ball.Break();
				break;
			}
			if (BType == 4){
				IsHited = true;
				if (BaDirection == 0) Position -= Velocity.Normalized() * 8;
				else Position -= Velocity.Normalized() * 6;
				Rotation += MathF.PI/2;
				return;
			}
			if (BType == 5){
				CreateLaser();
			}
			if (BType == 6){
				CreateBat();
			}
			if (BType == 7){
				CreateNuc();
			}
			if (ball.Velocity != Vector2.Zero){
					ball.Animate.Play("Broke");
					Animate.Play("Broke");
					Frozen = true;
					ball.Frozen = true;
					SceneTreeTimer CrashTimer = GetTree().CreateTimer(0.25);
					CrashTimer.Timeout += () => {
					Break();
					ball.Break();
					};
			} 
			Break();
		}
	}
	
	public void Change(){
		if(!IsFallen) {
			Animate.Play(Types[BType]);
			IsFallen = true;
			Collide.Disabled = false;
			Collide2.Disabled = false;
			Collide3.Disabled = true;
		}
	}
	
	public void CreateLaser(){
			BulletCreator Creator = (BulletCreator)BuCreator.Instantiate();
			Creator.BCType = 1;
			Creator.Rotation = AngleMap[BaDirection];
			Creator.Position = Position;
			GetTree().CurrentScene.AddChild(Creator);
	}
	
	public void CreateBat(){
			BulletCreator Creator = (BulletCreator)BuCreator.Instantiate();
			Creator.BCType = 2;
			Creator.Position = Position;
			Creator.GlobalRotation = AngleMap[BaDirection];
			Creator.BuType = BType;
			GetTree().CurrentScene.AddChild(Creator);
	}
	
	public void TrySummonBullet(){
			if (CanSummonBullet)
			{
				BulletCreator Creator = (BulletCreator)BuCreator.Instantiate();
				Creator.Position = Position;
				Creator.GlobalRotation = AngleMap[BaDirection];
				Creator.BuType = BType;
				GetTree().CurrentScene.AddChild(Creator);
			}
	}
	
	public void CreateNuc(){
			BulletCreator Creator = (BulletCreator)BuCreator.Instantiate();
			Creator.BCType = 3;
			Creator.Position = Position;
			Creator.BuType = BType;
			GetTree().CurrentScene.AddChild(Creator);
	}
	
	public void SafeQueueFree(){
		//var multiplayer = GetTree().GetMultiplayer();
		if (!IsInstanceValid(this) && !IsInsideTree()) return;
		//if (multiplayer.GetUniqueId() == 1) {
		DeCnt();
		if (Multiplayer.IsServer()) QueueFree();
		//	}
	}
	
	public void Break(){
		//if (!IsMultiplayerAuthority()) return;
		Animate.Play("Broke");
				SceneTreeTimer BrokeTimer = GetTree().CreateTimer(0.25);
				BrokeTimer.Timeout += () => {
					SafeQueueFree();
				};
	}
	
	public void DeCnt(){
		if (GetParent() is BallManager BM) {
			BM.BreakS.Play();
			if (Multiplayer.IsServer()){
				BM.BallCnt--;
				BM.IsBalled[BornX][BornY] = false;
			}
		}
	}
	
	public void HitFromAbove(){
		Godot.Collections.Array<Node2D> Bodies = Area.GetOverlappingBodies();
		foreach (Node2D body in Bodies){
			if (body is Player){
				Player player = (Player)body;
				player.CharacterAnimation.Play(player.HitAnim[player.Face]);
				player.IsHit = true;
				player.Health -= Damage;
				HitS.Play();
				Break();
			}
		}
	}
}
