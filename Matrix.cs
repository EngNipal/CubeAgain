using System;
using System.Threading.Tasks;

namespace CubeAgain
{
    class Matrix
    {
        public static double[][] MatrixCreate(int rows, int cols)
        {
            // Создаем матрицу, полностью инициализированную
            // значениями 0.0. Проверка входных параметров опущена.
            double[][] result = new double[rows][];
            for (int i = 0; i < rows; ++i)
                result[i] = new double[cols]; // авто инициализация в 0.0
            return result;
        }
        public static double[][] Multiply(double[][] matrixA, double[][] matrixB)
        {
            // Проверка ошибок, вычисление aRows, aCols, bCols
            int aRows = matrixA.Length; int aCols = matrixA[0].Length;
            int bRows = matrixB.Length; int bCols = matrixB[0].Length;
            if (aCols != bRows)
                throw new Exception("Не соответственные размеры матриц в методе Matrix.Multiply");
            double[][] result = MatrixCreate(aRows, bCols);
            Parallel.For(0, aRows, i =>
            {
                for (int j = 0; j < bCols; ++j)
                    for (int k = 0; k < aCols; ++k)
                        result[i][j] += matrixA[i][k] * matrixB[k][j];
            }
            );
            return result;
        }
        public static double[][] MatrixIdentity(int n)
        {
            double[][] result = MatrixCreate(n, n);
            for (int i = 0; i < n; ++i)
                result[i][i] = 1.0;
            return result;
        }
    }
}
