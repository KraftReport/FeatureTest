using StreamingServerApi.Controllers;

namespace ut.Tests;

public class UnitTest1
{
    [Fact]
    public void AddTest()
    {
        var service = new Service();
        var result = service.Add(1);
        Assert.Equal(2, result);
    }

    [Fact]
    public void SubstractTest()
    {
        var service = new Service();
        var result = service.Subtract(2);
        Assert.Equal(0, result);
    }

    [Fact]
    public void MultiplyTest()
    {
        var service = new Service();
        var result = service.Multiply(3);
        Assert.Equal(9, result);
    }

    [Fact]
    public void DivideTest()
    {
        var service = new Service();
        var result = service.Divide(4);
        Assert.Equal(1, result);
    }

    [Fact]
    public void ModulusTest()
    {
        var service = new Service();
        var result = service.Modulus(5);
        Assert.Equal(0, result);
    }
}
