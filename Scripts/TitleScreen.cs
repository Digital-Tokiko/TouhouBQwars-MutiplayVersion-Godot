using Godot;
using System;

public partial class TitleScreen : Node2D
{
	[Export] AnimationPlayer AnimManager;
	bool SelectAble = false;
	[Export] AudioStreamPlayer SelectS;
	[Export] Sprite2D S;
	[Export] Sprite2D C;
	[Export] PackedScene R;
	bool ZEnabled = false;
	int Type = 0; //0服务端，1客户端
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (SelectAble && (Input.IsActionJustPressed("down") || Input.IsActionJustPressed("up"))) ChangeSC();
		if (SelectAble && Input.IsActionJustPressed("Kick")) Confirm();
	}
	
	public void ChangeMenu(){
		if (SelectAble) return;
		AnimManager.Play("CameraMove");
		SelectAble = true;
	}
	
	public void ChangeSC(){
		S.Visible = !S.Visible;
		C.Visible = !C.Visible;
		Type = 1 - Type;
	}
	
	public void Confirm(){
		SelectS.Play();
		Room room = (Room)R.Instantiate();
		room.Type = Type;
		GetTree().Root.AddChild(room);
		GetTree().CurrentScene.QueueFree();
		GetTree().CurrentScene = room;
	}
	
}
