namespace TimeToTwentyfour.scripts.menu.main_menu;

/// <summary>
///     <see cref="MainMenuOptionButton"/> 的 Godot 信号连接文件。
/// </summary>
public partial class MainMenuOptionButton
{
    private void ConnectSignal()
    {
        ButtonDown += OnMouseDowned;
        ButtonUp += OnMouseUped;
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }
    
    private void OnMouseDowned()
    {
        OnMouseDown();
    }

    private void OnMouseUped()
    {
        OnMouseUp();
    }
    
    private void OnMouseEntered()
    {
        OnMouseEnter();
    }

    private void OnMouseExited()
    {
        OnMouseExit();
    }
}