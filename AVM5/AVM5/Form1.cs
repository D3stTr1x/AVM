using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace AVM5
{
    
    public partial class Form1 : Form
    {
        

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Коэфициенты системы
            double[,] matrix = {
                {1, 2, 4 },
                {2, 13, 24 },
                {4, 24, 77 }
                //{2.45, 1.75, -3.24 },
                //{1.75, -1.16, 2.18 },
                //{-3.24, 2.18, -1.85 }
                

                //{3, 1, -1},
                //{2, 4, 1},
                //{1, -1, 5}

                //{10, -1, 2, 0 },
                //{-1, 11, -1, 3},
                //{2, -1, 10, -1},
                //{0, 3, -1, 8}
            };

            double[] b = { 10, 50, 150 };
                //{ 6, 25, -11, 15 };
            //{ 4, 1, 1 };
            // { 1.23, 3.43, -0.16 };
            // Начальное приближение
            double[] x0 = { 0, 0, 0};
            // Создаем объект класса IterarionMetods
            IterarionMetods methods = new IterarionMetods(matrix, b, x0);

            // Получаем данные для метода 
            var seidelResiduals = methods.SolveSeidel(matrix, b, x0, 1e-3, 1000);
            bool seidelConverged = seidelResiduals.Last() < 1e-6;
            int seidelIterations = seidelResiduals.Count;

            // Получаем данные для метода минимальных невязок
            var minimalResiduals = methods.SolveMinimalResiduals(matrix, b, x0, 1e-3, 1000);
            bool minimalConverged = minimalResiduals.Last() < 1e-6;
            int minimalIterations = minimalResiduals.Count;

            var graph = zedGraphControl1.GraphPane;
            graph.CurveList.Clear(); 

            graph.Title.Text = "Сходимость методов";
            graph.XAxis.Title.Text = "Итерации";
            graph.YAxis.Title.Text = "Норма невязки";

            // Добавляем данные для метода Якоби
            var seidelCurve = new PointPairList();
            for (int i = 0; i < seidelResiduals.Count; i++)
            {
                seidelCurve.Add(i, seidelResiduals[i]);
            }
            LineItem Lines = graph.AddCurve("Метод Зейделя", seidelCurve, Color.Blue, SymbolType.Diamond);

            StringBuilder rungeOutput = new StringBuilder("Метод Зейделя:\n");
            foreach (var residual in seidelResiduals)
            {
                rungeOutput.AppendLine($"Норма Зейделя: {residual:F6}");
            }
            rungeOutput.AppendLine($"Метод Зейделя {(seidelConverged ? "сошелся" : "разошёлся")} за {seidelIterations} итераций.");
            Debug.WriteLine(rungeOutput.ToString());

            // Добавляем данные для метода минимальных невязок
            var minimalResidualCurve = new PointPairList();
            for (int i = 0; i < minimalResiduals.Count; i++)
            {
                minimalResidualCurve.Add(i, minimalResiduals[i]);
            }
            LineItem minimalResidualLine = graph.AddCurve("Метод минимальных невязок", minimalResidualCurve, Color.Red, SymbolType.Default);

            StringBuilder residualOutput = new StringBuilder("Метод Минимальных невязок:\n");
            foreach (var residual in minimalResiduals)
            {
                residualOutput.AppendLine($"Норма невязки минимаьных невязок: {residual:F6}");
            }
            residualOutput.AppendLine($"Метод Минимальных невязок {(minimalConverged ? "сошелся" : "разошёлся")} за {minimalIterations} итераций.");
            Debug.WriteLine(residualOutput.ToString());

            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }
    }

}

public class IterarionMetods
{
    private double[,] matrix;
    private double[] b;
    private double[] x0;

    // Конструктор для инициализации начальных данных
    public IterarionMetods(double[,] matrix, double[] b, double[] x0)
    {
        this.matrix = matrix;
        this.b = b;
        this.x0 = x0;
    }

    // Метод Зейделя
    public List<double> SolveSeidel(double[,] A, double[] b, double[] x0, double tolerance, int maxIterations)
    {
        // Получаем размер системы
        int n = b.Length;

        // Создаем массив для текущего приближения x, копируя начальное приближение x0
        double[] x = new double[n];
        Array.Copy(x0, x, n);

        // Список для хранения нормы невязки на каждой итерации
        var residuals = new List<double>();

        // Главный цикл, выполняющий итерации метода Зейделя
        for (int k = 0; k < maxIterations; k++)
        {
            // Массив для нового приближения x (следующая итерация)
            double[] xNew = new double[n];

            // Основной цикл по строкам матрицы (каждое уравнение системы)
            for (int i = 0; i < n; i++)
            {
                double sum1 = 0; // Сумма произведений для уже обновленных элементов (j < i)
                for (int j = 0; j < i; j++)
                {
                    sum1 += A[i, j] * xNew[j]; // Используем элементы из нового приближения
                }

                double sum2 = 0; // Сумма произведений для еще не обновленных элементов (j > i)
                for (int j = i + 1; j < n; j++)
                {
                    sum2 += A[i, j] * x[j]; // Используем элементы из предыдущего приближения
                }

                // Вычисляем новое значение x[i] с учетом текущих сумм
                xNew[i] = (b[i] - sum1 - sum2) / A[i, i];
            }
            


            // Вычисляем норму разницы между новым и предыдущим приближением
            double norm = 0;
            for (int i = 0; i < n; i++)
            {
                norm = Math.Max(norm, Math.Abs(xNew[i] - x[i])); // Используем максимум нормы
            }

            // Обновляем x для следующей итерации
            Array.Copy(xNew, x, n);

            // Вычисляем невязку r = b - Ax (разница между правой и левой частью уравнения)
            double[] r = CalculateResidual(A, b, x);
            residuals.Add(r.Max()); // Добавляем максимум по модулю невязки в список

            // Проверяем условие сходимости (если норма разницы меньше заданной точности)
            if (residuals[k] < tolerance || double.IsNaN(residuals[k]))
            {
                break; // Прерываем итерации, так как решение достаточно точное
            }
        }
        Debug.WriteLine("Зейдел ");
        for (int i = 0; i < n; i++)
            Debug.Write(x[i] + " ");
        Debug.WriteLine(" ");

        // Возвращаем список норм невязок для анализа сходимости
        return residuals;
    }

