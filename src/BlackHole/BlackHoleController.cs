namespace UrFUGame;

public class BlackHoleController : BaseController
{
    public BlackHoleView View { get; set; }
    public BlackHoleModel Model { get; set; }

    public override BaseModel GetModel() => Model;

    private static Random _random = new Random();

    public BlackHoleController(Vector2 position)
    {
        View = new BlackHoleView();
        Model = new BlackHoleModel(position, View.Size.X);
    }

    public override void Update()
    {
        var entities = EntityManager.GetNearbyEntities(Model.Position, 250);

        foreach (var entity in entities)
        {
            if (entity is EnemyController && !(entity as EnemyController).Model.IsActive)
                continue;

            if (entity is BulletController)
                entity.GetModel().Velocity += (entity.GetModel().Position - Model.Position).ScaleTo(0.3f);
            else
            {
                var dPos = Model.Position - entity.GetModel().Position;
                var length = dPos.Length();

                entity.GetModel().Velocity += dPos.ScaleTo(MathHelper.Lerp(2, 0, length / 250f));
            }
        }

        if ((GameRoot.GameTime.TotalGameTime.Milliseconds / 250) % 2 == 0)
        {
            Vector2 sprayVel = MathUtil.FromPolar(Model.SprayAngle, _random.NextFloat(12, 15));
            Color color = ColorUtil.HSVToColor(5, 0.5f, 0.8f);
            Vector2 pos = Model.Position + 2f * new Vector2(sprayVel.Y, -sprayVel.X) + _random.NextVector2(4, 8);
            var state = new ParticleState()
            {
                Velocity = sprayVel,
                LengthMultiplier = 1,
                Type = ParticleType.Enemy
            };

            GameRoot.ParticleManager.CreateParticle(Art.LineParticle, pos, color, 190, 1.5f, state);
        }

        Model.SprayAngle -= MathHelper.TwoPi / 50f;
    }

    public void WasShot()
    {
        Model.Hitpoints--;
        if (Model.Hitpoints <= 0)
        {
            Model.IsExpired = true;
            PlayerStatus.AddPoints(5);
            PlayerStatus.IncreaseMultiplier();
        }


        float hue = (float)((3 * GameRoot.GameTime.TotalGameTime.TotalSeconds) % 6);
        Color color = ColorUtil.HSVToColor(hue, 0.25f, 1);
        const int numParticles = 150;
        float startOffset = _random.NextFloat(0, MathHelper.TwoPi / numParticles);

        for (int i = 0; i < numParticles; i++)
        {
            Vector2 sprayVel = MathUtil.FromPolar(MathHelper.TwoPi * i / numParticles + startOffset, _random.NextFloat(8, 16));
            Vector2 pos = Model.Position + 2f * sprayVel;
            var state = new ParticleState()
            {
                Velocity = sprayVel,
                LengthMultiplier = 1,
                Type = ParticleType.IgnoreGravity
            };

            GameRoot.ParticleManager.CreateParticle(Art.LineParticle, pos, color, 90, 1.5f, state);
        }

        Sound.Explosion.Play(0.005f, _random.NextFloat(-0.2f, 0.2f), 0);
    }

    public void Kill()
    {
        Model.Hitpoints = 0;
        WasShot();
    }

    public override void Draw(SpriteBatch spriteBatch) => View.Draw(spriteBatch, Model);
}
