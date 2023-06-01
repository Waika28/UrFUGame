namespace UrFUGame;

public class BlackHoleModel : BaseModel
{
    public int Hitpoints { get; set; }
    public float SprayAngle { get; set; }

    public BlackHoleModel(Vector2 position, float width)
    {
        Position = position;
        Radius = width;
    }
}