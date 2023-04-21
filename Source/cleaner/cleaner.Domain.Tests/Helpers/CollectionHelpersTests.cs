using cleaner.Domain.Helpers;

namespace cleaner.Domain.Tests.Helpers;

using System.Collections.Generic;
using NUnit.Framework;

[TestFixture]
public class CollectionHelpersTests
{
    [Test]
    public void IsNullOrEmpty_GivenNull_ReturnsTrue()
    {
        List<int>? collection = null;
        Assert.IsTrue(CollectionHelpers.IsNullOrEmpty(collection));
    }

    [Test]
    public void IsNullOrEmpty_GivenEmpty_ReturnsTrue()
    {
        var collection = new List<int>();
        Assert.IsTrue(CollectionHelpers.IsNullOrEmpty(collection));
    }

    [Test]
    public void IsNullOrEmpty_GivenNotEmpty_ReturnsFalse()
    {
        var collection = new List<int> { 1, 2, 3 };
        Assert.IsFalse(CollectionHelpers.IsNullOrEmpty(collection));
    }

    [Test]
    public void IsNullOrEmpty_GivenEmptyArray_ReturnsTrue()
    {
        var collection = new int[0];
        Assert.IsTrue(CollectionHelpers.IsNullOrEmpty(collection));
    }

    [Test]
    public void IsNullOrEmpty_GivenNotEmptyArray_ReturnsFalse()
    {
        var collection = new[] { 1, 2, 3 };
        Assert.IsFalse(CollectionHelpers.IsNullOrEmpty(collection));
    }
}
