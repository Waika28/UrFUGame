namespace UrFUGame;

public class PlayerShipModel : BaseModel
{
    public PlayerShipModel(Vector2 position, float radius)
    {
        Position = position;
        Radius = radius;
    }

    public int CooldownRemaining = 0;
    public int CooldownFrames = 25;
    public int FramesUntilRespawn = 0;
    public bool IsDead { get { return FramesUntilRespawn > 0;}}
    
}