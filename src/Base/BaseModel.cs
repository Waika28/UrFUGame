namespace UrFUGame;

public abstract class BaseModel
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public float Orientation { get; set; }
    public float Radius { get; set; } = 20;
    public bool IsExpired { get; set; }
}