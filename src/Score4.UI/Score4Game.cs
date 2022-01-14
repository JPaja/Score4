using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;
using Score4.AI;
using Score4.Core;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Score4.UI;

public class Score4Game : Game
{
    private GraphicsDeviceManager _graphics;
    private BasicEffect _basicEffect_White;
    private BasicEffect _basicEffect_Black;
    private BasicEffect _basicEffect_Red;
    private VertexPositionNormalTexture[] _cubeVertices;

    private Cube[,,] cubes = new Cube[4, 4, 4];

    private Cube[,] pickers = new Cube[4, 4];
    private (bool, bool)[,,] matrix = new (bool, bool)[4,4,4];

    public Score4Game()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.IsFullScreen = false;
        _graphics.SynchronizeWithVerticalRetrace = true;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        Window.AllowUserResizing = true;
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    private Texture2D texture;

    protected override void LoadContent()
    {
        oldState = Keyboard.GetState();
        new SpriteBatch(GraphicsDevice);
        _basicEffect_White = new BasicEffect(GraphicsDevice)
        {
            AmbientLightColor = Vector3.One,
            LightingEnabled = true,
            DiffuseColor = Vector3.One,
            TextureEnabled = true,
            Texture = CreateWhiteTexture(GraphicsDevice),
        };

        _basicEffect_Black = new BasicEffect(GraphicsDevice)
        {
            AmbientLightColor = Vector3.One,
            LightingEnabled = true,
            DiffuseColor = Vector3.One,
            TextureEnabled = true,
            Texture = CreateBlackTexture(GraphicsDevice),
        };

        _basicEffect_Red = new BasicEffect(GraphicsDevice)
        {
            AmbientLightColor = Vector3.One,
            LightingEnabled = true,
            DiffuseColor = Vector3.One,
            TextureEnabled = true,
            Texture = CreateRedTexture(GraphicsDevice),
        };

        _cubeVertices = CubeFactory.Create();
        for (int x = 0; x < 4; x++)
        for (int z = 0; z < 4; z++)
        {
            for (int y = 0; y < 4; y++)
            {
                var cube = new Cube(new Vector3(1, 1, 1), new Vector3(5 - x * 5f, 3 - y * 5f, 7 - z * 5f));
                cubes[x, y, z] = cube;
            }

            pickers[x, z] = new Cube(new Vector3(1, 1, 1), new Vector3(5 - x * 5f, 3 + 5f, 7 - z * 5f));
        }

    }

    private int pickerX = 0;
    private int pickerZ = 0;
    private KeyboardState oldState;

    private Table tabla = new Table();
    private uint poeniBeli = 0;
    private uint poeniCrni = 0;

    private Random random = new Random();

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        if (Keyboard.GetState().IsKeyDown(Keys.Left))
            _sideRotation += 1f;
        else if (Keyboard.GetState().IsKeyDown(Keys.Right))
            _sideRotation -= 1f;
        if (Keyboard.GetState().IsKeyDown(Keys.Up))
            _upRotation += 1f;
        else if (Keyboard.GetState().IsKeyDown(Keys.Down))
            _upRotation -= 1f;

        if (Keyboard.GetState().IsKeyDown(Keys.W) && !oldState.IsKeyDown(Keys.W))
            pickerZ++;
        else if (Keyboard.GetState().IsKeyDown(Keys.S) && !oldState.IsKeyDown(Keys.S))
            pickerZ--;
        if (Keyboard.GetState().IsKeyDown(Keys.A) && !oldState.IsKeyDown(Keys.A))
            pickerX++;
        else if (Keyboard.GetState().IsKeyDown(Keys.D) && !oldState.IsKeyDown(Keys.D))
            pickerX--;

