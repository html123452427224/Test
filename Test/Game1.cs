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
    private float gravity = 0.1f;
    private float gravityScale = 1.05f;
    private float moveSpeed = 3f;

    private bool isJumping = false;
    private float jumpForce = -5f;
    private Matrix cameraTransform;

    private Texture2D tileSheet;
    private int tileSize = 8;
    private Rectangle grassTileSource;
    private Rectangle dirtTileSource;
    private Rectangle playerTileSource;

    private Rectangle platform1;
    private Rectangle platform2;
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

        platform1 = new Rectangle(50, 100, 80, 8);
        platform2 = new Rectangle(200, 150, 80, 8);
        platform3 = new Rectangle(300, 200, 80, 8);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        tileSheet = Content.Load<Texture2D>("spritesheet");
        grassTileSource = new Rectangle(0, 8, tileSize, tileSize);
        dirtTileSource = new Rectangle(8, 8, tileSize, tileSize);
        playerTileSource = new Rectangle(0, 0, tileSize, tileSize);
    }

    protected override void Update(GameTime gameTime)
{
    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
        Keyboard.GetState().IsKeyDown(Keys.Escape))
        Exit();

    var keyboard = Keyboard.GetState();

    if (keyboard.IsKeyDown(Keys.A))
    {
        playerPosition.X -= moveSpeed;
    }

    if (keyboard.IsKeyDown(Keys.D))
    {
        playerPosition.X += moveSpeed;
    }

    if ((keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Space)) && !isJumping)
    {
        playerVelocity.Y = jumpForce;
        isJumping = true;
    }

    if (playerVelocity.Y > 0)
    {
        gravity *= gravityScale;
    }

    playerVelocity.Y += gravity;
    float maxGravity = 3.0f;
    if (gravity > maxGravity) gravity = maxGravity;

    playerPosition += playerVelocity;

    // Top-side collision (landing on platforms)
    if (playerVelocity.Y > 0)
    {
        if (playerPosition.Y + tileSize <= platform1.Y + tileSize &&
            playerPosition.Y + tileSize + playerVelocity.Y >= platform1.Y &&
            playerPosition.X + tileSize > platform1.X && playerPosition.X < platform1.X + platform1.Width)
        {
            playerPosition.Y = platform1.Y - tileSize;
            playerVelocity.Y = 0;
            isJumping = false;
            gravity = 0.1f;
        }

        if (playerPosition.Y + tileSize <= platform2.Y + tileSize &&
            playerPosition.Y + tileSize + playerVelocity.Y >= platform2.Y &&
            playerPosition.X + tileSize > platform2.X && playerPosition.X < platform2.X + platform2.Width)
        {
            playerPosition.Y = platform2.Y - tileSize;
            playerVelocity.Y = 0;
            isJumping = false;
            gravity = 0.1f;
        }

        if (playerPosition.Y + tileSize <= platform3.Y + tileSize &&
            playerPosition.Y + tileSize + playerVelocity.Y >= platform3.Y &&
            playerPosition.X + tileSize > platform3.X && playerPosition.X < platform3.X + platform3.Width)
        {
            playerPosition.Y = platform3.Y - tileSize;
            playerVelocity.Y = 0;
            isJumping = false;
            gravity = 0.1f;
        }
    }

    // Bottom-side collision (hitting underside of platforms)
    if (playerVelocity.Y < 0)
    {
        if (playerPosition.Y >= platform1.Y + platform1.Height &&
            playerPosition.Y + playerVelocity.Y <= platform1.Y + platform1.Height &&
            playerPosition.X + tileSize > platform1.X && playerPosition.X < platform1.X + platform1.Width)
        {
            playerPosition.Y = platform1.Y + platform1.Height;
            playerVelocity.Y = 0;
        }

        if (playerPosition.Y >= platform2.Y + platform2.Height &&
            playerPosition.Y + playerVelocity.Y <= platform2.Y + platform2.Height &&
            playerPosition.X + tileSize > platform2.X && playerPosition.X < platform2.X + platform2.Width)
        {
            playerPosition.Y = platform2.Y + platform2.Height;
            playerVelocity.Y = 0;
        }

        if (playerPosition.Y >= platform3.Y + platform3.Height &&
            playerPosition.Y + playerVelocity.Y <= platform3.Y + platform3.Height &&
            playerPosition.X + tileSize > platform3.X && playerPosition.X < platform3.X + platform3.Width)
        {
            playerPosition.Y = platform3.Y + platform3.Height;
            playerVelocity.Y = 0;
        }
    }

    if (playerPosition.Y > GraphicsDevice.Viewport.Height - tileSize)
    {
        playerPosition.Y = GraphicsDevice.Viewport.Height - tileSize;
        playerVelocity.Y = 0;
        gravity = 0.1f;
        isJumping = false;
    }

    playerPosition.X = MathHelper.Clamp(playerPosition.X, 0, GraphicsDevice.Viewport.Width - tileSize);

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

        DrawPlatform(platform1);
        DrawPlatform(platform2);
        DrawPlatform(platform3);

        spriteBatch.Draw(
            tileSheet,
            playerPosition,
            playerTileSource,
            Color.White
        );

        spriteBatch.End();

        base.Draw(gameTime);
    }

    private void DrawPlatform(Rectangle platform)
    {
        spriteBatch.Draw(
            tileSheet,
            new Vector2(platform.X, platform.Y),
            grassTileSource,
            Color.White
        );

        for (int x = platform.X + tileSize; x < platform.X + platform.Width - tileSize; x += tileSize)
        {
            spriteBatch.Draw(
                tileSheet,
                new Vector2(x, platform.Y),
                dirtTileSource,
                Color.White
            );
        }

        Rectangle flippedGrassTileSource =
            new Rectangle(grassTileSource.X + tileSize, grassTileSource.Y, -tileSize, tileSize);
        spriteBatch.Draw(
            tileSheet,
            new Vector2(platform.X + platform.Width - tileSize, platform.Y),
            flippedGrassTileSource,
            Color.White
        );
    }
}
