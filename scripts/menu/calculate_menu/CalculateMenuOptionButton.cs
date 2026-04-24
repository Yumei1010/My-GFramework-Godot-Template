using Godot;

namespace TimeToTwentyfour.scripts.menu.calculate_menu;

public abstract partial class CalculateMenuOptionButton : Button , ICalculateMenuOptionButton
{
    public override void _Ready()
    {
        _ = ReadyAsync();
        ConnectSignal();
    }

    public new void SetText(string text)
    {
        TextContent = text;
        
        UpdateTextLabel();
    }
    
    public void SetTextColor(Color color)
    {
        TextColor = color;
        
        UpdateTextColor();
    }

    public void SetBackgroundColor(Color color)
    {
        BackgroundColor = color;
        
        UpdateBackgroundRect();
    }

    public void UpdateTextLabel()
    {
        if (!string.IsNullOrWhiteSpace(TextContent)) TextLabel.Text = TextContent;
    }

    public void UpdateTextLabel(string text)
    {
        TextContent = text;
        
        UpdateTextLabel();
    }

    public abstract void OnMouseDown();

    public abstract void OnMouseUp();

    public abstract void OnMouseEnter();

    public abstract void OnMouseExit();

    private void UpdateTextColor()
    {
        var labelSettings = new LabelSettings();
        labelSettings.FontColor = TextColor;
        labelSettings.FontSize = 16;
        TextLabel.LabelSettings = labelSettings;
    }

    private void UpdateBackgroundRect()
    {
        BackgroundRect.Color = BackgroundColor;
    }
}