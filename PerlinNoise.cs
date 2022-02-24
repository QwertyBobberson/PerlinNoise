using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PerlinNoise
{
    public class PerlinNoise
    {
        public float this[float x, float z]
        {
            get 
            {
                float[,] dotProducts = new float[2,2];
                Vector2 distance = new Vector2(x % 1, z % 1);

                Vector2 offset = new Vector2();
                Vector2 gradient = new Vector2();

                for(int _x = 0; _x < 2; _x++)
                {
                    for(int _z = 0; _z < 2; _z++)
                    {
                        if(_x == 0)
                        {
                            offset.X = -distance.X;
                        }
                        else
                        {
                            offset.X = 1 - distance.X;
                        }

                        if(_z == 0)
                        {
                            offset.Y = -distance.Y;
                        }
                        else
                        {
                            offset.Y = 1 - distance.Y;
                        }

                        gradient = GenerateVector((int)(x + offset.X), (int)(z + offset.Y));//grid[(int)(x + offset.X), (int)(z + offset.Y)];
                        dotProducts[_x, _z] = (offset.X * gradient.X) + (offset.Y * gradient.Y);
                    }
                }
                float percentFromX = distance.X;
                float percentFromZ = distance.Y;

                float xz1 = SmoothStep(percentFromX, dotProducts[0, 0], dotProducts[1, 0]);//(1-percentFromX)*dotProducts[0,0] + (percentFromX)*dotProducts[1,0];
                float xz2 = SmoothStep(percentFromX, dotProducts[0, 1], dotProducts[1, 1]);//(1-percentFromX)*dotProducts[0,1] + (percentFromX)*dotProducts[1,1]; 

                float xz = SmoothStep(percentFromZ, xz1, xz2);//(1-percentFromZ)*xz1 + (percentFromZ)*xz2;

                return xz;
            }
        }

        private Vector2 GenerateVector(int x, int y)
        {
            x = x > 0 ? x : -x;
            y = y > 0 ? y : -y;
            uint a = (uint)x, b = (uint)y;
            a *= 3284157443; b ^= a << 16 | a >> 16;
            b *= 1911520717; a ^= b << 16 | b >> 16;
            a *= 2048419325;
            float random = a * (3.14159265f / ~(~0u >> 1)); // in [0, 2*Pi]
            return new Vector2(MathF.Sin(a), MathF.Cos(a));
        }

        private float SmoothStep(float x, float min, float max)
        {
            return ((6 * MathF.Pow(x, 5) - 15 * MathF.Pow(x, 4) + 10 * MathF.Pow(x, 3)) * (max-min)) + min;
        }
    }
}