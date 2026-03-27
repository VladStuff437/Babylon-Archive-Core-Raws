using System.Globalization;
using System.Text.Json;

namespace BabylonArchiveCore.Runtime.Balance;

/// <summary>
/// Runtime loader таблиц баланса из JSON.
/// </summary>
public sealed class BalanceTableLoader
{
    public BalanceTable LoadFromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("Balance json cannot be null or empty.", nameof(json));
        }

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        var result = new BalanceTable
        {
            ProfileId = root.TryGetProperty("profileId", out var profileId)
                ? profileId.GetString() ?? "unknown"
                : "unknown"
        };

        PopulateScalars(result.Scalars, root, "damage", "damage");
        PopulateScalars(result.Scalars, root, "damageMultipliers", "damageMultipliers");
        PopulateScalars(result.Scalars, root, "economy", "economy");
        PopulateScalars(result.Scalars, root, "loot", "loot");
        PopulateScalars(result.Scalars, root, "factionModifiers", "factionModifiers");
        PopulateScalars(result.Scalars, root, "worldAxes", "worldAxes");
        PopulateScalars(result.Scalars, root, "mission", "mission");
        PopulateWeights(result.Weights, root, "lootRarityWeights");

        return result;
    }

    private static void PopulateScalars(Dictionary<string, float> target, JsonElement root, string propertyName, string prefix)
    {
        if (!root.TryGetProperty(propertyName, out var section) || section.ValueKind != JsonValueKind.Object)
        {
            return;
        }

        foreach (var item in section.EnumerateObject())
        {
            if (TryGetSingle(item.Value, out var value))
            {
                target[$"{prefix}.{item.Name}"] = value;
            }
        }
    }

    private static void PopulateWeights(Dictionary<string, int> target, JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var section) || section.ValueKind != JsonValueKind.Object)
        {
            return;
        }

        foreach (var item in section.EnumerateObject())
        {
            if (item.Value.ValueKind == JsonValueKind.Number && item.Value.TryGetInt32(out var value))
            {
                target[item.Name] = value;
            }
        }
    }

    private static bool TryGetSingle(JsonElement element, out float value)
    {
        value = default;

        if (element.ValueKind == JsonValueKind.Number && element.TryGetSingle(out value))
        {
            return true;
        }

        if (element.ValueKind == JsonValueKind.String)
        {
            return float.TryParse(element.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        return false;
    }
}

public sealed class BalanceTable
{
    public string ProfileId { get; init; } = "unknown";

    public Dictionary<string, float> Scalars { get; init; } = new(StringComparer.Ordinal);

    public Dictionary<string, int> Weights { get; init; } = new(StringComparer.Ordinal);

    public float GetScalar(string key, float fallback = 1f)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        return Scalars.TryGetValue(key, out var value) ? value : fallback;
    }

    public int GetWeight(string key, int fallback = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        return Weights.TryGetValue(key, out var value) ? value : fallback;
    }
}
