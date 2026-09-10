using GFramework.Godot.Setting.Data;
using GFrameworkTemplate.scripts.cqrs.audio.command.input;

namespace GFrameworkTemplate.Tests;

/// <summary>
///     音频设置链路验证：docs/guides/audio.md 的可单测部分。
///     AudioBusMap 与命令输入类都是纯数据，不碰 Godot 音频运行时。
/// </summary>
public class AudioSystemTests
{
    [Fact]
    public void AudioBusMap_默认映射三条总线()
    {
        var map = new AudioBusMap();

        Assert.Equal("Master", map.Master);
        Assert.Equal("BGM", map.Bgm);
        Assert.Equal("SFX", map.Sfx);
    }

    [Fact]
    public void AudioBusMap_可扩展新总线()
    {
        var map = new AudioBusMap { Bgm = "音樂" };   // 允许改名/扩展
        Assert.Equal("音樂", map.Bgm);
    }

    [Fact]
    public void 音量输入_默认0且可赋值()
    {
        var input = new ChangeBgmVolumeCommandInput();
        Assert.Equal(0f, input.Volume);          // 默认 0

        input.Volume = 0.5f;
        Assert.Equal(0.5f, input.Volume);
    }

    [Fact]
    public void 音量输入_边界值0和1()
    {
        var master = new ChangeMasterVolumeCommandInput { Volume = 0f };
        var sfx = new ChangeSfxVolumeCommandInput { Volume = 1f };
        Assert.Equal(0f, master.Volume);
        Assert.Equal(1f, sfx.Volume);
    }
}