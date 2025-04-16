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
    private float moveSpeed = 3f;
    private bool isJumping = false;  // To track if player is currently in the air
    private float jumpForce = -5f;   // Force applied when player jumps



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
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        var keyboard = Keyboard.GetState();

        // Horizontal movement
        if (keyboard.IsKeyDown(Keys.A))
        {
            playerPosition.X -= moveSpeed;
        }
        if (keyboard.IsKeyDown(Keys.D))
        {
            playerPosition.X += moveSpeed;
        }
        if (playerVelocity.Y > 0)
        {
            gravity *= gravityScale;
        }
        
        if ((keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Space)) && !isJumping)
        {
            playerVelocity.Y = jumpForce;  // Apply upward force
            isJumping = true;              // Player is now jumping
        }

        // Gravity
        playerVelocity.Y += gravity;
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
            isJumping = false; // Allow jumping again after landing
        }

        // Clamp to window horizontally
        playerPosition.X = MathHelper.Clamp(playerPosition.X, 0, GraphicsDevice.Viewport.Width - 8);

        // Debug
        Console.WriteLine($"Position: {playerPosition}, Velocity: {playerVelocity.Y}, Gravity: {gravity}");

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
