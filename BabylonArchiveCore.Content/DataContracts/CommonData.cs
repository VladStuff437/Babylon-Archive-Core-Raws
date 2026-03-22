namespace BabylonArchiveCore.Content.DataContracts;

/// <summary>
/// 3D vector for scene content. Framework-independent.
/// </summary>
public sealed record Vec3Data(float X, float Y, float Z);

/// <summary>
/// 3D dimensions for zones and objects.
/// </summary>
public sealed record SizeData(float Width, float Height, float Depth);
