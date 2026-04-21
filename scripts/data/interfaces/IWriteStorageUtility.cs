using GFramework.Core.Abstractions.utility;

namespace GFrameworkGodotTemplate.scripts.data.interfaces;

/// <summary>
/// 存储写入工具接口，定义从存储系统读取数据的基本操作
/// </summary>
public interface IWriteStorageUtility: IUtility
{
    /// <summary>
    /// 将内容写入存储中的指定键
    /// </summary>
    /// <param name="key">要写入的键</param>
    /// <param name="content">要写入的内容</param>
    void Write(string key, string content);
}