using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace UrFUGame
{
    public class GameRoot : Microsoft.Xna.Framework.Game
    {
        public static GameRoot Instance { get; private set; }
        public static Viewport Viewport { get { return Instance.GraphicsDevice.Viewport; } }
        public static Vector2 ScreenSize { get { return new Vector2(Viewport.Width, Viewport.Height); } }
        public static GameTime GameTime { get; private set; }
        public static ParticleManager<ParticleState> ParticleManager { get; private set; }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GameState gameState = GameState.Menu;

        public GameRoot()
        {
            Instance = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
        }

        protected override void Initialize()
        {
            base.Initialize();

            ParticleManager = new ParticleManager<ParticleState>(1024 * 20, ParticleState.UpdateParticle);

            const int maxGridPoints = 1600;
            Vector2 gridSpacing = new Vector2((float)Math.Sqrt(Viewport.Width * Viewport.Height / maxGridPoints));

            EntityManager.Add(PlayerShipController.Instance);

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.005f;
            MediaPlayer.Play(Sound.Music);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Art.Load(Content);
            Sound.Load(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            GameTime = gameTime;
            Input.Update();

            if (Input.WasKeyPressed(Keys.Escape))
                this.Exit();

            if (gameState == GameState.Menu)
            {
                if (Input.WasKeyPressed(Keys.Enter))
                    gameState = GameState.Playing;
            }
            else
            {


                if (Input.WasKeyPressed(Keys.P) && gameState == GameState.Playing)
                    gameState = GameState.Paused;

                if (Input.WasKeyPressed(Keys.P) && gameState == GameState.Paused)
                    gameState = GameState.Playing;

                if (gameState != GameState.Paused)
                {
                    PlayerStatus.Update();
                    EntityManager.Update();
                    EnemySpawner.Update();
                    ParticleManager.Update();
                }

            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (gameState == GameState.Menu)
            {
                var step = Art.Font.LineSpacing;
                var center = ScreenSize.Y / 2;
                spriteBatch.Begin();
                DrawCenterAlignedString("Press WASD to MOVE", center - 2 * step);
                DrawCenterAlignedString("Press P to PAUSE", center - step);
                DrawCenterAlignedString("And press ENTER to START", center);
                DrawCenterAlignedString("Then presss ESC to EXIT :(",center + step);
                spriteBatch.End();
                return;
            }

            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
            EntityManager.Draw(spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            ParticleManager.Draw(spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

            spriteBatch.DrawString(Art.Font, "Lives: " + PlayerStatus.Lives, new Vector2(5), Color.White);
            DrawRightAlignedString("Score: " + PlayerStatus.Score, 5);
            DrawRightAlignedString("Multiplier: " + PlayerStatus.Multiplier, 35);

            spriteBatch.Draw(Art.Pointer, Input.MousePosition, Color.White);

            if (PlayerStatus.IsGameOver)
            {
                string text = "Game Over\n" +
                    "Your Score: " + PlayerStatus.Score + "\n" +
                    "High Score: " + PlayerStatus.HighScore;

                Vector2 textSize = Art.Font.MeasureString(text);
                spriteBatch.DrawString(Art.Font, text, ScreenSize / 2 - textSize / 2, Color.White);
            }

            spriteBatch.End();
        }

        private void DrawRightAlignedString(string text, float y)
        {
            var textWidth = Art.Font.MeasureString(text).X;
            spriteBatch.DrawString(Art.Font, text, new Vector2(ScreenSize.X - textWidth - 5, y), Color.White);
        }

        private void DrawCenterAlignedString(string text, float y)
        {
            var textWidth = Art.Font.MeasureString(text).X;

            spriteBatch.DrawString(Art.Font, text, new Vector2((ScreenSize.X - textWidth) / 2, y), Color.White);
        }
    }
}
