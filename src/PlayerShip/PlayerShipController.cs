namespace UrFUGame;

public class PlayerShipController : BaseController
{
    public PlayerShipView View { get; set; }
    public PlayerShipModel Model { get; set; }

    public override BaseModel GetModel() => Model;

    private Random _random = new Random();

    private static PlayerShipController _instance;

    public static PlayerShipController Instance
    {
        get
        {
            if (_instance is null)
                _instance = new PlayerShipController();

            return _instance;
        }
    }

    public PlayerShipController() {
        View = new PlayerShipView();
        Model = new PlayerShipModel(GameRoot.ScreenSize / 2, 10);
    }


    public override void Update()
    {
        if (Model.IsDead)
        {
            if (--Model.FramesUntilRespawn == 0)
            {
                if (PlayerStatus.Lives == 0)
                {
                    PlayerStatus.Reset();
                    Model.Position = GameRoot.ScreenSize / 2;
                }
            }

            return;
        }

        var aim = Input.GetAimDirection();
        if (aim.LengthSquared() > 0 && Model.CooldownRemaining <= 0)
        {
            Model.CooldownRemaining = Model.CooldownFrames;
            float aimAngle = aim.ToAngle();
            Quaternion aimQuat = Quaternion.CreateFromYawPitchRoll(0, 0, aimAngle);

            float randomSpread = _random.NextFloat(-0.04f, 0.04f) + _random.NextFloat(-0.04f, 0.04f);
            Vector2 vel = MathUtil.FromPolar(aimAngle + randomSpread, 11f);

            Vector2 offset = Vector2.Transform(new Vector2(35, -8), aimQuat);
            EntityManager.Add(new BulletController(Model.Position + offset, vel));

            offset = Vector2.Transform(new Vector2(35, 8), aimQuat);
            EntityManager.Add(new BulletController(Model.Position + offset, vel));

            Sound.Shot.Play(0.002f, _random.NextFloat(-0.2f, 0.2f), 0);
        }

        if (Model.CooldownRemaining > 0)
            Model.CooldownRemaining--;

        const float speed = 8;
        Model.Velocity += speed * Input.GetMovementDirection();
        Model.Position += Model.Velocity;
        Model.Position = Vector2.Clamp(Model.Position, View.Size / 2, GameRoot.ScreenSize - View.Size / 2);

        if (Model.Velocity.LengthSquared() > 0)
            Model.Orientation = Model.Velocity.ToAngle();

        MakeExhaustFire();
        Model.Velocity = Vector2.Zero;
    }

    private void MakeExhaustFire()
    {
        if (Model.Velocity.LengthSquared() > 0.1f)
        {
            Model.Orientation = Model.Velocity.ToAngle();
            Quaternion rot = Quaternion.CreateFromYawPitchRoll(0f, 0f, Model.Orientation);

            double t = GameRoot.GameTime.TotalGameTime.TotalSeconds;

            Vector2 baseVel = Model.Velocity.ScaleTo(-3);

            Vector2 perpVel = new Vector2(baseVel.Y, -baseVel.X) * (0.6f * (float)Math.Sin(t * 10));
            Color sideColor = new Color(200, 38, 9);
            Color midColor = new Color(255, 187, 30);
            Vector2 pos = Model.Position + Vector2.Transform(new Vector2(-25, 0), rot);
            const float alpha = 0.7f;

            Vector2 velMid = baseVel + _random.NextVector2(0, 1);
            GameRoot.ParticleManager.CreateParticle(Art.LineParticle, pos, Color.White * alpha, 60f, new Vector2(0.5f, 1),
                new ParticleState(velMid, ParticleType.Enemy));
            GameRoot.ParticleManager.CreateParticle(Art.Glow, pos, midColor * alpha, 60f, new Vector2(0.5f, 1),
                new ParticleState(velMid, ParticleType.Enemy));

            Vector2 vel1 = baseVel + perpVel + _random.NextVector2(0, 0.3f);
            Vector2 vel2 = baseVel - perpVel + _random.NextVector2(0, 0.3f);
            GameRoot.ParticleManager.CreateParticle(Art.LineParticle, pos, Color.White * alpha, 60f, new Vector2(0.5f, 1),
                new ParticleState(vel1, ParticleType.Enemy));
            GameRoot.ParticleManager.CreateParticle(Art.LineParticle, pos, Color.White * alpha, 60f, new Vector2(0.5f, 1),
                new ParticleState(vel2, ParticleType.Enemy));

            GameRoot.ParticleManager.CreateParticle(Art.Glow, pos, sideColor * alpha, 60f, new Vector2(0.5f, 1),
                new ParticleState(vel1, ParticleType.Enemy));
            GameRoot.ParticleManager.CreateParticle(Art.Glow, pos, sideColor * alpha, 60f, new Vector2(0.5f, 1),
                new ParticleState(vel2, ParticleType.Enemy));
        }
    }

    public void Kill()
    {
        PlayerStatus.RemoveLife();
        Model.FramesUntilRespawn = PlayerStatus.IsGameOver ? 300 : 120;

        Color explosionColor = new Color(0.8f, 0.8f, 0.4f);

        for (int i = 0; i < 1200; i++)
        {
            float speed = 18f * (1f - 1 / _random.NextFloat(1f, 10f));
            Color color = Color.Lerp(Color.White, explosionColor, _random.NextFloat(0, 1));
            var state = new ParticleState()
            {
                Velocity = _random.NextVector2(speed, speed),
                Type = ParticleType.None,
                LengthMultiplier = 1
            };

            GameRoot.ParticleManager.CreateParticle(Art.LineParticle, Model.Position, color, 190, 1.5f, state);
        }
    }

    public override void Draw(SpriteBatch spriteBatch) => View.Draw(spriteBatch, Model);
}