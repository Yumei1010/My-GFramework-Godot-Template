using System.Collections.Generic;

namespace GFrameworkTemplate.scripts.component.behavior_tree;

/// <summary>
///     行为树共享黑板：以字符串键存取任意数据，供树内节点交换状态（如"当前目标""剩余弹药"）。
/// </summary>
/// <remarks>
///     纯逻辑类，零 Godot / GFramework 依赖，可独立单元测试。
///     键比较使用 <see cref="System.StringComparer.Ordinal" />；未设置的键返回默认值，不抛异常。
/// </remarks>
public sealed class Blackboard
{
    private readonly Dictionary<string, object?> _values = new(System.StringComparer.Ordinal);

    /// <summary>
    ///     已存放的数据项数量。
    /// </summary>
    public int Count => _values.Count;

    /// <summary>
    ///     写入数据（已存在则覆盖）。
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="key">键（不能为 null 或空白）</param>
    /// <param name="value">要存放的值</param>
    public void Set<T>(string key, T value)
    {
        System.ArgumentException.ThrowIfNullOrWhiteSpace(key);
        _values[key] = value;
    }

    /// <summary>
    ///     读取数据；键不存在或类型不匹配时返回默认值。
    /// </summary>
    /// <typeparam name="T">期望的数据类型</typeparam>
    /// <param name="key">键</param>
    /// <returns>存放的值；缺失或类型不符时为 <see langword="default" />。</returns>
    public T? Get<T>(string key)
    {
        System.ArgumentException.ThrowIfNullOrWhiteSpace(key);
        return _values.TryGetValue(key, out var value) && value is T typed ? typed : default;
    }

    /// <summary>
    ///     尝试读取指定类型的数据。
    /// </summary>
    /// <typeparam name="T">期望的数据类型</typeparam>
    /// <param name="key">键</param>
    /// <param name="value">读取到的值</param>
    /// <returns>存在且类型匹配时返回 true。</returns>
    public bool TryGet<T>(string key, out T? value)
    {
        System.ArgumentException.ThrowIfNullOrWhiteSpace(key);
        if (_values.TryGetValue(key, out var stored) && stored is T typed)
        {
            value = typed;
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    ///     判断键是否存在。
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>存在时返回 true。</returns>
    public bool ContainsKey(string key)
    {
        return !string.IsNullOrWhiteSpace(key) && _values.ContainsKey(key);
    }

    /// <summary>
    ///     移除指定键。
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>成功移除返回 true。</returns>
    public bool Remove(string key)
    {
        return !string.IsNullOrWhiteSpace(key) && _values.Remove(key);
    }

    /// <summary>
    ///     清空全部数据。
    /// </summary>
    public void Clear()
    {
        _values.Clear();
    }
}
