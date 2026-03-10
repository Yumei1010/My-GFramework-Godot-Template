using GFrameworkGodotTemplate.scripts.serializer;

namespace GFrameworkGodotTemplate.scripts.poker;

/// <summary>
/// 扑克数据序列化器，负责将扑克数据对象序列化为可存储或传输的格式，以及反序列化操作
/// </summary>
/// <remarks>
/// 该类实现了ISerializer接口，提供针对PokerData类型数据的序列化功能
/// </remarks>
public class PokerDataSerializer : ISerializer<PokerData>;