namespace UrFUGame;

public class EnemyModel : BaseModel
{
    public List<IEnumerator<int>> Behaviours { get; } = new List<IEnumerator<int>>();
    public int TimeUntilStart { get; set; } = 60;
    public bool IsActive { get { return TimeUntilStart <= 0; } }
    public int PointValue { get; set; }

    private Vector2 _size;
    private static Random _random = new Random();

    private EnemyModel(Vector2 position, Vector2 size)
    {
        Position = position;
        _size = size;
        Radius = _size.X;
        PointValue = 1;
    }
    
    public static EnemyModel CreateSeeker(Vector2 position, Vector2 size) {
        var enemy = new EnemyModel(position, size);
        enemy.AddBehaviour(enemy.FollowPlayer(0.9f));
        enemy.PointValue = 2;

        return enemy;
    }
    
    public static EnemyModel CreateWanderer(Vector2 position, Vector2 size) {
        var enemy = new EnemyModel(position, size);
        enemy.AddBehaviour(enemy.MoveRandomly());

        return enemy;
    }

    public void AddBehaviour(IEnumerable<int> behaviour)
    {
        Behaviours.Add(behaviour.GetEnumerator());
    }

    public void ApplyBehaviours()
    {
        for (int i = 0; i < Behaviours.Count; i++)
        {
            if (!Behaviours[i].MoveNext())
                Behaviours.RemoveAt(i--);
        }
    }

    #region Behaviours
    public IEnumerable<int> FollowPlayer(float acceleration)
    {
        while (true)
        {
            if (!PlayerShipController.Instance.Model.IsDead)
                Velocity += (PlayerShipController.Instance.Model.Position - Position).ScaleTo(acceleration);

            if (Velocity != Vector2.Zero)
                Orientation = Velocity.ToAngle();

            yield return 0;
        }
    }

    public IEnumerable<int> MoveRandomly()
    {
        float direction = _random.NextFloat(0, MathHelper.TwoPi);

        while (true)
        {
            direction += _random.NextFloat(-0.1f, 0.1f);
            direction = MathHelper.WrapAngle(direction);

            for (int i = 0; i < 6; i++)
            {
                Velocity += MathUtil.FromPolar(direction, 0.4f);
                Orientation += 0.2f;

                var bounds = GameRoot.Viewport.Bounds;
                bounds.Inflate(-_size.X / 2 - 1, -_size.Y / 2 - 1);

                if (!bounds.Contains(Position.ToPoint()))
                    direction = (GameRoot.ScreenSize / 2 - Position).ToAngle() + _random.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);

                yield return 0;
            }
        }
    }
    #endregion
}