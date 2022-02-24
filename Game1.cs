using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Threading;

namespace PerlinNoise
{
    public enum Trait {Octaves, Scale, Lacunarity, Persistance, Function, OffsetWeight, OffsetScale, Change}
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D block;
        PerlinNoise noise;
        PerlinNoise offsetNoise;

        int pixelSize = 1;

        int octaves = 5;
        float scale = 1000;
        float lacunarity = 2;
        float persistence = 0.5f;
        float noisePower = 3;
        float offsetNoiseWeight = 100;
        float offsetNoiseScale = 100;
        float xOffset = 100;
        float yOffset = 100;
        int changeScale = 1;

        float[,] colors;

        KeyboardState prevState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            block = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            block.SetData<Color>(0, null, new Color[] {Color.White}, 0, 1);
            noise = new PerlinNoise();
            offsetNoise = new PerlinNoise();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _graphics.PreferredBackBufferHeight = 1000;
            _graphics.PreferredBackBufferWidth = 1000;
            _graphics.ApplyChanges();
            colors = new float[_graphics.PreferredBackBufferWidth/pixelSize, _graphics.PreferredBackBufferHeight/pixelSize];
            GenerateColors();
            // TODO: use this.Content to load your game content here
        }

        Trait itemToEdit = Trait.Octaves;
        int change;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            

            if(SinglePress(Keys.O))
            {
                itemToEdit = Trait.Octaves;
            }
            if(SinglePress(Keys.S))
            {
                itemToEdit = Trait.Scale;
            }
            if(SinglePress(Keys.L))
            {
                itemToEdit = Trait.Lacunarity;
            }
            if(SinglePress(Keys.P))
            {
                itemToEdit = Trait.Persistance;
            }
            if(SinglePress(Keys.F))
            {
                itemToEdit = Trait.Function;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.LeftShift) && SinglePress(Keys.W))
            {
                itemToEdit = Trait.OffsetWeight;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.LeftShift) && SinglePress(Keys.S))
            {
                itemToEdit = Trait.OffsetScale;
            }
            if(SinglePress(Keys.C))
            {
                itemToEdit = Trait.Change;
            }

            if(SinglePress(Keys.Up))
            {
                change = changeScale;
            }
            else if(SinglePress(Keys.Down))
            {
                change = -changeScale;
            }
            else
            {
                change = 0;
            }

            if(change != 0)
            {
                switch(itemToEdit)
                {
                    case Trait.Octaves:
                        octaves  = octaves + change > 0 ? octaves + change : 1;
                        Console.WriteLine($"Octaves: {octaves}");
                        break;
                    case Trait.Scale:
                        scale = scale + (change * 100) > 100 ? scale + (change * 100) : 100;
                        Console.WriteLine($"Scale: {scale}");
                        break;
                    case Trait.Lacunarity:
                        lacunarity = lacunarity + change > 1 ? lacunarity + change : 1.1f;
                        Console.WriteLine($"Lacunarity: {lacunarity}");
                        break;
                    case Trait.Persistance:
                        persistence = persistence += change/10f > 0 ? persistence + change/10f : .1f;
                        Console.WriteLine($"Persistance: {persistence}");
                        break;
                    case Trait.Function:
                        noisePower = noisePower + change/4f > 0 ? noisePower + change/4f : .25f;
                        Console.WriteLine($"Noise Power: {noisePower}");
                        break;
                    case Trait.OffsetScale:
                        offsetNoiseScale = offsetNoiseScale + (change * 10) > 10 ? offsetNoiseScale + (change * 10) : 10;
                        Console.WriteLine($"Offset Scale: {offsetNoiseScale}");
                        break;
                    case Trait.OffsetWeight:
                        offsetNoiseWeight += (change * 10);
                        Console.WriteLine($"Offset Weight: {offsetNoiseWeight}");
                        break;
                    case Trait.Change:
                        changeScale = changeScale + change/changeScale > 0 ? changeScale + change/changeScale : 1;
                        Console.WriteLine($"Change: {changeScale}");
                        break;
                }
                if(itemToEdit != Trait.Change)
                {
                    GenerateColors();
                }
            }
            prevState = Keyboard.GetState();
            base.Update(gameTime);
        }

        public bool SinglePress(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key) && !prevState.IsKeyDown(key);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            for(int x = 0; x < colors.GetLength(0); x++)
            {
                for(int y = 0; y < colors.GetLength(1); y++)
                {
                    float noiseValue = MathF.Pow(colors[x, y], noisePower);

                    _spriteBatch.Draw(block, new Rectangle(x * pixelSize, y * pixelSize, pixelSize, pixelSize), new Color(0, noiseValue/7.5f, noiseValue/1.5f));
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void GenerateColors()
        {
            float max = float.MinValue;
            float min = float.MaxValue;
            for(int x = 0; x < colors.GetLength(0); x++)
            {
                for(int y = 0; y < colors.GetLength(1); y++)
                {
                    
                    float amplitude = .5f;
                    float frequency = 2;
                    float noiseValue = 0;
                    for(int i = 0; i < octaves; i++)
                    {
                        noiseValue += noise[(x + xOffset + (offsetNoiseWeight * offsetNoise[x/offsetNoiseScale, y/offsetNoiseScale]))/scale * frequency, (y + yOffset + (offsetNoiseWeight * offsetNoise[x/offsetNoiseScale, y/offsetNoiseScale]))/scale * frequency] * amplitude;
                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }

                    colors[x, y] = noiseValue;

                    if(colors[x,y] > max)
                    {
                        max = noiseValue;
                    }
                    if(colors[x,y] < min)
                    {
                        min = noiseValue;
                    }
                }
            }

            for(int x = 0; x < colors.GetLength(0); x++)
            {
                for(int y = 0; y < colors.GetLength(1); y++)
                {
                    colors[x, y] = (colors[x, y] - min)/(max-min);
                }
            }
        }
    }
}
