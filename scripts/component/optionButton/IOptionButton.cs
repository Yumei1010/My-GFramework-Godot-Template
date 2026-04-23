namespace TimeToTwentyfour.scripts.component.optionButton;

/// <summary>
/// 选项按钮接口，定义了选项按钮的基本属性和必须实现的功能
/// </summary>
public interface IOptionButton
{
    /// <summary>
    /// 设置文本内容
    /// </summary>
    /// <param name="text">目标文本内容 <see cref="string"/></param>
    void SetText(string text);
    
    /// <summary>
    /// 更新文本内容时调用的方法
    /// </summary>
    void UpdateTextLabel();
    
    /// <summary>
    /// 更新文本内容时调用的方法
    /// </summary>
    /// <param name="text">目标文本内容 <see cref="string"/></param>
    void UpdateTextLabel(string text);
}