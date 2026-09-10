# 音频设置与音量管理教程（Audio）

> 本文纯教学：讲清"音频音量怎么管理 + 播放器怎么组织"。
> 框架提供音量管理的设置链路（`GodotAudioSettings` + `AudioBusMap` + `ISettingsSystem`），播放器本身用 Godot 原生。
> 模板已接入完整音频 CQRS 域（`scripts/cqrs/audio/`）——本文以它为例讲解 + 扩展指南。

---

## 0. 一句话先说明白

**音频管理 = 音量设置链路（命令 → 设置模型 → 应用器 → Godot 总线）+ 播放器播放（Godot 原生）+ 可扩展的总线映射。**

框架管"音量持久化与总线写入"，不管"谁在播放"——播放器组织是你的代码（通常配合对象池/事件）。

---

## 1. 架构分层（模板现状）

```
[UI 音量滑杆] ──SendCommand──► ChangeBgmVolumeCommand
                                     │
                              [CQRS 命令]
                                     │ 改模型 + 应用
                                     ▼
                         ISettingsModel.GetData<AudioSettings>()
                                     │
                              [ISettingsSystem.Apply<GodotAudioSettings>]
                                     │
                                     ▼
               GodotAudioSettings.ApplyAsync() → AudioServer 总线音量
                    （AudioBusMap 决定写哪条总线：Master/BGM/SFX）
```

**关键**：模板把"音频管理"做成了 **CQRS 域**——命令即接口，音量改动是设置的一部分（可持久化、可重置）。

## 2. 核心组件

| 组件 | 作用 |
|---|---|
| `ChangeMasterVolumeCommand` / `ChangeBgmVolumeCommand` / `ChangeSfxVolumeCommand` | 音量控制命令（CQRS） |
| `AudioSettings` | 设置数据（Master/Bgm/Sfx 音量 0-1） |
| `GodotAudioSettings` | 应用器：把模型音量写入 Godot 总线 |
| `AudioBusMap` | 总线名映射：`Master` / `BGM` / `SFX`（可扩展） |
| `VolumeChangedEvent` | 音量变化事件（广播给 UI 刷新） |
| `ISettingsSystem.Apply<T>()` | 触发应用器执行 |

## 3. 用法（模板命令示例）

```csharp
// 发命令（UI 滑杆回调）
this.SendCommand(new ChangeBgmVolumeCommand(new ChangeBgmVolumeCommandInput
{
    Volume = 0.6f
}));
```

命令内部做了什么（模板 `ChangeBgmVolumeCommand`）：

```csharp
protected override async Task OnExecuteAsync(ChangeBgmVolumeCommandInput input)
{
    var model = this.GetModel<ISettingsModel>()!;
    model.GetData<AudioSettings>().BgmVolume = input.Volume;       // 1. 改设置数据
    await this.GetSystem<ISettingsSystem>()!.Apply<GodotAudioSettings>()  // 2. 应用
        .ConfigureAwait(false);
}
// → GodotAudioSettings.ApplyAsync() 内部写 AudioServer 对应总线音量
```

**音量变化广播**：应用后发 `VolumeChangedEvent`，UI/音效系统订阅刷新（示例看模板 `scripts/cqrs/audio/event/`）。

## 4. AudioBusMap：扩展新总线

默认三条总线：**Master / BGM / SFX**。想加"环境音"总线：

```csharp
// 1. 扩展映射（框架 AudioBusMap 是普通类，可建子类或 JsonConvert 扩展）
public class MyAudioBusMap : AudioBusMap
{
    public string Ambience { get; set; } = "Ambience";   // 新总线
}

// 2. ModelModule 注册时用扩展映射
.RegisterApplicator(new GodotAudioSettings(model, new MyAudioBusMap()));
```

> Godot 侧：AudioServer 里要先建好对应总线（Audio > 总线布局），`AudioBusMap` 名字要和它一致。

## 5. 播放器组织（框架外，建议做法）

框架没有"播放管理器"——AudioStreamPlayer 的组织是你的代码。模板起手建议：

### 方案 A：节点 + 事件（适合中小规模）

```csharp
[Log]
[ContextAware]
public partial class SfxPlayer : Node
{
    [Export] private AudioStreamPlayer _player = null!;

    // 播放一个音效
    public void PlayOneShot(AudioStream clip, float volume = 1f)
    {
        _player.Stream = clip;
        _player.VolumeDb = Mathf.LinearToDb(volume);
        _player.Play();
    }
}
```

### 方案 B：对象池 + CQRS（模板配套）

```csharp
// 用框架对象池（docs/guides/object-pool.md）管理音效节点：
var sfx = sfxPool.Acquire(SfxKey.OreHit);
GetNode<Node>("%SfxContainer").AddChild(sfx);
sfx.PlayOneShot(clip);
// 播完归还（AudioStreamPlayer.Finished 信号 → CQRS 事件 → 池归还）
```

**音量关联**：播放节点 `VolumeDb` 由 `AudioBusMap` 路由（挂到对应总线的 AudioStreamPlayer 自动跟随总线音量）。

## 6. 常见坑

| 坑 | 说明 |
|---|---|
| **AudioBusMap 名字与 Godot 总线不一致** | 应用器找不到总线会报错/静默；先在 Godot Audio 布局建总线 |
| **音量范围** | 设置数据 0-1，Godot 用 dB——应用器内部 `LinearToDb` 转换，业务层保持 0-1 |
| **缓存 GodotAudioSettings** | `Apply<GodotAudioSettings>()` 需要系统可查；确保 settings 已注册 |
| **所有播放射到一条线** | 建议 Master/BGM/SFX/Ambience 分总线，便于单类调整 |

## 7. API 速查

| API | 说明 |
|---|---|
| `ChangeMaster/Bgm/SfxVolumeCommand(input)` | 改音量命令（0-1） |
| `AudioSettings.Master/Bgm/SfxVolume` | 音量数据（0-1，float） |
| `ISettingsSystem.Apply<TApplicator>()` | 触发应用器 |
| `GodotAudioSettings.ApplyAsync()` | 应用器：写总线音量 |
| `AudioBusMap.Master/Bgm/Sfx` | 总线映射（字符串） |
| `VolumeChangedEvent` | 音量变化事件 |

## 8. 源码阅读入口

| 路径 | 内容 |
|---|---|
| `scripts/cqrs/audio/` | 模板音频 CQRS 域（命令/输入/事件） |
| `GFramework.Godot/Setting/GodotAudioSettings.cs` | 应用器：设置 → 总线 |
| `GFramework.Godot/Setting/Data/AudioBusMap.cs` | 总线映射 |
| `GFramework.Game/Setting/SettingsSystem.cs` | 设置系统（Apply 管道） |
| `resource/bus/project_bus_layout.tres` | 模板 Godot 总线布局（Master/BGM/SFX） |