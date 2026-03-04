using Godot;
using System;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;

public partial class Main : Node
{
    [Export]
    public PackedScene MobScene { get; set; }

    private int _score;
    public override void _Ready()
	{
		GetNode<Area2D>("Player").Hide();
	}

	public override void _Process(double delta)
	{
	}

	public void GameOver()
	{
		//Game over para os timers de mobs e pontuação;
		GetNode<Timer>("MobTimer").Stop(); 
		GetNode<Timer>("ScoreTimer").Stop();
		GetNode<Hud>("HUD").ShowGameOver();

		GetNode<AudioStreamPlayer2D>("Music").Stop();
		GetNode<AudioStreamPlayer2D>("DeathSound").Play();
    }

	public void NewGame()
	{
        //NewGame inicia a pontuação, reseta a posição do player e inicia o timer de início do jogo;
        _score = 0;
		
        var player = GetNode<Player>("Player");
		var startPosition = GetNode<Marker2D>("StartPosition");
		player.Start(startPosition.Position);

		GetNode<Timer>("StartTimer").Start();

		var hud = GetNode<Hud>("HUD");

		hud.UpdateScore(_score);
		
		hud.ShowMessage("Se ligue!");
		
		hud.UpdateHealth(player.CurrentPlayerHealth, player.MaxPlayerHealth);
        
		GetTree().CallGroup("mobs", Node.MethodName.QueueFree);
		GetNode<AudioStreamPlayer2D>("Music").Play();

    }

	private void OnScoreTimerTimeout()
	{
        //Incrementa a pontuação a cada vez que o timer de pontuação atinge o tempo limite usando _score++;
        _score++;
		GetNode<Hud>("HUD").UpdateScore(_score);
	}

	private void OnStartTimerTimeout()
	{
        //Inicia o timer de mobs e pontuação quando o timer de início do jogo atinge o tempo limite usando Start();
        GetNode<Timer>("MobTimer").Start();
        GetNode<Timer>("ScoreTimer").Start();
    }

	private void OnMobTimerTimeout()
	{
        //Instancia um novo mob a cada vez que o timer de mobs atinge o tempo limite usando MobScene.Instantiate<Mob>();
        Mob mob = MobScene.Instantiate<Mob>();

		//Define um spawn para o mob usando o PathFollow configurado na engine;
		var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		mobSpawnLocation.ProgressRatio = GD.Randf(); //Randomiza a posição de spawn;

		float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2; //Calcula a direção do mob usando a rotação do spawn e adicionando Pi/2 para ajustar a direção;

		mob.Position = mobSpawnLocation.Position; //Define a posição do mob para a posição do spawn;

		//Randomiza a direção do inimigo;
		direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
		mob.Rotation = direction;

        //Randomiza a velocidade do mob;
        var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
		mob.LinearVelocity = velocity.Rotated(direction); //Define a velocidade do mob usando a velocidade randomizada e rotacionada para a direção do mob;

		AddChild(mob); //Adiciona o mob à cena;
    }

	private void OnPlayerPlayerHealth(int currentPlayerHealth, int maxPlayerHealth)
	{
		GetNode<Hud>("HUD").UpdateHealth(currentPlayerHealth, maxPlayerHealth);
	}
}
