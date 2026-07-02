using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkTemplate.scripts.cqrs.visualnovel.@event;
using GFrameworkTemplate.scripts.data.story;
using GFrameworkTemplate.scripts.enums.visualnovel;
using Godot;

namespace GFrameworkTemplate.global;

/// <summary>
///     立绘栏全局单例——3 槽位（左/中/右），自动分配，交叉淡入淡出切换表情
/// </summary>
[Log]
[ContextAware]
public partial class TachieManager : CanvasLayer
{
    private TextureRect LeftSlot => GetNode<TextureRect>("%LeftSlot");
    private TextureRect CenterSlot => GetNode<TextureRect>("%CenterSlot");
    private TextureRect RightSlot => GetNode<TextureRect>("%RightSlot");
    private TextureRect HelperSlot => GetNode<TextureRect>("%HelperSlot");

    /// <summary>角色名 → 槽位名</summary>
    private readonly Dictionary<string, string> _charToSlot = new();
    /// <summary>槽位名 → 当前角色名</summary>
    private readonly Dictionary<string, string> _slotToChar = new();

    private static readonly string[] SlotOrder = { "Left", "Center", "Right" };

    public override void _Ready()
    {
        this.RegisterEvent<VisualNovelTachieTriggeredEvent>(OnTachie).UnRegisterWhenNodeExitTree(this);
    }

    private void OnTachie(VisualNovelTachieTriggeredEvent e)
    {
        foreach (var (charName, slot) in e.Tachies)
        {
            switch (slot.Type)
            {
                case TachieOperation.Show:  ShowChar(charName, slot.FilePath); break;
                case TachieOperation.Change: ChangeChar(charName, slot.FilePath); break;
                case TachieOperation.Close:  CloseChar(charName); break;
            }
        }
    }

    private void ShowChar(string name, string path)
    {
        if (_charToSlot.ContainsKey(name)) return; // 已在场

        var slotName = AllocSlot();
        if (slotName == null) { _log.Warn($"立绘槽位已满，无法显示: {name}"); return; }

        var rect = GetSlotRect(slotName);
        var tex = LoadTexture(path);
        if (tex == null) return;

        rect.Texture = tex;
        rect.Visible = true;
        rect.Modulate = Colors.White;

        _charToSlot[name] = slotName;
        _slotToChar[slotName] = name;
    }

    private async void ChangeChar(string name, string path)
    {
        if (!_charToSlot.TryGetValue(name, out var slotName))
        {
            // 不在场则自动 show
            ShowChar(name, path);
            return;
        }

        var rect = GetSlotRect(slotName);
        var newTex = LoadTexture(path);
        if (newTex == null) return;

        // 用 HelperSlot 交叉淡入淡出
        HelperSlot.Texture = newTex;
        HelperSlot.Position = rect.Position;
        HelperSlot.Size = rect.Size;
        HelperSlot.Modulate = Colors.Transparent;
        HelperSlot.Visible = true;

        var tween = CreateTween();
        tween.TweenProperty(HelperSlot, "modulate", Colors.White, 0.3f);
        tween.Parallel().TweenProperty(rect, "modulate", Colors.Transparent, 0.3f);
        await ToSignal(tween, Tween.SignalName.Finished);

        rect.Texture = newTex;
        rect.Modulate = Colors.White;
        HelperSlot.Visible = false;
    }

    private void CloseChar(string name)
    {
        if (!_charToSlot.Remove(name, out var slotName)) return;
        _slotToChar.Remove(slotName);
        GetSlotRect(slotName).Visible = false;
    }

    private string? AllocSlot()
    {
        foreach (var s in SlotOrder)
            if (!_slotToChar.ContainsKey(s))
                return s;
        return null;
    }

    private TextureRect GetSlotRect(string slotName) => slotName switch
    {
        "Left" => LeftSlot,
        "Center" => CenterSlot,
        "Right" => RightSlot,
        _ => LeftSlot
    };

    private Texture2D? LoadTexture(string logicalName)
    {
        var p = StoryResourceMapper.ResolveTexturePath(logicalName);
        if (string.IsNullOrEmpty(p)) { _log.Warn($"立绘纹理未注册: {logicalName}"); return null; }
        return GD.Load<Texture2D>(p);
    }
}
