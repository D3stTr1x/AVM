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

namespace AVM7
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void zedGraphControl1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Начальные условия для решения ОДУ
            double x0 = 0, y0 = 1, h = 0.0001; // Начальные значения x и y и шаг интегртрования
            int steps = 1000;  // Количество шагов

            // Решение методом Эйлера
            var eulerResults = SolveByEuler(x0, y0, h, steps);

            // Решение явным методом Адамса 4-го порядка
            var adamsResults = SolveByAdams4(x0, y0, h, steps);

            var originalResults = GetExactSolution(x0, y0, h, steps);

            var graphPane = zedGraphControl1.GraphPane;
            graphPane.CurveList.Clear();

            graphPane.Title.Text = "Сравнение методов решения ОДУ";
            graphPane.XAxis.Title.Text = "x";
            graphPane.YAxis.Title.Text = "y";

            // График для метода Эйлера
            var eulerCurve = new PointPairList();
            foreach (var point in eulerResults)
            {
                eulerCurve.Add(point.Key, point.Value);
                Debug.WriteLine($"Эулер - Key - {point.Key}, Value - {point.Value}");
            }
            graphPane.AddCurve("Эйлер", eulerCurve, Color.Blue, SymbolType.XCross);

            // График для метода Адамса
            var adamsCurve = new PointPairList();
            foreach (var point in adamsResults)
            {
                adamsCurve.Add(point.Key, point.Value);
                Debug.WriteLine($"Адамс - Key - {point.Key}, Value - {point.Value}");
            }
            graphPane.AddCurve("Явный метод Адамса", adamsCurve, Color.Red, SymbolType.Diamond);

            var originalCurve = new PointPairList();
            foreach (var point in originalResults)
            {
                originalCurve.Add(point.Key, point.Value);
            }
            graphPane.AddCurve("Оригинальное решение", originalCurve, Color.Green, SymbolType.UserDefined);

            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }

        // Метод Эйлера
        private List<KeyValuePair<double, double>> SolveByEuler(double x0, double y0, double h, int steps)
        {
            var results = new List<KeyValuePair<double, double>> { new KeyValuePair<double, double>(x0, y0) };

            // Начинаем цикл с 1, потому что начальное значение уже добавлено
            for (int i = 1; i <= steps; i++)
            {
                // Вычисление следующего значения y с использованием метода Эйлера
                y0 += h * F(x0, y0);  // F(x, y) - функция, определяющая дифференциальное уравнение
                x0 += h;

                results.Add(new KeyValuePair<double, double>(x0, y0));
            }

            return results;
        }

        // Явный метод Адамса 4-го порядка
        private List<KeyValuePair<double, double>> SolveByAdams4(double x0, double y0, double h, int steps)
        {
            // Используем метод Эйлера для получения первых четырех значений
            var initialPoints = SolveByEuler(x0, y0, h, 4); // Эйлер для первых 4 шагов

            List<KeyValuePair<double, double>> result = new List<KeyValuePair<double, double>>(initialPoints);

            // Вычисление значений функции f(x, y) для первых 4 точек
            double[] f = new double[4];
            for (int i = 0; i < 4; i++)
            {
                f[i] = F(result[i].Key, result[i].Value);
            }

            // Начинаем вычисления с 4-й точки
            double x = result[3].Key;
            double y = result[3].Value;

            for (int step = 4; step < steps; step++)
            {
                // Применяем формулу Адамса 4-го порядка
                y += h / 24 * (55 * f[3] - 59 * f[2] + 37 * f[1] - 9 * f[0]);
                x += h;

                // Сдвигаем значения в массиве f
                for (int i = 0; i < 3; i++)
                {
                    f[i] = f[i + 1];
                }
                f[3] = F(x, y);

                // Добавляем новую точку в результат
                result.Add(new KeyValuePair<double, double>(x, y));
            }

            return result;
        }

        private List<KeyValuePair<double, double>> GetExactSolution(double x0, double y0, double h, double steps)
        {
            var points = new List<KeyValuePair<double, double>>();

            // Расчет конечной точки xEnd
            double xEnd = x0 + steps * h;

            // Заполнение точками
            for (double x = x0; x <= xEnd; x += h)
            {
                double y = Math.Exp(x); // точное решение y' = y при y(0) = 1
                points.Add(new KeyValuePair<double, double>(x, y));
            }

            return points;
        }

        private double F(double x, double y)
        {
            return y;
        }
    }
}
