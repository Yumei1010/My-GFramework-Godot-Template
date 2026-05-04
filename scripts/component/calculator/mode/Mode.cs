using System.Globalization;
using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.calculator;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.model.calculator;

namespace TimeToTwentyfour.scripts.component.calculator.mode;

/// <summary>
///     运算模式抽象基类
///     提供 Fraction 解析、数值格式化等共享工具方法。
///     子类只需重写 <see cref="Calculate(IPoker, IPoker)"/> 或 <see cref="Calculate(IPoker)"/>。
/// </summary>
public abstract class Mode : IMode
{
    private const double DoubleEpsilon = 1e-9;

    public abstract ModeType ModeType { get; }
    public abstract bool IsBinary { get; }

    /// <summary>
    ///     二元运算。非二元模式默认抛出 <see cref="NotSupportedException"/>。
    /// </summary>
    public virtual string Calculate(IPoker pokerA, IPoker pokerB) =>
        throw new NotSupportedException($"{GetType().Name} 不支持二元运算");

    /// <summary>
    ///     一元运算。非一元模式默认抛出 <see cref="NotSupportedException"/>。
    /// </summary>
    public virtual string Calculate(IPoker poker) =>
        throw new NotSupportedException($"{GetType().Name} 不支持一元运算");

    /// <summary>
    ///     将手牌数值解析为 <see cref="Fraction"/>，根据 <see cref="NumType"/> 选择解析策略。
    /// </summary>
    private protected static Fraction ParseToFraction(IPoker poker)
    {
        string raw = poker.NumValue.Trim();
        return poker.NumType switch
        {
            NumType.Fraction => ParseFractionString(raw),
            NumType.Decimal or NumType.Integer => ParseDecimalString(raw),
            _ => throw new NotSupportedException($"未知数值类型: {poker.NumType}")
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

    /// <summary>
    ///     计算阶乘，非正整数返回 -1。
    /// </summary>
    protected static double CalculateFactorial(double n)
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

    /// <summary>
    ///     格式化 double 值：整数去掉小数点，非整数保留一位小数并去除尾部零。
    /// </summary>
    protected static string FormatDouble(double value)
    {
        if (Math.Abs(value - Math.Round(value)) < DoubleEpsilon) return Math.Round(value).ToString(CultureInfo.InvariantCulture);
        return Math.Round(value, 1, MidpointRounding.AwayFromZero).ToString("F1", CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');
    }
}
