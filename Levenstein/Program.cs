static int ComputeLevenshteinDistance(string? firstInput, string? secondInput)
{
    if ((firstInput == null) || (secondInput == null)) return -1;

    int firstLen = firstInput.Length;
    int secondLen = secondInput.Length;

    if ((firstLen == 0) && (secondLen == 0)) return 0;
    if (firstLen == 0) return secondLen;
    if (secondLen == 0) return firstLen;

    string first = firstInput.ToUpper();
    string second = secondInput.ToUpper();

    int[,] distMatrix = new int[firstLen + 1, secondLen + 1];

    for (int i = 0; i <= firstLen; i++)
        distMatrix[i, 0] = i;

    for (int j = 0; j <= secondLen; j++)
        distMatrix[0, j] = j;

    for (int i = 1; i <= firstLen; i++)
    {
        for (int j = 1; j <= secondLen; j++)
        {
            int cost = (first.Substring(i - 1, 1) == second.Substring(j - 1, 1)) ? 0 : 1;
            
            int insert = distMatrix[i, j - 1] + 1;
            int delete = distMatrix[i - 1, j] + 1;
            int substitute = distMatrix[i - 1, j - 1] + cost;

            distMatrix[i, j] = Math.Min(Math.Min(insert, delete), substitute);
        }
    }

    return distMatrix[firstLen, secondLen];
}

Console.WriteLine("Моя программа для вычисления расстояния Левенштейна");
Console.WriteLine();
Console.WriteLine("Введите две строки, чтобы узнать расстояние между ними.");
Console.WriteLine("Если хотите выйти, введите 'exit' как первую строку.");
Console.WriteLine();

while (true)
{
    Console.Write("Первая строка: ");
    string? userInput1 = Console.ReadLine();

    if (userInput1 == null || userInput1.ToLower() == "exit")
    {
        Console.WriteLine("Программа завершена.");
        break;
    }

    Console.Write("Вторая строка: ");
    string? userInput2 = Console.ReadLine();

    if (userInput2 == null)
    {
        Console.WriteLine("Программа завершена.");
        break;
    }

    int result = ComputeLevenshteinDistance(userInput1, userInput2);
    Console.WriteLine($"Результат: {result}");
    Console.WriteLine();
}