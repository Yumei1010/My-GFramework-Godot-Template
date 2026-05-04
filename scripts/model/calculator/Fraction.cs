using System.Globalization;
using System.Runtime.InteropServices;

namespace TimeToTwentyfour.scripts.model.calculator;

[StructLayout(LayoutKind.Auto)]
internal readonly struct Fraction
{
    private const double DoubleEpsilon = 1e-9;
    private const long MaxFractionDenominator = 10_000;

    public long Numerator { get; }
    private long Denominator { get; }

    public Fraction(long num, long den)
    {
        if (den == 0) throw new DivideByZeroException();
        long g = Gcd(Math.Abs(num), Math.Abs(den));
        Numerator = (den < 0 ? -num : num) / g;
        Denominator = Math.Abs(den) / g;
    }

    public static Fraction FromDouble(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value)) return new Fraction(0, 1);
        if (Math.Abs(value - Math.Round(value)) < DoubleEpsilon) return new Fraction((long)Math.Round(value), 1);

        long num0 = 0, den0 = 1;
        long num1 = 1, den1 = 0;
        double x = Math.Abs(value);
        for (int i = 0; i < 30; i++)
        {
            long a = (long)Math.Floor(x);
            long num2 = a * num1 + num0;
            long den2 = a * den1 + den0;
            if (den2 > MaxFractionDenominator) break;
            num0 = num1; den0 = den1;
            num1 = num2; den1 = den2;
            double rem = x - a;
            if (rem < DoubleEpsilon) break;
            x = 1.0 / rem;
        }
        return new Fraction(value < 0 ? -num1 : num1, den1);
    }

    public double ToDouble() => (double)Numerator / Denominator;

    public static Fraction operator +(Fraction a, Fraction b) => new(a.Numerator * b.Denominator + b.Numerator * a.Denominator, a.Denominator * b.Denominator);
    public static Fraction operator -(Fraction a, Fraction b) => new(a.Numerator * b.Denominator - b.Numerator * a.Denominator, a.Denominator * b.Denominator);
    public static Fraction operator *(Fraction a, Fraction b) => new(a.Numerator * b.Numerator, a.Denominator * b.Denominator);
    public static Fraction operator /(Fraction a, Fraction b) => new(a.Numerator * b.Denominator, a.Denominator * b.Numerator);

    /// <summary>
    ///     智能格式化：整数→整数，有限小数→小数（分母质因数仅含 2 和 5），无限小数→分数 "num/den"。
    /// </summary>
    public override string ToString()
    {
        if (Denominator == 1)
            return Numerator.ToString(CultureInfo.InvariantCulture);

        if (IsTerminatingDenominator(Denominator))
        {
            double value = (double)Numerator / Denominator;
            if (Math.Abs(value - Math.Round(value)) < DoubleEpsilon)
                return Math.Round(value).ToString(CultureInfo.InvariantCulture);
            return value.ToString("0.##############################", CultureInfo.InvariantCulture);
        }

        return $"{Numerator}/{Denominator}";
    }

    /// <summary>检查分母的质因数是否只有 2 和 5（即分数可表示为有限小数）。</summary>
    private static bool IsTerminatingDenominator(long den)
    {
        while (den % 2 == 0) den /= 2;
        while (den % 5 == 0) den /= 5;
        return den == 1;
    }

    private static long Gcd(long a, long b) { while (b != 0) { long t = b; b = a % b; a = t; } return a; }
}
