namespace UrFUGame;

public class PlayerShipView : BaseView
{
    public override void SetImage() =>
        Image = Art.Player;

    public override void Draw(SpriteBatch spriteBatch, BaseModel baseModel)
    {
        var model = baseModel as PlayerShipModel;
        if (!model.IsDead)
            base.Draw(spriteBatch, model);
    }
}