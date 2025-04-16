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
    private float gravity = 0.1f; // Starting gravity
    private float gravityScale = 1.05f; // Gravity multiplier per frame
    private float moveSpeed = 3f;

    private bool isJumping = false; // To track if player is currently in the air
    private float jumpForce = -5f; // Force applied when player jumps
    private Matrix cameraTransform;


    // Tile parameters
    private Texture2D tileSheet; // The full texture of the sprite sheet
    private int tileSize = 8; // Assuming each tile is 8x8
    private Rectangle grassTileSource; // The portion of the sprite sheet for the grass tile
    private Rectangle dirtTileSource; // The portion of the sprite sheet for the dirt tile
    private Rectangle playerTileSource; // The portion of the sprite sheet for the player sprite

    // Platform positions (start and end x, y for each platform)
    private Rectangle platform1; // Platform 1
    private Rectangle platform2; // Platform 2
    private Rectangle platform3;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    SpriteBatch spriteBatch;

    protected override void Initialize()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        playerPosition = new Vector2(100, 0);
        playerVelocity = new Vector2(0, 0);

        // Platform positions
        platform1 = new Rectangle(50, 100, 80, 8); // Platform 1 at (50, 100) with width of 80 tiles
        platform2 = new Rectangle(200, 150, 80, 8); // Platform 2 at (200, 150) with width of 80 tiles
        platform3 = new Rectangle(300, 200, 80, 8);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Load the full sprite sheet (all tiles)
        tileSheet = Content.Load<Texture2D>("spritesheet");

        // Assuming grass is at position (0, 8) and dirt is at position (8, 8) on your sprite sheet
        grassTileSource = new Rectangle(0, 8, tileSize, tileSize); // The grass tile (8x8)
        dirtTileSource = new Rectangle(8, 8, tileSize, tileSize); // The dirt tile (8x8)
        playerTileSource = new Rectangle(0, 0, tileSize, tileSize); // The player sprite (8x8)
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

        // Jumping mechanic
        if ((keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Space)) && !isJumping)
        {
            playerVelocity.Y = jumpForce; // Apply upward force
            isJumping = true; // Player is now jumping
        }
        if (playerVelocity.Y > 0)
        {
            gravity *= gravityScale;
        }         

        // Gravity (accelerates downward)
        playerVelocity.Y += gravity;
        float maxGravity = 3.0f;
        if (gravity > maxGravity) gravity = maxGravity;

        // Update position based on velocity
        playerPosition += playerVelocity;

        // Platform collision (check if player is above and lands on a platform)
        if (playerVelocity.Y > 0) // If falling down
        {
            if (playerPosition.Y + tileSize <= platform1.Y + tileSize &&
                playerPosition.Y + tileSize + playerVelocity.Y >= platform1.Y &&
                playerPosition.X + tileSize > platform1.X && playerPosition.X < platform1.X + platform1.Width)
            {
                // Player lands on platform1
                playerPosition.Y = platform1.Y - tileSize; // Set player on top of the platform
                playerVelocity.Y = 0; // Stop falling
                isJumping = false; // Allow jumping again after landing
                gravity = 0.1f;
            }

            if (playerPosition.Y + tileSize <= platform2.Y + tileSize &&
                playerPosition.Y + tileSize + playerVelocity.Y >= platform2.Y &&
                playerPosition.X + tileSize > platform2.X && playerPosition.X < platform2.X + platform2.Width)
            {
                // Player lands on platform2
                playerPosition.Y = platform2.Y - tileSize; // Set player on top of the platform
                playerVelocity.Y = 0; // Stop falling
                isJumping = false; // Allow jumping again after landing
                gravity = 0.1f;
            }

            if (playerPosition.Y + tileSize <= platform3.Y + tileSize &&
                playerPosition.Y + tileSize + playerVelocity.Y >= platform3.Y &&
                playerPosition.X + tileSize > platform3.X && playerPosition.X < platform3.X + platform3.Width)
            {
                playerPosition.Y = platform3.Y - tileSize; // Set player on top of the platform
                playerVelocity.Y = 0; // Stop falling
                isJumping = false; // Allow jumping again after landing
                gravity = 0.1f;
            }
        }

        // Add floor collision (reset when hitting the ground)
        if (playerPosition.Y > GraphicsDevice.Viewport.Height - tileSize)
        {
            playerPosition.Y = GraphicsDevice.Viewport.Height - tileSize;
            playerVelocity.Y = 0;
            gravity = 0.1f; // Reset gravity when landing
            isJumping = false; // Allow jumping again after landing
        }

        // Clamp to window horizontally
        playerPosition.X = MathHelper.Clamp(playerPosition.X, 0, GraphicsDevice.Viewport.Width - tileSize);
        Console.WriteLine($"Position: {playerPosition}, Velocity: {playerVelocity.Y}, Gravity: {gravity}");
// Kamera bude sledovat hráče – centrováno na hráče (případně mírně offset pro estetiku)
        var viewport = GraphicsDevice.Viewport;
        cameraTransform = Matrix.CreateTranslation(
            -playerPosition.X + viewport.Width / 2f - tileSize / 2f,
            -playerPosition.Y + viewport.Height / 2f - tileSize / 2f,
            0
        );

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        spriteBatch.Begin(transformMatrix: cameraTransform);


        // Draw the platforms as grass and dirt tiles
        DrawPlatform(platform1);
        DrawPlatform(platform2);
        DrawPlatform(platform3);

        // Draw the player (on top of the grass and platforms)
        spriteBatch.Draw(
            tileSheet, // Texture (sprite sheet)
            playerPosition, // Position of the player
            playerTileSource, // Source rectangle for the player sprite
            Color.White // White color (no tint)
        );

        spriteBatch.End();

        base.Draw(gameTime);
    }

    private void DrawPlatform(Rectangle platform)
    {
        // Draw the left and right green grass tiles (edges)
        spriteBatch.Draw(
            tileSheet, // Texture (sprite sheet)
            new Vector2(platform.X, platform.Y), // Position of the left tile
            grassTileSource, // Source rectangle for grass tile
            Color.White // White color (no tint)
        );

        // Draw the middle dirt tiles
        for (int x = platform.X + tileSize; x < platform.X + platform.Width - tileSize; x += tileSize)
        {
            spriteBatch.Draw(
                tileSheet, // Texture (sprite sheet)
                new Vector2(x, platform.Y), // Position of the dirt tile
                dirtTileSource, // Source rectangle for dirt tile
                Color.White // White color (no tint)
            );
        }

        // Draw the rightmost green grass tile with a flip to make sure it ends with grass on the right side
        Rectangle flippedGrassTileSource =
            new Rectangle(grassTileSource.X + tileSize, grassTileSource.Y, -tileSize, tileSize); // Flip horizontally
        spriteBatch.Draw(
            tileSheet, // Texture (sprite sheet)
            new Vector2(platform.X + platform.Width - tileSize, platform.Y), // Position of the right tile
            flippedGrassTileSource, // Source rectangle for flipped grass tile
            Color.White // White color (no tint)
        );
    }
}
