using System;
using System.Diagnostics;
using System.Xml.Linq;

class Program
{
    static void Main()
    {
        double[,] A =
        {
            { 2, -1, 2 },
            { 5, -3, 3 },
            { -1, 0, -2 }
        };

        // Начальный вектор
        double[] x = { 1, 1, 1};

        // Заданная точность
        double epsilon = 1e-6;

        // Метод прямой итерации
        (double eigenvalue, double[] eigenvector, int iterations) = PowerIteration(A, x, epsilon);

        // Вывод результата
        Console.WriteLine($"Собственное значение: {eigenvalue}");
        Console.WriteLine("Собственный вектор: [" + string.Join(", ", eigenvector) + "]");
        Console.WriteLine($"Количество итераций: {iterations}");
    }

    static (double eigenvalue, double[] eigenvector, int iterations) PowerIteration(double[,] A, double[] x, double epsilon)
    {
        int n = x.Length;//размерность матрицы и вектора
        double[] xNext = new double[n];//вектор, который будет содержать результаты на текущей итерации
        double lambdaOld = 0;
        double lambdaNew;//старое и новое собственное значение для проверки сходимости
        int iterations = 0;

        while (true) //умножение матрицы A на текущий вектор x, результат сохраняется в векторе xNext, на каждом шаге мы вычисляем новый вектор, умножая старый вектор на матрицу.
        {
            iterations++;

            // Умножение матрицы A на вектор x
            for (int i = 0; i < n; i++)
            {
                xNext[i] = 0;
                for (int j = 0; j < n; j++)
                {
                    xNext[i] += A[i, j] * x[j];
                }
            }

            // Нормализация вектора xNext для того, чтобы избежать его экспоненциального роста
            double norm = 0;
            for (int i = 0; i < n; i++)
            {
                norm += xNext[i] * xNext[i];
            }
            norm = Math.Sqrt(norm); // Норма - корень из суммы квадратов компонентов

            for (int i = 0; i < n; i++)
            {
                xNext[i] /= norm; // Каждай элемент вектора делится на норму, чтобы получить вектор длинной 1
            }

            // собственное значение
            double[] AxNext = new double[n]; // хранит результат умножения матрицы A на вектор xNext.
            for (int i = 0; i < n; i++) 
            {
                AxNext[i] = 0; // Обнуляем i-ый элемент массива AxNext перед вычислением.
                for (int j = 0; j < n; j++) 
                {
                    AxNext[i] += A[i, j] * xNext[j]; // Вычисляем i-ый элемент AxNext как скалярное произведение i-ой строки матрицы A и вектора xNext.
                }
            }
            lambdaNew = xNext.Zip(AxNext, (a, b) => a * b).Sum(); // сумма произведений соответствующих элементов двух векторов(скалярное произведение)

            Debug.WriteLine(lambdaNew - lambdaOld);
            
            // Проверка сходимости
            if (Math.Abs(lambdaNew - lambdaOld) < epsilon)
            {
                break;
            }

            // Если метод не сошелся, то старое собственное значение обновляется, а вектор x копируется из xNext для использования на следующей итерации
            lambdaOld = lambdaNew;
            Array.Copy(xNext, x, n);
        }

        return (lambdaNew, xNext, iterations);
    }
}
