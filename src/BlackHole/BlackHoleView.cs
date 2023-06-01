namespace UrFUGame;

public class BlackHoleView : BaseView
{
    public override void SetImage() =>
        Image = Art.BlackHole;

    public override void Draw(SpriteBatch spriteBatch, BaseModel model)
    {
        float scale = 1 + 0.1f * (float)Math.Sin(10 * GameRoot.GameTime.TotalGameTime.TotalSeconds);
        spriteBatch.Draw(Image, model.Position, null, Color, model.Orientation, Size / 2f, scale, 0, 0);
    }
}