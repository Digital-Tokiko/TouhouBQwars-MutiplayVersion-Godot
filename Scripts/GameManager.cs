using Godot;
using System;

public partial class GameManager : Node2D
{
	[Export] public int Type = 0;
	[Export] Sprite2D BG;
	[Export] Sprite2D BG1;
	[Export] Sprite2D BG2;
	[Export] Sprite2D BG3;
	[Export] Sprite2D Fie;
	[Export] Sprite2D Fie1;
	[Export] Sprite2D Fie2;
	[Export] Sprite2D Fie3;
	[Export] AudioStreamPlayer BGM0;
	[Export] AudioStreamPlayer BGM1;
	[Export] AudioStreamPlayer BGM2;
	[Export] AudioStreamPlayer BGM3;
	[Export] PackedScene Ball;
	[Export] PackedScene player;
	[Export] BallManager BM;
	[Export] StartTimer ST;
	[Export] Timer GT;
	[Export] Label GameO;
	[Export] private MultiplayerSpawner spawner;
	[Export] private MultiplayerSpawner Pspawner;
	MultiPlayerManager multiplayerManager;
	
	private bool BeenEnough = false;
	AudioStreamPlayer CurrentBGM;
	Random rand = new Random();
	
	private double[] XPos = {-180.4,-140.3,-100,-60.3,-20.3,20,59.7,99.7,139.7,180};
	private double[] YPos = {-183,-143,-102.5,-63,-22.6,17,57,97,137,177.5};

	// Called when the node enters the scene tree for the first time.
	
	public override void _Ready()
	{
		multiplayerManager = GetNode<MultiPlayerManager>("/root/MultiPlayerManager");
		Multiplayer.MultiplayerPeer = multiplayerManager.peer;
		Multiplayer.MultiplayerPeer.Poll();
		spawner.SpawnFunction = new Callable(this,nameof(BallSpawn));
		Pspawner.SpawnFunction = new Callable(this,nameof(PlayerSpawn));
		spawner.SetMultiplayerAuthority(1); // 服务端权限
		Pspawner.SetMultiplayerAuthority(1);	
		switch(Type){
			case 0: BG.Visible = true; Fie.Visible = true; CurrentBGM=BGM0; break;
			case 1: BG1.Visible = true; Fie1.Visible = true; CurrentBGM=BGM1; break;
			case 2: BG2.Visible = true; Fie2.Visible = true; CurrentBGM=BGM2; break;
			case 3: BG3.Visible = true; Fie3.Visible = true; CurrentBGM=BGM3; break;
		}
		CurrentBGM.Play();
		if (Multiplayer.IsServer()) CallDeferred(nameof(ServerStart));
		GetTree().Paused = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!BeenEnough && (Multiplayer.IsServer()))
		{
			if (BM.BallCnt < 26){
				var BallData = CreateBall(1,9);
				spawner.Spawn(BallData);
			} 
			else BeenEnough = true;
		} 
		else if (BM.BallCnt<26 && (Multiplayer.IsServer())) {
			var BallData = CreateBall(1,9);
			spawner.Spawn(BallData);
		}
	}
	
	public Godot.Collections.Dictionary CreateBall(int Left,int Right){
		bool IsInCorner = false;
		int X = rand.Next(Left,Right);
		int Y = rand.Next(Left,Right);
		if ((X == 0 || X == 9) && (Y == 0 || Y == 9)) IsInCorner=true;
		if (IsInCorner || BM.IsBalled[X][Y]){
			return new Godot.Collections.Dictionary{
			{"Position",new Vector2(99999,99999)},
			{"Type",0},
			{"BX",X},
			{"BY",Y}
		} ;
		}
		Vector2 Pos = new Vector2((float)XPos[X],(float)YPos[Y]);
		int BType = BallSelect();
		BM.IsBalled[X][Y] = true;
		BM.BallCnt++;
		return new Godot.Collections.Dictionary{
			{"Position",Pos},
			{"Type",BType},
			{"BX",X},
			{"BY",Y}
		} ;
	}
	
	public int BallSelect(){
		if (rand.Next(0,150) < 50){
			int r = rand.Next(0,100);
			switch(Type){
				case 0:
					if (r<80) return 4;
					else return 5;	
				case 1:
					if (r<80) return 5;
					else return 6;
				case 2:
					if (r < 60) return 6;
					else if (60 <= r && r < 81) return 5;	
					else return 7;
				case 3:				
					if (r < 50) return 7;
					else if (50 <= r && r < 70) return 6;
					else if (70 <= r && r < 90) return 5;
					else return 4;
				default:
					return 0;
			}
		}
		else return rand.Next(0,4);
		}
	
	private Node BallSpawn(Variant data){
		var Dict = (Godot.Collections.Dictionary)data;	
		var ball = (Balls)Ball.Instantiate();
		ball.Position = (Vector2)Dict["Position"];
		ball.BType = (int)Dict["Type"];
		ball.BornX = (int)Dict["BX"];
		ball.BornY = (int)Dict["BY"];
		return ball;
	}
	
	public Godot.Collections.Dictionary CreatePlayer(int Index,long ID,int Type){
		return new Godot.Collections.Dictionary{
			{"I",Index},
			{"ID",ID},
			{"Type",Type}
		};
	}
	
	private Node PlayerSpawn(Variant data){
		var Dict = (Godot.Collections.Dictionary)data;
		var P = (Player)player.Instantiate();	
		P.Index = (int)Dict["I"];
		P.Name = new StringName(((long)Dict["ID"]).ToString());
		P.CType = (int)Dict["Type"];
		P.SetMultiplayerAuthority((int)Dict["ID"]);
		return P;
	}
	
	public void ServerStart(){
		foreach (long uid in multiplayerManager.PlayerUID){
				Godot.Collections.Dictionary PlayerData = CreatePlayer(multiplayerManager.PlayerUID.IndexOf(uid),uid,multiplayerManager.CTypes[multiplayerManager.PlayerUID.IndexOf(uid)]);
				Pspawner.Spawn(PlayerData);
				}
		for (int i=0;i<26;i++){
			var BallData = CreateBall(1,9);
			spawner.Spawn(BallData);
			}
	}
}
