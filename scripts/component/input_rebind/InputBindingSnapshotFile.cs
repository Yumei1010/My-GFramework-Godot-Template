using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using GFramework.Game.Abstractions.Input;
using Godot;

namespace GFrameworkTemplate.scripts.component.input_rebind;

/// <summary>
///     输入绑定快照的序列化与文件读写（把改键结果持久化到 <c>user://</c>）。
/// </summary>
/// <remarks>
///     序列化用自己的 DTO 而非框架类型（框架类型只有只读属性，且枚举以字符串存储更利兼容）。
///     <see cref="Serialize" /> / <see cref="Deserialize" /> 不触文件系统，可单元测试。
/// </remarks>
public static class InputBindingSnapshotFile
{
    /// <summary>
    ///     序列化选项：缩进输出，便于人工排查存档。
    /// </summary>
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    /// <summary>
    ///     把绑定快照序列化为 JSON 文本。
    /// </summary>
    /// <param name="snapshot">绑定快照。</param>
    /// <returns>JSON 文本。</returns>
    public static string Serialize(InputBindingSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var dto = new SnapshotDto
        {
            Actions = snapshot.Actions.ToDictionary(
                static action => action.ActionName,
                static action => action.Bindings.Select(ToDto).ToList()),
        };

        return JsonSerializer.Serialize(dto, Options);
    }

    /// <summary>
    ///     从 JSON 文本还原绑定快照。
    /// </summary>
    /// <param name="json">JSON 文本。</param>
    /// <returns>绑定快照；文本为空或格式非法时返回 null。</returns>
    public static InputBindingSnapshot? Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        SnapshotDto? dto;
        try
        {
            dto = JsonSerializer.Deserialize<SnapshotDto>(json);
        }
        catch (JsonException)
        {
            return null;
        }

        if (dto?.Actions is null)
        {
            return null;
        }

        var actions = dto.Actions
            .Where(static pair => !string.IsNullOrWhiteSpace(pair.Key))
            .Select(pair => new InputActionBinding(
                pair.Key,
                pair.Value.Select(FromDto).Where(static b => b is not null).Select(static b => b!).ToList()))
            .ToList();

        return new InputBindingSnapshot(actions);
    }

    /// <summary>
    ///     把绑定快照写入文件（目录不存在时自动创建）。
    /// </summary>
    /// <param name="resPath">目标路径（如 <c>user://input_bindings.json</c>）。</param>
    /// <param name="snapshot">绑定快照。</param>
    /// <returns>写入成功返回 true。</returns>
    public static bool Save(string resPath, InputBindingSnapshot snapshot)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resPath);

        var absolutePath = ProjectSettings.GlobalizePath(resPath);
        var directory = System.IO.Path.GetDirectoryName(absolutePath);
        if (!string.IsNullOrEmpty(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
        }

        System.IO.File.WriteAllText(absolutePath, Serialize(snapshot));
        return true;
    }

    /// <summary>
    ///     从文件读取绑定快照。
    /// </summary>
    /// <param name="resPath">来源路径。</param>
    /// <returns>绑定快照；文件不存在或内容非法时返回 null。</returns>
    public static InputBindingSnapshot? Load(string resPath)
    {
        if (string.IsNullOrWhiteSpace(resPath))
        {
            return null;
        }

        var absolutePath = ProjectSettings.GlobalizePath(resPath);
        if (!System.IO.File.Exists(absolutePath))
        {
            return null;
        }

        return Deserialize(System.IO.File.ReadAllText(absolutePath));
    }

    /// <summary>
    ///     框架绑定描述 → 存档 DTO。
    /// </summary>
    /// <param name="binding">绑定描述。</param>
    /// <returns>DTO。</returns>
    private static BindingDto ToDto(InputBindingDescriptor binding)
    {
        return new BindingDto
        {
            Device = binding.DeviceKind.ToString(),
            Kind = binding.BindingKind.ToString(),
            Code = binding.Code,
            Name = binding.DisplayName,
            AxisDirection = binding.AxisDirection,
        };
    }

    /// <summary>
    ///     存档 DTO → 框架绑定描述（字段非法时返回 null）。
    /// </summary>
    /// <param name="dto">DTO。</param>
    /// <returns>绑定描述或 null。</returns>
    private static InputBindingDescriptor? FromDto(BindingDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Code) ||
            !Enum.TryParse<InputDeviceKind>(dto.Device, out var deviceKind) ||
            !Enum.TryParse<InputBindingKind>(dto.Kind, out var bindingKind))
        {
            return null;
        }

        return new InputBindingDescriptor(deviceKind, bindingKind, dto.Code, dto.Name ?? string.Empty, dto.AxisDirection);
    }

    /// <summary>
    ///     存档根对象。
    /// </summary>
    private sealed class SnapshotDto
    {
        /// <summary>动作名 → 绑定列表。</summary>
        public Dictionary<string, List<BindingDto>>? Actions { get; set; }
    }

    /// <summary>
    ///     单条绑定的存档结构。
    /// </summary>
    private sealed class BindingDto
    {
        /// <summary>设备类别（枚举名字符串）。</summary>
        public string Device { get; set; } = string.Empty;

        /// <summary>绑定种类（枚举名字符串）。</summary>
        public string Kind { get; set; } = string.Empty;

        /// <summary>绑定 Code。</summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>显示名。</summary>
        public string? Name { get; set; }

        /// <summary>轴方向（仅轴绑定有值）。</summary>
        public float? AxisDirection { get; set; }
    }
}
