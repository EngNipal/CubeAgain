namespace CubeAgain
{
    public enum Turns
    {
        R,
        Rp,
        R2,
        U,
        Up,
        U2,
        F,
        Fp,
        F2
    }
    // Удалённые методы.
    // Метод, организующий структуру нейросети.
    //public static void UserStruct()
    //{
    //    Console.Write("Введите количество слоёв нейросети, включая выходной слой: ");
    //    int numOfLayers = Convert.ToInt32(Console.ReadLine());
    //    Console.Write($"Введите количество входов для слоя 1 из {NumOfLayers}: ");
    //    int inputQuantity = Convert.ToInt32(Console.ReadLine());
    //    List<int> numNeurons = new List<int>();
    //    for (int i = 0; i < NumOfLayers; i++)
    //    {
    //        if (i != NumOfLayers - 1)
    //        {
    //            Console.Write($"Введите количество нейронов для слоя {i + 1} из {NumOfLayers}: ");
    //        }
    //        else
    //        {
    //            Console.WriteLine($"Введите количество нейронов для слоя {i + 1} из {NumOfLayers}");
    //            Console.Write($"Это будет количеством выходов у нейронной сети: ");
    //        }
    //        numNeurons[i] = Convert.ToInt32(Console.ReadLine());
    //    }
    //    PresetStruct(numOfLayers, inputQuantity, numNeurons);
    //}


    // Методы копирования значений из массива в массив.
    //public static void CopyValues(int[] from, int[] to)
    //{
    //    if (to?.Length < from?.Length)
    //    {
    //        throw new Exception("Размер целевого массива меньше размера источника. Копирование не произведено.");
    //    }
    //    else
    //    {
    //        for (int i = 0; i < from.Length; i++)
    //        {
    //            to[i] = from[i];
    //        }
    //    }
    //}
    //public static void CopyValues(double[] from, double[] to)
    //{
    //    if (to?.Length < from?.Length)
    //    {
    //        throw new Exception("Размер целевого массива меньше размера источника. Копирование не произведено.");
    //    }
    //    else
    //    {
    //        for (int i = 0; i < from.Length; i++)
    //        {
    //            to[i] = from[i];
    //        }
    //    }
    //}


    //double[] tempInputs = new double[Blocks[0].FCL.NumInputs];
    //double[] tempInOutputs0 = new double[Blocks[0].Outputs.Length];
    //double[] tempInOutputs1 = new double[Blocks[1].Outputs.Length];
    //double[] tempPolicy = new double[Policy.Length];
}
