using System;

class RootFinding
{
    static void Main(string[] args)
    {
        // Определение функции F(x)
        Func<double, double> F = x => Math.Exp(x) - 3;//Пример функции                         Math.Pow(x, 2) - 2; ;

        // Производная функции для метода Ньютона
        Func<double, double> FPrime = x => Math.Exp(x);//                                      2 *x;

        // Начальные параметры
        double a = 0, b = 2; // Интервал для методов деления отрезка и секущих                 a = 1, b = 2;
        double x0 = 1.5;     // Начальное приближение для метода Ньютона
        double epsilon = 1e-6; // Точность

        //Func<double, double> F = x => Math.Pow(x, 3) - 3 * x + 2;
        //Func<double, double> FPrime = x => 3 * Math.Pow(x, 2) - 3;
        //double a = -3, b = -2; 
        //double x0 = -2.5;      
        //double epsilon = 1e-6;

        // Метод деления отрезка пополам
        double bisectionRoot = BisectionMethod(F, a, b, epsilon);
        Console.WriteLine($"Метод деления отрезка пополам: x = {bisectionRoot}");

        // Метод Ньютона
        double newtonRoot = NewtonMethod(F, FPrime, x0, epsilon);
        Console.WriteLine($"Метод Ньютона: x = {newtonRoot}");
    }

    static double BisectionMethod(Func<double, double> F, double a, double b, double epsilon)
    {
        while (Math.Abs(b - a) > epsilon) // пока длина интервала [b−a] больше заданной точности
        {
            double c = (a + b) / 2;
            if (F(c) == 0 || (b - a) / 2 < epsilon) //Если F(c) равно нулю или длина половины интервала становится меньше точности
                return c;

            if (Math.Sign(F(a)) == Math.Sign(F(c))) //Если F(a) и F(c) имеют одинаковый знак, корень находится в правой половине 
                a = c;
            else //Иначе корень в левой половине
                b = c;
        }
        return (a + b) / 2; //возвращаем середину текущего интервала как найденное значение корня
    }

    static double NewtonMethod(Func<double, double> F, Func<double, double> FPrime, double x0, double epsilon)
    {
        while (Math.Abs(F(x0)) > epsilon) // Продолжаем вычисления, пока значение функции F(x0) не станет достаточно близко к нулю
        {
            x0 = x0 - F(x0) / FPrime(x0);
        }
        return x0;
    }
}