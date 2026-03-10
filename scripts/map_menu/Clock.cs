using Godot;

namespace GFrameworkGodotTemplate.scripts.clock_menu;

public partial class Clock : TextureRect
{
    private TextureButton One => GetNode<TextureButton>("%One");
    private TextureButton Two => GetNode<TextureButton>("%Two");
    private TextureButton Three => GetNode<TextureButton>("%Three");
    private TextureButton Four => GetNode<TextureButton>("%Four");
    private TextureButton Five => GetNode<TextureButton>("%Five");
    private TextureButton Six => GetNode<TextureButton>("%Six");
    private TextureButton Seven => GetNode<TextureButton>("%Seven");
    private TextureButton Eight => GetNode<TextureButton>("%Eight");
    private TextureButton Nine => GetNode<TextureButton>("%Nine");
    private TextureButton Ten => GetNode<TextureButton>("%Ten");
    private TextureButton Eleven => GetNode<TextureButton>("%Eleven");
    private TextureButton Twelve => GetNode<TextureButton>("%Twelve");
    private Node2D HourHand => GetNode<Node2D>("%HourHand");
    private Node2D MinuteHand => GetNode<Node2D>("%MinuteHand");

    public override void _Ready()
    {
        One.ButtonDown += () => { };
        Two.ButtonDown += () => { };
        Three.ButtonDown += () => { };
        Four.ButtonDown += () => { };
        Five.ButtonDown += () => { };
        Six.ButtonDown += () => { };
        Seven.ButtonDown += () => { };
        Eight.ButtonDown += () => { };
        Nine.ButtonDown += () => { };
        Ten.ButtonDown += () => { };
        Eleven.ButtonDown += () => { };
        Twelve.ButtonDown += () => { };
    }
}
