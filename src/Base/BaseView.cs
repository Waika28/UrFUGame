namespace UrFUGame;

public abstract class BaseView
{
    public Texture2D Image { get; set; }
    public Color Color { get; set; } = Color.White;

    public Vector2 Size => Image == null ? Vector2.Zero : new Vector2(Image.Width, Image.Height);

    public BaseView()
    {
        SetImage();
    }

    public abstract void SetImage();
    public virtual void Draw(SpriteBatch spriteBatch, BaseModel model)
    {
        spriteBatch.Draw(Image, model.Position, null, Color, model.Orientation, Size / 2f, 1f, 0, 0);
    }
}
