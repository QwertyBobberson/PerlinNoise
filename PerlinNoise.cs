using System;

namespace PerlinNoise
{
    public abstract class PerlinNoise
    {
        struct Float2
        {
            public float x;
            public float y;

            public Float2(float _x, float _y)
            {
                x = _x;
                y = _y;
            }
        }
        public static float GetPoint(float x, float z)
        {
            float[,] dotProducts = new float[2,2];
            Float2 distance = new Float2(x % 1, z % 1);
            Float2 offset;
            Float2 gradient;

            for(int _x = 0; _x < 2; _x++)
            {
                for(int _z = 0; _z < 2; _z++)
                {
                    offset.x = _x == 0 ? -distance.x : 1 - distance.x;
                    offset.y = _z == 0 ? -distance.y : 1 - distance.y;
                    
                    gradient = GenerateVector((int)(x + offset.x), (int)(z + offset.y));
                    dotProducts[_x, _z] = (offset.x * gradient.x) + (offset.y * gradient.y);
                }
            }

            float xz1 = SmoothStep(distance.x, dotProducts[0, 0], dotProducts[1, 0]);
            float xz2 = SmoothStep(distance.x, dotProducts[0, 1], dotProducts[1, 1]);

            float xz = SmoothStep(distance.y, xz1, xz2);

            return xz;
        
        }

        private static Float2 GenerateVector(int x, int y)
        {
            x = x > 0 ? x : -x;
            y = y > 0 ? y : -y;
            uint a = (uint)x, b = (uint)y;
            a *= 3284157443; b ^= a << 16 | a >> 16;
            b *= 1911520717; a ^= b << 16 | b >> 16;
            a *= 2048419325;
            float random = a * (3.14159265f / ~(~0u >> 1));
            return new Float2(MathF.Sin(a), MathF.Cos(a));
        }

        private static float SmoothStep(float x, float min, float max)
        {
            return ((x * x * x * (x * (x * 6 - 15) + 10)) * (max-min)) + min;
        }
    }
}