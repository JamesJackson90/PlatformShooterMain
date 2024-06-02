using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;

namespace PlatformShooter
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Dictionary<Vector2, int> fg;
        private Texture2D textureAtlas;
        private Texture2D rectangleTexture;
        private Sprite player;
        private int TILESIZE = 32;
        private Dictionary<Vector2, int> collisions;
        private const float GRAVITY = 0.1f; // Adjust gravity strength as needed

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            fg = LoadMap("../../../Data/level1_Tile Layer 1.csv");
        }

        private Dictionary<Vector2, int> LoadMap(string filepath)
        {
            Dictionary<Vector2, int> result = new();

            StreamReader reader = new(filepath);

            int y = 0;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] items = line.Split(',');

                for (int x = 0; x < items.Length; x++)
                {
                    if (int.TryParse(items[x], out int value))
                    {
                        if (value > -1)
                        {
                            result[new Vector2(x, y)] = value;
                        }
                    }
                }
                y++;
            }
            return result;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1480;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            textureAtlas = Content.Load<Texture2D>("Tileset");
            rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            rectangleTexture.SetData(new Color[] { new Color(255, 0, 0, 255) });

            Texture2D playerTexture = Content.Load<Texture2D>("Player/Biker_run");
            SpriteAnimation animation = new SpriteAnimation(playerTexture, 6, 10); // Assuming 8 frames, 10 FPS
            player = new Sprite(animation, new Rectangle(TILESIZE, TILESIZE, TILESIZE, TILESIZE * 2));
            collisions = LoadMap("../../../Data/MapOutput_Collisions.csv"); // Load collision map
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState state = Keyboard.GetState();
            player.Update(state, gameTime, collisions, TILESIZE);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Blue);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            int display_tilesize = 32;
            int num_tiles_per_row = 12;
            int pixel_tilesize = 32;

            foreach (var item in fg)
            {
                Rectangle drect = new(
                    (int)item.Key.X * display_tilesize,
                    (int)item.Key.Y * display_tilesize,
                    display_tilesize,
                    display_tilesize
                );
                int x = item.Value % num_tiles_per_row;
                int y = item.Value / num_tiles_per_row;

                Rectangle src = new(x * pixel_tilesize, y * pixel_tilesize, pixel_tilesize, pixel_tilesize);

                _spriteBatch.Draw(textureAtlas, drect, src, Color.White);
            }

            player.Draw(_spriteBatch);
            DrawRectHollow(_spriteBatch, player.Rect, 4);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void DrawRectHollow(SpriteBatch spriteBatch, Rectangle rect, int thickness)
        {
            spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.X,
                    rect.Y,
                    rect.Width,
                    thickness
                ),
                Color.White
            );
            spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.X,
                    rect.Bottom - thickness,
                    rect.Width,
                    thickness
                ),
                Color.White
            );
            spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.X,
                    rect.Y,
                    thickness,
                    rect.Height
                ),
                Color.White
            );
            spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.Right - thickness,
                    rect.Y,
                    thickness,
                    rect.Height
                ),
                Color.White
            );
        }
    }
}