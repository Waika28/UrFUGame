namespace UrFUGame;

public class BulletController : BaseController
{
    public BulletView View { get; set; }
    public BulletModel Model { get; set; }

    public override BaseModel GetModel() => Model;
    
    private static Random _random = new Random();

    public BulletController(Vector2 position, Vector2 velocity)
    {
        View = new BulletView();
        Model = new BulletModel(position, velocity);
    }

    public override void Update()
    {
        if (Model.Velocity.LengthSquared() > 0)
            Model.Orientation = Model.Velocity.ToAngle();

        Model.Position += Model.Velocity;

        if (!GameRoot.Viewport.Bounds.Contains(Model.Position.ToPoint()))
        {
            Model.IsExpired = true;

            for (int i = 0; i < 30; i++)
                GameRoot.ParticleManager.CreateParticle(Art.LineParticle, Model.Position, Color.LightBlue, 50, 1,
                    new ParticleState() { Velocity = _random.NextVector2(0, 9), Type = ParticleType.Bullet, LengthMultiplier = 1 });

        }
    }
    public override void Draw(SpriteBatch spriteBatch) => View.Draw(spriteBatch, Model);
}