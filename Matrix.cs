namespace Polar
{
    public struct Matrix<T>
    {
        public int XCount;
        public int YCount;
        private T[][] Values;

        public T this[int x, int y]
        {
            get { return Values[x][y]; }
            set { Values[x][y] = value; }
        }

        public Matrix(int xCount, int yCount)
        {
            XCount = xCount;
            YCount = yCount;
            Values = new T[XCount][];
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i] = new T[YCount];
            }
        }
    }
}
