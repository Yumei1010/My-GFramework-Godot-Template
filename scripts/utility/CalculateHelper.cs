using System.Globalization;
using System.Runtime.InteropServices;
using GFrameworkGodotTemplate.scripts.enums.calculate;
using GFrameworkGodotTemplate.scripts.enums.poker;
using GFrameworkGodotTemplate.scripts.poker;

namespace GFrameworkGodotTemplate.scripts.utility;

/// <summary>
/// 计算辅助类
/// </summary>
public static class CalculateHelper
{
    private const double DoubleEpsilon = 1e-9;
    private const long MaxFractionDenominator = 10_000;
    
    [StructLayout(LayoutKind.Auto)]
    private readonly struct Fraction
    {
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

        public override string ToString() => Denominator == 1 ? Numerator.ToString(CultureInfo.InvariantCulture) : $"{Numerator}/{Denominator}";

        private static long Gcd(long a, long b) { while (b != 0) { long t = b; b = a % b; a = t; } return a; }
    }

    /// <summary>
    /// 二元计算，包含加，减，乘，除，模，指数幂，N次根
    /// </summary>
    /// <param name="pokerA">手牌A <see cref="IPoker"/></param>
    /// <param name="pokerB">手牌B <see cref="IPoker"/></param>
    /// <param name="mode">运算方式 <see cref="ModeType"/></param>
    public static string Calculate(IPoker pokerA, IPoker pokerB,ModeType mode)
    {
        Fraction fa = ParseToFraction(pokerA);
        Fraction fb = ParseToFraction(pokerB);

        return mode switch
        {
            ModeType.Add => (fa + fb).ToString(),
            ModeType.Subtract => (fa - fb).ToString(),
            ModeType.Multiply => (fa * fb).ToString(),
            ModeType.Divide => fb.Numerator == 0 ? "ERROR:DivByZero" : (fa / fb).ToString(),
            ModeType.Modulo => fb.ToDouble() == 0 ? "ERROR:DivByZero" : FormatDouble(fa.ToDouble() % fb.ToDouble()),
            ModeType.Power => FormatDouble(Math.Pow(fa.ToDouble(), fb.ToDouble())),
            ModeType.NthRoot => fb.ToDouble() == 0 ? "ERROR:ZeroRootIndex" : FormatDouble(Math.Pow(fb.ToDouble(), 1.0 / fa.ToDouble())),
            _ => "24"
        };
    }
    
    /// <summary>
    /// 一元计算，包含绝对值，阶乘，平方根，向上取整，向下取整
    /// </summary>
    /// <param name="poker">手牌A <see cref="IPoker"/></param>
    /// <param name="mode">运算方式 <see cref="ModeType"/></param>
    public static string Calculate(IPoker poker, ModeType mode)
    {
        double val = ParseToFraction(poker).ToDouble();

        return mode switch
        {
            ModeType.AbsoluteValue => FormatDouble(Math.Abs(val)),
            ModeType.Factorial => FormatDouble(CalculateFactorial(val)),
            ModeType.SquareRoot => val < 0 ? "ERROR:InvalidSqrt" : FormatDouble(Math.Sqrt(val)),
            ModeType.Ceil => FormatDouble(Math.Ceiling(val)),
            ModeType.Floor => FormatDouble(Math.Floor(val)),
            _ => "24"
        };
    }

    private static Fraction ParseToFraction(IPoker poker)
    {
        string raw = poker.GetNumValue().Trim();
        return poker.GetNumType() switch
        {
            NumType.Fraction => ParseFractionString(raw),
            NumType.Decimal or NumType.Integer => ParseDecimalString(raw),
            _ => throw new NotSupportedException($"未知数值类型: {poker.GetNumType()}")
        };
    }

    private static Fraction ParseFractionString(string frac)
    {
        var parts = frac.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 2 ||
            !long.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out long num) ||
            !long.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out long den) ||
            den == 0)
            throw new FormatException($"无效的分数格式: {frac}");
        return new Fraction(num, den);
    }

    private static Fraction ParseDecimalString(string dec)
    {
        if (double.TryParse(dec, NumberStyles.Float, CultureInfo.InvariantCulture, out double d)) return Fraction.FromDouble(d);
        throw new FormatException($"无效的数值格式: {dec}");
    }
    
    private static double CalculateFactorial(double n)
    {
        if (n < 0) return -1;
        if (Math.Abs(n - Math.Round(n)) > DoubleEpsilon) return -1;
        
        long result = 1;
        for (int i = 2; i <= (int)Math.Round(n); i++)
        {
            result *= i;
        }
        return result;
    }
    
    private static string FormatDouble(double value)
    {
        if (Math.Abs(value - Math.Round(value)) < DoubleEpsilon) return Math.Round(value).ToString(CultureInfo.InvariantCulture);
        return Math.Round(value, 1, MidpointRounding.AwayFromZero).ToString("F1", CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');
    }
}