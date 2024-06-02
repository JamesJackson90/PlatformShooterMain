using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace PlatformShooter
{
    public class Sprite
    {
        public SpriteAnimation Animation { get; private set; }
        public Rectangle Rect { get; set; }
        public Vector2 Velocity { get; set; }

       public Sprite(SpriteAnimation animation, Rectangle rect)
        {
            Animation = animation;
            int frameWidth = animation.Texture.Width / 10;
            int frameHeight = animation.Texture.Height - 2;
            Rect = new Rectangle(rect.X, rect.Y, frameWidth, frameHeight);
            Velocity = Vector2.Zero;
        }

        public void Update(KeyboardState keystate, GameTime gameTime, Dictionary<Vector2, int> collisions, int tileSize)
        {
            var newRect = Rect;
            var newVelocity = Velocity;

            // Reset velocity
            newVelocity = Vector2.Zero;

            if (keystate.IsKeyDown(Keys.Right))
            {
                newVelocity.X = 5;
            }
            else if (keystate.IsKeyDown(Keys.Left))
            {
                newVelocity.X = -5;
            }

            if (keystate.IsKeyDown(Keys.Up))
            {
                newVelocity.Y = -5;
            }
            else if (keystate.IsKeyDown(Keys.Down))
            {
                newVelocity.Y = 5;
            }

            // Update X position
            newRect.X += (int)newVelocity.X;
            if (!CheckCollision(newRect, collisions, tileSize))
            {
                Rect = newRect;
            }

            // Update Y position
            newRect.Y += (int)newVelocity.Y;
            if (!CheckCollision(newRect, collisions, tileSize))
            {
                Rect = newRect;
            }

            Velocity = newVelocity;
            Animation.Update(gameTime);
        }

        private bool CheckCollision(Rectangle rect, Dictionary<Vector2, int> collisions, int tileSize)
        {
            // Check collisions based on the tile map
            int leftTile = rect.Left / tileSize;
            int rightTile = (rect.Right - 1) / tileSize; // Subtracting 1 to prevent crossing two tiles
            int topTile = rect.Top / tileSize;
            int bottomTile = (rect.Bottom - 1) / tileSize; // Subtracting 1 to prevent crossing two tiles

            for (int x = leftTile; x <= rightTile; x++)
            {
                for (int y = topTile; y <= bottomTile; y++)
                {
                    // Check if the tile at (x, y) is solid
                    if (collisions.ContainsKey(new Vector2(x, y)))
                    {
                        return true; // Collision detected
                    }
                }
            }
            return false; // No collision detected
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Animation.Position = new Vector2(Rect.X, Rect.Y);
            Animation.Draw(spriteBatch);
        }
    }
}