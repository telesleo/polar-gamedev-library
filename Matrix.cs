﻿namespace Polar
{
    public struct Matrix<T>
    {
        public int XCount;
        public int YCount;
        public T[] Values { get; private set; }

        public T this[int x, int y]
        {
            get { return Values[y * XCount + x]; }
            set { Values[y * XCount + x] = value; }
        }

        public Matrix(int xCount, int yCount)
        {
            XCount = xCount;
            YCount = yCount;
            Values = new T[XCount * YCount];
        }

        public bool Valid(int x, int y)
        {
            if (x < 0 || x >= XCount || y < 0 || y >= YCount)
            {
                return false;
            }
            return true;
        }
    }
}