        if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !oldState.IsKeyDown(Keys.Enter))
        {
            if (tabla.CanPlay(pickerX, pickerZ))
            {
                tabla = tabla.Play(pickerX, pickerZ, false);
                tabla = Score4AI.Predict(tabla, true);
                (poeniBeli, poeniCrni) = tabla.CountPoints();
                matrix = tabla.GetMatrix();
            }
        }

        if (Keyboard.GetState().IsKeyDown(Keys.R) && !oldState.IsKeyDown(Keys.R))
        {
            tabla = new Table();
            poeniBeli = 0;
            poeniCrni = 0;
            matrix = new (bool, bool)[4, 4, 4];
        }

        if (pickerZ < 0)
            pickerZ = 0;
        if (pickerZ >= 4)
            pickerZ = 3;
        if (pickerX < 0)
            pickerX = 0;
        if (pickerX >= 4)
            pickerX = 3;
        oldState = Keyboard.GetState();
        base.Update(gameTime);
    }




    private float _sideRotation = 0;
    private float _upRotation = 40f;

    private readonly RasterizerState _rasterizerState = new RasterizerState
    {
        CullMode = CullMode.CullClockwiseFace
    };


    public static Texture2D CreateWhiteTexture(GraphicsDevice device)
    {
        Color[] data = new Color[1];
        data[0] = new Color(255, 255, 255, 255);
        return TextureFromColorArray(device, data, 1, 1);
    }

    public static Texture2D CreateBlackTexture(GraphicsDevice device)
    {
        Color[] data = new Color[1];
        data[0] = new Color(0, 0, 0, 255);
        return TextureFromColorArray(device, data, 1, 1);
    }

    public static Texture2D CreateRedTexture(GraphicsDevice device)
    {
        Color[] data = new Color[1];
        data[0] = new Color(255, 0, 0, 255);
        return TextureFromColorArray(device, data, 1, 1);
    }

    public static Texture2D TextureFromColorArray(GraphicsDevice device, Color[] data, int width, int height)
    {
        Texture2D tex = new Texture2D(device, width, height);
        tex.SetData<Color>(data);
        return tex;
    }

    protected override void Draw(GameTime gameTime)
    {
        Window.Title = $"Beli poeni: {poeniBeli} | Crni poeni: {poeniCrni}";

        GraphicsDevice.RasterizerState = _rasterizerState;
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _basicEffect_White.World = Matrix.CreateRotationY(MathHelper.ToRadians(_sideRotation))
                                   * Matrix.CreateRotationX(MathHelper.ToRadians(_upRotation));



        _basicEffect_White.VertexColorEnabled = true;
        _basicEffect_White.View = Matrix.CreateTranslation(0f, 0f, -60f);

        _basicEffect_White.Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.PiOver4,
            GraphicsDevice.Viewport.AspectRatio,
            1f,
            100f);

        //
        _basicEffect_Black.World = Matrix.CreateRotationY(MathHelper.ToRadians(_sideRotation))
                                   * Matrix.CreateRotationX(MathHelper.ToRadians(_upRotation));



        _basicEffect_Black.VertexColorEnabled = true;
        _basicEffect_Black.View = Matrix.CreateTranslation(0f, 0f, -60f);

        _basicEffect_Black.Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.PiOver4,
            GraphicsDevice.Viewport.AspectRatio,
            1f,
            100f);
        //
        _basicEffect_Red.World = Matrix.CreateRotationY(MathHelper.ToRadians(_sideRotation))
                                 * Matrix.CreateRotationX(MathHelper.ToRadians(_upRotation));



        _basicEffect_Red.VertexColorEnabled = true;
        _basicEffect_Red.View = Matrix.CreateTranslation(0f, 0f, -60f);

        _basicEffect_Red.Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.PiOver4,
            GraphicsDevice.Viewport.AspectRatio,
            1f,
            100f);

        var beli = new List<(int i, int j, int k)>();
        var crni = new List<(int i, int j, int k)>();
        for (int x = 0; x < 4; x++)
        for (int y = 0; y < 4; y++)
        {
            for (int z = 0; z < 4; z++)
            {
                var (filled, crniIgrac) = matrix[x, y, z];
                if (!filled)
                    continue;
                if (crniIgrac)
                    crni.Add((x, 3 - z, y));
                else
                    beli.Add((x, 3 - z, y));
            }
        }

        foreach (var pass in _basicEffect_White.CurrentTechnique.Passes)
        {
            pass.Apply();
            foreach (var (i, j, k) in beli)
                cubes[i, j, k].RenderShape(GraphicsDevice);
        }

        foreach (var pass in _basicEffect_Black.CurrentTechnique.Passes)
        {
            pass.Apply();
            foreach (var (i, j, k) in crni)
                cubes[i, j, k].RenderShape(GraphicsDevice);
        }

        foreach (var pass in _basicEffect_Red.CurrentTechnique.Passes)
        {
            pass.Apply();
            pickers[pickerX, pickerZ].RenderShape(GraphicsDevice);

        }

        base.Draw(gameTime);
    }
}
