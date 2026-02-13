using Godot;
using System;
using System.Collections.Generic;

public partial class MultiPlayerManager : Node2D
{
	public static MultiPlayerManager Instance { get; private set; }
	public int Port = -1;
	public string Ip = "";
	public ENetMultiplayerPeer peer = new ENetMultiplayerPeer();
	public int[] CTypes = {0,0,0,0};
	public List<long> PlayerUID = new List<long>();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void CreateServer(){
		peer.CreateServer(Port,4);
		Multiplayer.MultiplayerPeer = peer;
	}
	
	public void CreateClient(){
		peer.CreateClient(Ip,Port);
		Multiplayer.MultiplayerPeer = peer;
	}
}
