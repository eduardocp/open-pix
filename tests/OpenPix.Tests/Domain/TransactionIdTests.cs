using OpenPix.Core.Domain;
using Xunit;

namespace OpenPix.Tests.Domain;

public class TransactionIdTests
{
    [Fact]
    public void Constructor_Should_Accept_Valid_Id()
    {
        var id = new TransactionId("TX123");
        Assert.Equal("TX123", id.Value);
    }

    [Fact]
    public void Constructor_Should_Accept_Asterisk()
    {
        var id = new TransactionId("***");
        Assert.Equal("***", id.Value);
    }
    
    [Fact]
    public void Constructor_Should_Default_Null_Or_Empty_To_Asterisk()
    {
        Assert.Equal("***", new TransactionId(null).Value);
        Assert.Equal("***", new TransactionId("").Value);
    }

    [Fact]
    public void Constructor_Should_Throw_If_Too_Long()
    {
        var longId = new string('a', 26);
        Assert.Throws<ArgumentException>(() => new TransactionId(longId));
    }

    [Fact]
    public void Constructor_Should_Throw_If_Invalid_Characters()
    {
        Assert.Throws<ArgumentException>(() => new TransactionId("invalid-char"));
        Assert.Throws<ArgumentException>(() => new TransactionId("spaced id"));
    }
}
