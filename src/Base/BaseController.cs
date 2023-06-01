namespace UrFUGame;

public abstract class BaseController
{
    public abstract BaseModel GetModel();
    public abstract void Update();
    public abstract void Draw(SpriteBatch spriteBatch);
}
