using Godot;

public partial class Player : Area2D
{
    #region Signals
    
	[Signal]
	public delegate void HitEventHandler(); //Colisão do player com um inimigo

	[Signal]
	public delegate void PlayerHealthEventHandler(int currentPlayerHealth, int MaxPlayerHealth);

    #endregion

    #region Exports

    [Export]
	private bool _isPlayerInvincible { get; set; }
	
	[Export]
	public float PlayerInvincibilityDuration { get; set; } = 1.0f;
	
	[Export]
	public int Speed { get; set; } = 400; //Velocidade do player

	#endregion

    #region Variáveis declaradas

    public Vector2 ScreenSize;    

	private const int _maxPlayerHealth = 3; 
	private int _currentPlayerHealth;
	
    public int MaxPlayerHealth => _maxPlayerHealth;
	public int CurrentPlayerHealth => _currentPlayerHealth;
    #endregion

    public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size; //Obtém o tamanho da tela do jogo
	}

	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero; //Seta a velocidade do jogador como zero;

		if (Input.IsActionPressed("move_right"))
		{
			velocity.X += 1;
		}

		if (Input.IsActionPressed("move_left"))
		{
			velocity.X -= 1;
		}

		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y += 1;
		}

		if (Input.IsActionPressed("move_up"))
		{
			velocity.Y -= 1;
		}

		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed; //Normaliza a velocidade e multiplica pela velocidade do jogador
			animatedSprite2D.Play();
		}
		else
		{
			animatedSprite2D.Stop();
		}

		Position += velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
			y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		);

		if (velocity.X != 0)
		{
			animatedSprite2D.Animation = "walk";
			animatedSprite2D.FlipV = false; //Não inverte a animação verticalmente
			animatedSprite2D.FlipH = velocity.X < 0; //Inverte a animação horizontalmente se o jogador se mover para a esquerda usando FlipH;
		}
		else if (velocity.Y != 0)
		{
			animatedSprite2D.Animation = "up";
			animatedSprite2D.FlipV = velocity.Y > 0; //Inverte a animação se o jogador se mover para baixo usando FlipV;
		}

	}
	private void OnBodyEntered(Node2D body)
	{			
		if(!_isPlayerInvincible)
		{
			_currentPlayerHealth -= 1;
            EmitSignal(SignalName.PlayerHealth, _currentPlayerHealth, _maxPlayerHealth);
		
            if (_currentPlayerHealth == 0)
			{ 
				Hide();
				EmitSignal(SignalName.Hit);
				GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true); //Desativa a colisão do jogador para evitar múltiplas colisões;
			}
			else
			{
				StartInvincibility();
			}
		}
    }

	private void StartInvincibility()
	{
		_isPlayerInvincible = true;

		var sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");	
        Tween tween = CreateTween();

		int loops = (int)(PlayerInvincibilityDuration / 0.5f);
		
		tween.SetLoops(loops);
        tween.TweenProperty(sprite, "modulate", new Color(1, 0, 0, 1), 0.25f);
		tween.TweenProperty(sprite, "modulate", new Color(1, 1, 1, 1), 0.25f);

		tween.SetLoops(1);
		tween.TweenCallback(Callable.From(EndInvincibility));
    }

	private void EndInvincibility()
	{
		_isPlayerInvincible = false;
    }

    public void Start(Vector2 position)
	{
		Position = position; //Posição inicial do jogador;
		_currentPlayerHealth = _maxPlayerHealth; //Reseta a vida do jogador para o valor máximo;
        Show(); //Mostra o jogador na tela;
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false; //Ativa a colisão do jogador para permitir colisões com inimigos;
    }
}