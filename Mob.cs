using Godot;
using System;

public partial class Mob : RigidBody2D
{
	public override void _Ready()
	{
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D"); // Obtém o node do inimigo;
		string[] mobTypes = animatedSprite2D.SpriteFrames.GetAnimationNames(); //Pega as variantes dos inimigos e adiciona a um array;
		animatedSprite2D.Play(mobTypes[GD.Randi() % mobTypes.Length]); // Randomiza a animação do inimigo usando GD.Randi();
    }
	public override void _Process(double delta)
	{
	}
	
	private void OnVisibleOnScreenNotifier2DScreenExited()
	{
		QueueFree(); //Libera o inimigo da memória quando ele sair da tela usando QueueFree();
    }
}
