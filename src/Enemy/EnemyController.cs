namespace UrFUGame;

public class EnemyController : BaseController
{
    public EnemyView View { get; set; }
    public EnemyModel Model { get; set; }

    public override BaseModel GetModel() => Model;
    private static Random _random = new Random();

    private EnemyController(EnemyModel model, EnemyView view)
    {
        Model = model;
        View = view;
    }

    public static EnemyController CreateSeekerController(Vector2 position)
    {
        var view = new EnemyView(Art.Seeker);
        var model = EnemyModel.CreateSeeker(position, view.Size);
        return new EnemyController(model, view);
    }

    public static EnemyController CreateWandererController(Vector2 position)
    {
        var view = new EnemyView(Art.Wanderer);
        var model = EnemyModel.CreateWanderer(position, view.Size);
        return new EnemyController(model, view);
    }

    public override void Update()
    {
        if (Model.TimeUntilStart <= 0)
            Model.ApplyBehaviours();
        else
        {
            Model.TimeUntilStart--;
            View.Color = Color.White * (1 - Model.TimeUntilStart / 60f);
        }

        Model.Position += Model.Velocity;
        Model.Position = Vector2.Clamp(Model.Position, View.Size / 2, GameRoot.ScreenSize - View.Size / 2);

        Model.Velocity *= 0.8f;
    }

    public void HandleCollision(EnemyController other)
    {
        var d = Model.Position - other.Model.Position;
        Model.Velocity += 10 * d / (d.LengthSquared() + 1);
    }

    public void WasShot()
    {
        Model.IsExpired = true;
        PlayerStatus.AddPoints(Model.PointValue);
        PlayerStatus.IncreaseMultiplier();

        float hue1 = _random.NextFloat(0, 6);
        float hue2 = (hue1 + _random.NextFloat(0, 2)) % 6f;
        Color color1 = ColorUtil.HSVToColor(hue1, 0.5f, 1);
        Color color2 = ColorUtil.HSVToColor(hue2, 0.5f, 1);

        for (int i = 0; i < 120; i++)
        {
            float speed = 18f * (1f - 1 / _random.NextFloat(1, 10));
            var state = new ParticleState()
            {
                Velocity = _random.NextVector2(speed, speed),
                Type = ParticleType.Enemy,
                LengthMultiplier = 1
            };

            Color color = Color.Lerp(color1, color2, _random.NextFloat(0, 1));
            GameRoot.ParticleManager.CreateParticle(Art.LineParticle, Model.Position, color, 190, 1.5f, state);
        }

        Sound.Explosion.Play(0.005f, _random.NextFloat(-0.2f, 0.2f), 0);
    }

    public override void Draw(SpriteBatch spriteBatch) => View.Draw(spriteBatch, Model);
}