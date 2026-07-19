using FluentAssertions;
using NUnit.Framework;

namespace Tests.Helpers;

[TestFixture]
[Category("UnitTest")]
public class SerializationHelperTests
{
    [Test]
    public void SerializeRequest_WhenPayloadIsNull_ReturnsEmptyString()
    {
        // Arrange
        object? payload = null;

        // Act
        var result = SerializationHelper.SerializeRequest(payload!);

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public void SerializeRequest_WhenPayloadIsNotNull_ReturnsSerializedJson()
    {
        // Arrange
        var payload = new { Name = "UPS", Id = 1 };

        // Act
        var result = SerializationHelper.SerializeRequest(payload);

        // Assert
        result.Should().Be("{\"Name\":\"UPS\",\"Id\":1}");
    }
}
