using Godot;
using System;

public partial class Room : Node2D
{
	[Export] public int PlayerCnt { get; set; }
	[Export] public int SceneType  { get; set; }
	[Export] public PackedScene gameScene;
	public int Type = 0;
	public int Index = 1;
	[Export] LineEdit Ip;
	[Export] LineEdit Port;
	[Export] Label Tip;
	[Export] Label SL;
	[Export] Label CL;
	[Export] AudioStreamPlayer CS;
	MultiPlayerManager multiplayerManager;
	[Export] AnimatedSprite2D P1;
	[Export] AnimatedSprite2D P2;
	[Export] AnimatedSprite2D P3;
	[Export] AnimatedSprite2D P4;
	[Export] AnimatedSprite2D Spot;
	[Export] Node2D Select;
	[Export] Node2D Display;
	string[] Characters = {"Rumia","Cirno","Bug","Bird"};
	
	//private int CharacterType = 0;
	//[Export] MultiplayerSynchronizer MPS;
	bool Flag = true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		multiplayerManager = GetNode<MultiPlayerManager>("/root/MultiPlayerManager");
		Multiplayer.PeerConnected += OnPeerConnected;
		Multiplayer.PeerDisconnected += OnPeerDisconnected;
		multiplayerManager.PlayerUID.Add(1);
		if (Type == 0) {
			Ip.Visible = false;	
			SL.Visible = true;
		}
		else CL.Visible = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Flag && Type == 0 && multiplayerManager.Port != -1) {
			multiplayerManager.CreateServer();
			P1.Visible = true;
			Spot.Visible = true;
			Select.Visible = true;
			Display.Visible = true;
			Flag = false;
			}
		else if (Flag && Type == 1 && multiplayerManager.Port != -1 && multiplayerManager.Ip != "" ){
			multiplayerManager.CreateClient();
			Spot.Visible = true;
			Select.Visible = true;
			Display.Visible = true;
			var groupNodes = GetTree().GetNodesInGroup("ServerButton");
			foreach (var node in groupNodes){
				if (node is Button B) B.Visible = false;
				if (node is TextureButton B2) B2.Visible = false;
			}
			Flag = false;
		}
		if (!Flag) ClearInput();
		switch (PlayerCnt){
			case 1: P1.Visible = true; break;
			case 2: P2.Visible = true; break;
			case 3: P3.Visible = true; break;
			case 4: P4.Visible = true; break;
		}
	}
	
	public void SubmitPort(string input){
		multiplayerManager.Port = int.Parse(input);
	}
	
	public void SubmitIp(string input){
		multiplayerManager.Ip = input;
	}
		
	public void ClearInput(){
		Ip.Visible = false;
		Port.Visible = false;
		Tip.Visible = false;
		SL.Visible = false;
		CL.Visible = false;
	}
	
	public void OnPeerConnected(long PeerID){
		CS.Play();
		PlayerCnt++;
		if(Multiplayer.IsServer())RpcId(PeerID,nameof(AssignIndex),PlayerCnt);
		multiplayerManager.PlayerUID.Add(PeerID);
	}
	
	public void OnPeerDisconnected(long PeerID){
		WhichToChange(multiplayerManager.PlayerUID.IndexOf(PeerID)).Visible = false;
		multiplayerManager.PlayerUID.Remove(PeerID);
		PlayerCnt--;
	}
	
	public AnimatedSprite2D WhichToChange(int i){
		switch(i){
			default: return P1; break;
			case 1: return P1; break;
			case 2: return P2; break;
			case 3: return P3; break;
			case 4: return P4; break;
		}
	}
	
	public void _on_rumia_button_down(){
		ChangeCharacter(0,Index);
		if (!Multiplayer.IsServer()){
			//RpcId(1,nameof(ChangeType),Index,0);
			RpcId(1,nameof(ChangeCharacter),0,Index);
		}
	}
	
	public void _on_cirno_button_down(){
		ChangeCharacter(1,Index);
		if (!Multiplayer.IsServer()){
			//RpcId(1,nameof(ChangeType),Index,1);
			RpcId(1,nameof(ChangeCharacter),1,Index);
		}
	}
	
	public void _on_bug_button_down(){
		ChangeCharacter(2,Index);
		if (!Multiplayer.IsServer()){
			//RpcId(1,nameof(ChangeType),Index,2);
			RpcId(1,nameof(ChangeCharacter),2,Index);
		}
	}
	
	public void _on_bird_button_down(){
		ChangeCharacter(3,Index);
		if (!Multiplayer.IsServer()){
			//RpcId(1,nameof(ChangeType),Index,3);
			RpcId(1,nameof(ChangeCharacter),3,Index);
		}
	}
	
	public void _on_s_1_button_down(){
		SceneType = 0;
		Spot.Play("0");
	}
	
	public void _on_s_2_button_down(){
		SceneType = 1;
		Spot.Play("1");
	}
	
	public void _on_s_3_button_down(){
		SceneType = 2;
		Spot.Play("2");
	}
	
	public void _on_s_4_button_down(){
		SceneType = 3;
		Spot.Play("3");
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void ChangeCharacter(int CT,int I){
		WhichToChange(I).Play(Characters[CT]);
	}
	
	[Rpc(MultiplayerApi.RpcMode.Authority)]
	public void AssignIndex(int i){
		Index = i;
	}
	
	[Rpc(MultiplayerApi.RpcMode.Authority)]
	public void StartGameServer(){
		GameManager game = (GameManager)gameScene.Instantiate();
		game.Type = SceneType;
		GetTree().Root.AddChild(game);
		multiplayerManager.CTypes[0] = ToType(P1);
		if (PlayerCnt > 1) multiplayerManager.CTypes[1] = ToType(P2);
		if (PlayerCnt > 2) multiplayerManager.CTypes[2] = ToType(P3);
		if (PlayerCnt > 3) multiplayerManager.CTypes[3] = ToType(P4);
		foreach (long uid in multiplayerManager.PlayerUID){
			if(uid!=1) RpcId(uid,nameof(StartGameCommon));
		}
			GetTree().CurrentScene.QueueFree();
			GetTree().CurrentScene = game;
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void StartGameCommon(){
		GameManager game = (GameManager)gameScene.Instantiate();
			game.Type = SceneType;
			GetTree().Root.AddChild(game);
			if (GetTree().CurrentScene is Node2D node) node.Visible = false;
			GetTree().CurrentScene.ProcessMode = Node.ProcessModeEnum.Disabled;
			GetTree().CurrentScene.QueueFree();
			GetTree().CurrentScene = game;
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void ChangeType(int I,int T){
		multiplayerManager.CTypes[I] = T;
	}
	
	public int ToType(AnimatedSprite2D Anim){
		switch(Anim.Animation){
			default: return 0; break;
			case "Rumia": return 0; break;
			case "Cirno": return 1; break;
			case "Bug":	return 2; break;
			case "Bird": return 3; break;
		}
	}
	
	/*public void Free(){
		if (GetTree().CurrentScene is Node2D node) node.Visible = false;
		GetTree().CurrentScene.ProcessMode = Node.ProcessModeEnum.Disabled;
	}*/
	
	/*public bool AllReady(){
		return multiplayerManager.IsReady[0] && multiplayerManager.IsReady[1] && multiplayerManager.IsReady[2] && multiplayerManager.IsReady[3];
	}*/
}
