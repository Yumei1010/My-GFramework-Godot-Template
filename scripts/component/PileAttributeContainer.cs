using Godot;

namespace GFrameworkGodotTemplate.scripts.component;

public partial class PileAttributeContainer : HBoxContainer
{
    private TextureRect AttributeIconRect => GetNode<TextureRect>("%AttributeIconRect");
    private RichTextLabel AttributeNameLabel => GetNode<RichTextLabel>("%AttributeNameLabel");
    private RichTextLabel AttributeLevelLabel => GetNode<RichTextLabel>("%AttributeLevelLabel");
    
    private String _themePrefix = "[wave amp=5 freq=5.0]";
    private bool _displayIcon;
    private bool _displayName;
    
    public void Initialize(Color color,Texture2D iconTexture, String text,int level)
    {
        UpdateThemeColor(color);
        UpdateAttributeIcon(iconTexture);
        UpdateAttributeName(text);
        UpdateAttributeLevel(level);
    }
    
    public void Transform(Color color,Texture2D iconTexture, String text,int level)
    {
        UpdateThemeColor(color);
        UpdateAttributeIcon(iconTexture);
        UpdateAttributeName(text);
        UpdateAttributeLevel(level);
    }

    public void Display(bool displayIcon, bool displayName)
    {
        UpdateIconDisplay(displayIcon);
        UpdateNameDisplay(displayName);
    }
    
    private void UpdateThemeColor(Color color)
    {
        Modulate = color;
    }
    
    private void UpdateAttributeIcon(Texture2D iconTexture)
    {
        AttributeIconRect.Texture = iconTexture;
    }

    private void UpdateAttributeName(String text)
    {
        AttributeNameLabel.Text = _themePrefix + text;
    }

    private void UpdateAttributeLevel(int level)
    {
        AttributeLevelLabel.Text = _themePrefix;
        for (int i = 0; i < level; i++)
        {
            AttributeLevelLabel.Text += "-";
        }
    }
    
    private void UpdateIconDisplay(bool displayIcon)
    {
        AttributeIconRect.Visible = displayIcon;
    }

    private void UpdateNameDisplay(bool displayName)
    {
        AttributeNameLabel.Visible = displayName;
    }
}