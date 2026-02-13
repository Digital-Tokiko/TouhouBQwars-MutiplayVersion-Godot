using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	private float MoveSpeed = 175;
	[Export]
	public AnimatedSprite2D CharacterAnimation;
	[Export]
	public AnimationPlayer ComAnimation;
	[Export]
	public PackedScene Side;
	[Export] public float Health {get; set;} = 160;
	[Export] public int Index = 0; //代表玩家序号
	[Export] private AudioStreamPlayer PushS;
	[Export] private AudioStreamPlayer OutS;

	[Export] private AnimatedSprite2D CirnoAnim;
	[Export] private AnimatedSprite2D BugAnim;
	[Export] private AnimatedSprite2D BirdAnim;
	[Export] private CollisionShape2D DownC;
	[Export] private CollisionShape2D UpC;
	[Export] private CollisionShape2D LeftC;
	[Export] private CollisionShape2D RightC;
	
	[Export] public int Face {get; set;} = 0;  //0代表下方，1代表上方，2代表左方，3代表右方
	//public int Face = 0;  //0代表下方，1代表上方，2代表左方，3代表右方
	private bool IsKicking = false; //可以防止玩家移动
	public bool IsHit = false;
	[Export] public int CType = 0;
	
	public string[] HitAnim = {"DownHit","UpHit","LeftHit","RightHit"};
	private string[] StillAnim = {"DownStill","UpStill","LeftStill","RightStill"};
	private string[] WalkAnim = {"DownWalk","UpWalk","LeftWalk","RightWalk"};
	private string[] PushAnim = {"DownPush","UpPush","LeftPush","RightPush"};
	private string[] OutAnim = {"out","out_Cirno","out_Bug","out_Bird"};
	private string[] ResetAnim = {"RESET","RESET_Cirno","RESET_Bug","RESET_Bird"};
	private Vector2[] BornPosition = {new Vector2(-179,-202),new Vector2(180,-202),new Vector2(-179,158),new Vector2(182,158)};
	private Vector2[] BallDirection = {new Vector2(0,1),new Vector2(0,-1),new Vector2(-1,0),new Vector2(1,0)};

	public override void _EnterTree()
	{
	if (Multiplayer != null)
	{
		SetMultiplayerAuthority(Name.ToString().ToInt());
	}
}
	public override void _Ready()
	{
		Born();
		CreateSide();
		
		switch (CType){
			case 0: CharacterAnimation.Visible = true; break;
			case 1: CharacterAnimation = CirnoAnim; CharacterAnimation.Visible = true; break;
			case 2: CharacterAnimation = BugAnim; CharacterAnimation.Visible = true; break;
			case 3: CharacterAnimation = BirdAnim; CharacterAnimation.Visible = true; break;
			}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Velocity.Y > 0) Face = 0;
		else if (Velocity.Y < 0) Face = 1;
		else if (Velocity.X < 0) Face = 2;
		else if (Velocity.X > 0) Face = 3;
		
		if (IsHit){
			SceneTreeTimer HitTimer = GetTree().CreateTimer(0.5);
			HitTimer.Timeout += () => {
				IsHit = false;
			};
		}
		
		if (Input.IsActionJustPressed("Kick") && !IsKicking && !IsHit && IsMultiplayerAuthority()){
			IsKicking = true;
			CharacterAnimation.Play(PushAnim[Face]);
			KickBoxChange(false);
			SceneTreeTimer KickTimer = GetTree().CreateTimer(0.4);
			KickTimer.Timeout += () => {
				IsKicking = false;
				KickBoxChange(true);
			};
		}
		
		if (IsKicking || IsHit) return;
		if (IsMultiplayerAuthority()) {
			if (Velocity == Vector2.Zero) CharacterAnimation.Play(StillAnim[Face]);
			else CharacterAnimation.Play(WalkAnim[Face]);
		}
	}
	
		public override void _PhysicsProcess(double delta)
	{    
		if (IsKicking || IsHit || !IsMultiplayerAuthority()) return;
		Velocity = Input.GetVector("left","right","up","down") * MoveSpeed;
		MoveAndCollide(Velocity * (float)delta);
		
	}
	
	public void Kicked(Node2D body){
		if (body is Balls){
			PushS.Play();
			Balls ball = (Balls)body;
			ball.Frozen = false;
			ball.IsKicked = true;
			ball.Velocity = BallDirection[Face] * ball.MoveSpeed;
			if (ball.BType == CType) {
			ball.BaDirection = Face;
			ball.CanSummonBullet = true;
			}
		}
	}
	
	public void KickBoxChange(bool To){
		switch(Face){
		case 0: DownC.Disabled = To; break;
		case 1: UpC.Disabled = To; break;
		case 2: LeftC.Disabled = To; break;
		case 3: RightC.Disabled = To; break;
		}
	}
	
	public void CreateSide(){
		var SideS = Side.Instantiate() as SideState;
		SideS.Index = Index;
		SideS.CType = CType;
		SideS.LinkedPlayer = this;
		GetTree().Root.CallDeferred(Node.MethodName.AddChild,SideS);
	}
	
	public void Born(){
		Health = 160;
		Position = BornPosition[CType];
		ComAnimation.PlayBackwards(OutAnim[CType]);
		IsKicking = true;
		SceneTreeTimer BornTimer = GetTree().CreateTimer(0.5);
		BornTimer.Timeout += () =>{
			IsKicking = false;
			ComAnimation.Play(ResetAnim[CType]);
		};
	}
	
	public void Out(){
		ComAnimation.Play(OutAnim[CType]);
		OutS.Play();
		IsKicking = true;
		IsHit = false;
		SceneTreeTimer OutTimer = GetTree().CreateTimer(0.5);
		OutTimer.Timeout += () =>{
				Position = new Vector2(10000,10000);
		};
	}
	
}
