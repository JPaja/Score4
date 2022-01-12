using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Score4AI;

namespace MonoGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private BasicEffect _basicEffect_White;
        private BasicEffect _basicEffect_Black;
        private BasicEffect _basicEffect_Red;
        private VertexPositionNormalTexture[] _cubeVertices;

        private Cube[,,] cubes = new Cube[4,4,4];

        private Cube[,] pickers = new Cube[4, 4];
        
        public Game1()
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
            _spriteBatch = new SpriteBatch(GraphicsDevice);
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
            for (int i = 0; i < 4; i++)
            for (int k = 0; k < 4; k++)
            {
                for (int j = 0; j < 4; j++)
                {
                    var cube = new Cube(new Vector3(1, 1, 1), new Vector3(5 - i * 5f, 3 - j * 5f, 7 - k * 5f));
                    cubes[i, j, k] = cube;
                }
                pickers[i,k] = new Cube(new Vector3(1, 1, 1), new Vector3(5 - i * 5f, 3 + 5f, 7 - k * 5f));
            }

        }

        private int pickerX = 0;
        private int pickerZ = 0;
        private KeyboardState oldState;

        private Tabla tabla = new Tabla();
        private int poeniBeli = 0;
        private int poeniCrni = 0;
        
        
        public static void Minimax(StabloTabla stablo, int generacija, bool maximize, sbyte alpha = sbyte.MinValue, sbyte beta = sbyte.MaxValue)
        {
            if (generacija == 0 || stablo.Tabla.Podaci == unchecked((ulong)-1))
            {
                var (poeni1, poeni2) = stablo.Tabla.IzracunajPoene();
                stablo.Vrednost = (sbyte)(poeni1 - poeni2);
                return;
            }

            if (maximize)
            {
                var max = sbyte.MinValue;
                foreach (var potez in stablo.Potezi)
                {
                    if (potez == null)
                        continue;
                    Minimax(potez, generacija - 1, false, alpha, beta);
                    var vrednost = potez.Vrednost.Value;
                    max = Math.Max(max, vrednost);
                    alpha = Math.Max(alpha, vrednost);
                    if (beta <= alpha)
                        break;
                }
                stablo.Vrednost = max;
            }
            else
            {
                var min = sbyte.MaxValue;
                foreach (var potez in stablo.Potezi)
                {
                    if (potez == null)
                        continue;
                    Minimax(potez, generacija - 1, true, alpha, beta);
                    var vrednost = potez.Vrednost.Value;
                    min = Math.Min(min, vrednost);
                    beta = Math.Min(beta, vrednost);
                    if (beta <= alpha)
                        break;
                }
                stablo.Vrednost = min;
            }

        }

        private const int Generacija = 5;
        public static StabloTabla IzracunajTable(Tabla tabla, int generacija = 0)
        {
            if (generacija == Generacija)
                return new StabloTabla(tabla, false);
            var istorija = new StabloTabla(tabla);
            for (var i = 0; i < 16; i++)
            {
                if (tabla.PopunjenKvadrant(i))
                    continue;
                istorija.Potezi[i] = IzracunajTable(tabla.DodajPotez(i, generacija % 2 == 0), generacija + 1);
            }
            return istorija;
        }

        private Random random = new Random();
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            if(Keyboard.GetState().IsKeyDown(Keys.Left))
                _sideRotation += 1f;
            else if(Keyboard.GetState().IsKeyDown(Keys.Right))
                _sideRotation -= 1f;
            if(Keyboard.GetState().IsKeyDown(Keys.Up))
                _upRotation += 1f;
            else if(Keyboard.GetState().IsKeyDown(Keys.Down))
                _upRotation -= 1f;
            
            if(Keyboard.GetState().IsKeyDown(Keys.W) && !oldState.IsKeyDown(Keys.W))
                pickerZ ++;
            else if(Keyboard.GetState().IsKeyDown(Keys.S) && !oldState.IsKeyDown(Keys.S))
                pickerZ --;
            if(Keyboard.GetState().IsKeyDown(Keys.A) && !oldState.IsKeyDown(Keys.A))
                pickerX ++;
            else if(Keyboard.GetState().IsKeyDown(Keys.D) && !oldState.IsKeyDown(Keys.D))
                pickerX --;

            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !oldState.IsKeyDown(Keys.Enter))
            {
                var kvadrant = pickerX*4 + pickerZ;
                var velicina = tabla.LopticaUKvadrantu(kvadrant);
                if (velicina != 4)
                {
                    tabla = tabla.DodajPotez(kvadrant, false);
                    
                    var stablo = IzracunajTable(tabla);
                    Minimax(stablo, Generacija, false);
                    
                    var aiPotezi = stablo.Potezi.Where(p => p != null && p.Vrednost.HasValue  && p.Vrednost.Value == stablo.Vrednost.Value).ToList();
                    var aiPotez = aiPotezi.ElementAt(random.Next(aiPotezi.Count()));
                    var aiKvadrant = Array.IndexOf(stablo.Potezi, aiPotez);
                    tabla = tabla.DodajPotez(aiKvadrant, true);
                    (poeniBeli, poeniCrni) = tabla.IzracunajPoene();
                }
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.R) && !oldState.IsKeyDown(Keys.R))
            {
                tabla = new Tabla();
                poeniBeli = 0;
                poeniCrni = 0;
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
            
            //_basicEffect.EnableDefaultLighting();



            var beli = new List<(int i, int j, int k)>();
            var crni = new List<(int i, int j, int k)>();
            for (int i = 0; i < 4; i++)
                for (int k = 0; k < 4; k++)
                {
                    var kvadrant = i * 4 + k;
                    var index = kvadrant * 4;
                    var kvadrantData = (byte)((tabla.Podaci >> index) & 0b1111);
                    var igracData = (byte)((tabla.Igrac >> index) & 0b1111);
                    for (int j = 0; j < 4; j++)
                    {
                        var filled = (kvadrantData & (1 << j)) != 0;
                        var crniIgrac = (igracData & (1 << j)) != 0;
                        if(!filled)
                            continue;
                        if(crniIgrac)
                            crni.Add((i,3-j,k));
                        else
                            beli.Add((i,3-j,k));
                    }
                }
                
            
            
            foreach (var pass in _basicEffect_White.CurrentTechnique.Passes)
            {
                pass.Apply();
                //GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _cubeVertices, 0, 12);
                foreach (var (i,j,k) in beli)
                    cubes[i,j,k].RenderShape(GraphicsDevice);
            }
            
            foreach (var pass in _basicEffect_Black.CurrentTechnique.Passes)
            {
                pass.Apply();
                //GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _cubeVertices, 0, 12);
                foreach (var (i,j,k) in crni)
                    cubes[i,j,k].RenderShape(GraphicsDevice);
            }
            
            foreach (var pass in _basicEffect_Red.CurrentTechnique.Passes)
            {
                pass.Apply();
                //GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _cubeVertices, 0, 12);
              
                    pickers[pickerX,pickerZ].RenderShape(GraphicsDevice);
               
            }

            base.Draw(gameTime);
        }
    }
}
