using GFramework.Core.Abstractions.Registries;
using Godot;

namespace GFrameworkTemplate.scripts.framework.registry;

/// <summary>
///     Godot 纹理注册表：按字符串键注册与查找 <see cref="Texture" /> 资源。
/// </summary>
/// <remarks>
///     键比较使用 <see cref="StringComparer.Ordinal" />（区分大小写、按序比较），行为一致且高效。
/// </remarks>
public class GodotTextureRegistry() : KeyValueRegistryBase<string, Texture>(StringComparer.Ordinal), IGodotTextureRegistry;
