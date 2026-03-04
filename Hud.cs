using Godot;
using System;
using System.Formats.Asn1;
using System.Runtime.ExceptionServices;

public partial class Hud : CanvasLayer
{
	[Signal]
	public delegate void StartGameEventHandler();
	public override void _Ready()
	{
	}
	public override void _Process(double delta)
	{
	}

	public void ShowMessage(string text)
	{
		var message = GetNode<Label>("Message");
		message.Text = text;
		message.Show();

		GetNode<Timer>("MessageTimer").Start();
	}

	async public void ShowGameOver()
	{
		ShowMessage("Game Over");

		GetNode<ProgressBar>("HealthBar").Hide();
		GetNode<Label>("HealthLabel").Hide();

        var messageTimer = GetNode<Timer>("MessageTimer");
		await ToSignal(messageTimer, Timer.SignalName.Timeout);

		var message = GetNode<Label>("Message");
		message.Text = "Desvie dos Bobo!";
		message.Show();

		await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
		GetNode<Button>("StartButton").Show();

	}

	public void UpdateScore(int score)
	{
		GetNode<Label>("ScoreLabel").Text = score.ToString();
	}

	public void UpdateHealth(int currentHealth, int maxHealth)
	{
		var healthBar = GetNode<ProgressBar>("HealthBar");
		var healthLabel = GetNode<Label>("HealthLabel");

		healthBar.MinValue = 0;
		healthBar.MaxValue = maxHealth;
		healthBar.Value = currentHealth;

        healthLabel.Text = $"HP: {currentHealth}/{maxHealth}";
		healthBar.Value = currentHealth;
		
		healthBar.Show();
		healthLabel.Show();
    }

    private void OnStartButtonPressed()
	{
		GetNode<Button>("StartButton").Hide();
		EmitSignal(SignalName.StartGame);
	}

	private void OnMessageTimerTimeout()
	{
		GetNode<Label>("Message").Hide();
	}
}
