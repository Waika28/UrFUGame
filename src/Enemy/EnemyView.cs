namespace UrFUGame;

public class EnemyView : BaseView
{
    public EnemyView(Texture2D image) =>
        Image = image;

    public override void SetImage()
    { }

    public override void Draw(SpriteBatch spriteBatch, BaseModel baseModel)
    {
        var model = baseModel as EnemyModel;
        if (model.TimeUntilStart > 0)
        {
            float factor = model.TimeUntilStart / 60f;
            spriteBatch.Draw(Image, model.Position, null, Color.White * factor, model.Orientation, Size / 2f, 2 - factor, 0, 0);
        }

        base.Draw(spriteBatch, model);
    }
}