using System;
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
    private float gravity = 0.1f;        // Starting gravity
    private float gravityScale = 1.01f;  // Gravity multiplier per frame


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
        playerPosition = new Vector2(100,0);
        playerVelocity = new Vector2(0, 0);
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
        
        if (playerVelocity.Y > 0)
        {
            gravity *= gravityScale;
        }
        
        // Apply current gravity value
        playerVelocity.Y += gravity;
        
        // Optionally cap maximum gravity to prevent extreme values
        float maxGravity = 3.0f;
        if (gravity > maxGravity) gravity = maxGravity;
        
        // Update position based on velocity
        playerPosition += playerVelocity;
        
        // Debug output to see gravity scaling
        System.Diagnostics.Debug.WriteLine($"Gravity: {gravity}, Velocity: {playerVelocity.Y}");
        
        // Add floor collision
        if (playerPosition.Y > GraphicsDevice.Viewport.Height - 8)
        {
            playerPosition.Y = GraphicsDevice.Viewport.Height - 8;
            playerVelocity.Y = 0;
            gravity = 0.1f; // Reset gravity when landing
        }
        Console.WriteLine($"Position: {playerPosition}, Velocity: {playerVelocity}, Gravity: {gravity}");

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