    // Получение минорной матрицы (для детерминанта)
    private double[,] GetMinor(double[,] matrix, int size)
    {
        double[,] minor = new double[size, size]; // Создаем новую матрицу для хранения минора
        for (int i = 0; i < size; i++)  // Копируем только первые `size x size` элементы из входной матрицы
        {
            for (int j = 0; j < size; j++)
            {
                minor[i, j] = matrix[i, j]; // Копируем элемент из оригинальной матрицы в минор
            }
        }
        return minor;
    }

    // Вычисление детерминанта матрицы (рекурсивный метод, используется только для проверки)
    private double Determinant(double[,] matrix)
    {
        int n = matrix.GetLength(0); // Определяем размерность матрицы (число строк/столбцов)
        if (n == 1) // Особый случай: если матрица 1x1, детерминант равен единственному элементу
            return matrix[0, 0];
        if (n == 2) // Особый случай: если матрица 2x2, используем простую формулу
            return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];

        double det = 0;
        for (int i = 0; i < n; i++) // Рекурсивно вычисляем детерминант методом разложения по первой строке
        {
            double[,] minor = GetMinor(matrix, n - 1); // Получаем минор матрицы, исключая первую строку и текущий столбец
            det += Math.Pow(-1, i) * matrix[0, i] * Determinant(minor); // Вычисляем детерминант с учетом чередование знаков
        }
        return det;
    }

    // Вычисление невязки r = b - Ax
    private double[] CalculateResidual(double[,] matrix, double[] b, double[] x)
    {
        int n = b.Length;
        double[] residual = new double[n];

        for (int i = 0; i < n; i++)
        {
            double Ax = 0; // Переменная для хранения значения произведения строки матрицы A на вектор x
            for (int j = 0; j < n; j++)
            { 
                Ax += matrix[i, j] * x[j]; // Считаем скалярное произведение строки i и вектора x
            }
            residual[i] = b[i] - Ax; // r[i] = b[i] - (A * x)
        }

        return residual;
    }

    // Метод минимальных невязок
    public List<double> SolveMinimalResiduals(double[,] A, double[] b, double[] x0, double tolerance, int maxIterations)
    {
        int n = b.Length;
        double[] x = new double[n];
        Array.Copy(x0, x, n);

        var residuals = new List<double>();

        for (int k = 0; k < maxIterations; k++)
        {
            // Вычисляем невязку r = b - Ax
            double[] r = CalculateResidual(A, b, x);

            double[] Ar = MultiplyMatrixVector(A, r);
            double alphaNumerator = SkalyarnPr(r, r); // r^T * r
            double alphaDenominator = SkalyarnPr(r, Ar); // r^T * A * r

            // Проверка, чтобы избежать деления на 0
            if (Math.Abs(alphaDenominator) < 1e-12)
            {
                residuals.Add(r.Max());
                break;
            }

            double alpha = alphaNumerator / alphaDenominator;

            // Обновляем приближение x
            for (int i = 0; i < n; i++)
            {
                x[i] += alpha * r[i];
            }

            // Считаем норму невязки (максимальное значение модуля элементов вектора r)
            residuals.Add(r.Max());

            // Проверка сходимости
            if (residuals[k] < tolerance || double.IsNaN(residuals[k]))
                break;
        }
        Debug.WriteLine("Минимальные невязки: ");
        for (int i = 0; i < n; i++)
            Debug.Write(x[i] + " ");
        Debug.WriteLine(" ");
        return residuals;
    }

    // Умножение матрицы на вектор
    private double[] MultiplyMatrixVector(double[,] A, double[] v)
    {
        int n = v.Length;
        double[] result = new double[n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                result[i] += A[i, j] * v[j]; // Умножаем строку матрицы на вектор, считаем произведение и накапливаем в result[i]
            }
        }

        return result;
    }

    // Скалярное произведение двух векторов
    private double SkalyarnPr(double[] v1, double[] v2)
    {
        double result = 0;
        for (int i = 0; i < v1.Length; i++)
        {
            result += v1[i] * v2[i]; // Перебираем элементы обоих векторов, умножаем соответствующие элементы и накапливаем сумму
        }
        return result;
    }
}
