/// <summary>
/// Represents an interface for serializable requests.
/// </summary>
public interface ISerializableRequest
{
    /// <summary>
    /// Serializes the request into a string representation.
    /// </summary>
    /// <returns>A string representation of the serialized request.</returns>
    string SerializeRequest();
}
