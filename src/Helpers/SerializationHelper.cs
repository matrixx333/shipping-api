using System.Text.Json;

public static class SerializationHelper
{
    public static string SerializeRequest(object payload)
    {
        if (payload == null) return string.Empty;
        return JsonSerializer.Serialize(payload);
    }
}