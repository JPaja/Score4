using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Score4.UI.Primitives;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Score4.UI;

public class Score4Game : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpherePrimitive _spherePrimitive;
    
    private BasicEffect _effect;
    private Texture2D _blackTexture;
    private Texture2D _whiteTexture;

    private Vector3 _cameraPosition = new(0, 0, 0);

    private float _moveSpeed = 10f;
    
    private KeyboardState _keyboardState;
    private MouseState _mouseState;

    private TableModel _table;
    
    private readonly RasterizerState _rasterizerState = new()
    {
        CullMode = CullMode.CullClockwiseFace
    };
    
    
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
        var radius = 1f;
        _spherePrimitive = new SpherePrimitive(GraphicsDevice,  1f, 10);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _blackTexture = CreateTexture(GraphicsDevice, Color.Black);
        _whiteTexture = CreateTexture(GraphicsDevice, Color.White);
        _effect = new BasicEffect(GraphicsDevice)
        {
            AmbientLightColor = Vector3.One,
            LightingEnabled = true,
            DiffuseColor =  new Vector3(0.5f, 0, 0),
            TextureEnabled = true,
            Texture = _blackTexture,
        };
        _keyboardState = Keyboard.GetState();
        _mouseState = Mouse.GetState();
        _table = new TableModel();
        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        var keyboardState = Keyboard.GetState();
        var mouseState = Mouse.GetState();
        
        var scrollDelta = mouseState.ScrollWheelValue - _mouseState.ScrollWheelValue;
        if (scrollDelta != 0f)
            _cameraPosition.Z += _moveSpeed * (scrollDelta / 10f) * delta;
     
        /*var mouseXDelta = mouseState.X - _mouseState.X;
        var mouseYDelta = mouseState.Y - _mouseState.Y;

        if (mouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Pressed)
        {
            _cameraX += mouseXDelta * delta;
            _cameraY += mouseYDelta * delta;
        }*/

        if (keyboardState.IsKeyDown(Keys.Left))
            _cameraPosition.X += _moveSpeed * delta;
        else if (keyboardState.IsKeyDown(Keys.Right))
            _cameraPosition.X -= _moveSpeed * delta;
        if (keyboardState.IsKeyDown(Keys.Up))
            _cameraPosition.Y += _moveSpeed * delta;
        else if (keyboardState.IsKeyDown(Keys.Down))
            _cameraPosition.Y -= _moveSpeed * delta;

        _keyboardState = keyboardState;
        _mouseState = mouseState;
        base.Update(gameTime);
    }

    public static Texture2D CreateTexture(GraphicsDevice device, Color color)
    {
        var texture = new Texture2D(device, 1, 1);
        texture.SetData(new []{ color});
        return texture;
    }
    
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.RasterizerState = _rasterizerState;
        GraphicsDevice.Clear(Color.Aqua);
        
        var world = Matrix.CreateRotationY(MathHelper.ToRadians(_cameraPosition.X))
                    * Matrix.CreateRotationX(MathHelper.ToRadians(_cameraPosition.Y));

        Matrix.CreateLookAt(_cameraPosition, Vector3.One, _cameraPosition);
        
        var view = Matrix.CreateLookAt(new(1, 0, -10), Vector3.Zero ,_cameraPosition);
            //Matrix.CreateTranslation(_cameraPosition.X, _cameraPosition.Y, _cameraPosition.Z);
        var projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1f, 100f);
        
        _effect.EnableDefaultLighting();
        _effect.Texture = _whiteTexture;
        _effect.VertexColorEnabled = true;
        _effect.View = view;
        _effect.World = world;
        _effect.Projection = projection;

        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                for (int z = 0; z < 4; z++)
                {
                    _effect.World = Matrix.CreateTranslation(x * 1.5f - 3 , y * 1.5f - 3, z * 1.5f - 3);
                    _spherePrimitive.Draw(_effect);
                }
            }
        }
        //_effect.LightingEnabled = true; // turn on the lighting subsystem.
        /*_effect.DirectionalLight0.DiffuseColor = new Vector3(0.5f, 0, 0); // a red light
        _effect.DirectionalLight0.Direction = new Vector3(1, 0, 0);  // coming along the x-axis
        _effect.DirectionalLight0.SpecularColor = new Vector3(0, 1, 0); // with green highlights*/

    
        base.Draw(gameTime);
    }
}