namespace TimeToTwentyfour.scripts.menu.calculate_menu;

public partial class CalculateMenuOptionButton
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