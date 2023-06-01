namespace UrFUGame;

public class BulletModel : BaseModel
{
    public BulletModel(Vector2 position, Vector2 velocity)
    {
        Position = position;
        Velocity = velocity;
        Orientation = Velocity.ToAngle();
        Radius = 8;
    }
}