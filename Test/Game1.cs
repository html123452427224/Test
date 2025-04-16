using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Test;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    
    private Vector2 playerPosition;
    private Vector2 playerVelocity;
    private float gravity = 0.5f;  // Gravity acceleration per frame

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }
    SpriteBatch spriteBatch;
    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        spriteBatch = new SpriteBatch(GraphicsDevice);
        playerPosition = new Vector2(100, 100);
        playerVelocity = Vector2.Zero;
        base.Initialize();
    }
    Texture2D myTexture;

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        myTexture = Content.Load<Texture2D>("spritesheet");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        playerVelocity.Y += gravity;
        
        // Apply velocity to position
        playerPosition += playerVelocity;
       

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Begin drawing with the sprite batch
        spriteBatch.Begin();

        // Draw the texture at the position (100, 100)
        spriteBatch.Draw(
            myTexture,                      // The spritesheet texture
            playerPosition,          // Position to draw at
            new Rectangle(0, 0, 8, 8),      // Source rectangle (top-left 8x8 sprite)
            Color.White                     // Color tint (white means no tint)
        );

        // End drawing
        spriteBatch.End();

        base.Draw(gameTime);
    }
}
