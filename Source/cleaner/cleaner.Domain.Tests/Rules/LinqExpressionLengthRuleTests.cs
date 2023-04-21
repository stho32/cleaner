using cleaner.Domain.Rules;
using NUnit.Framework;

namespace cleaner.Domain.Tests.Rules;

[TestFixture]
public class LinqExpressionLengthRuleTests
{
    private LinqExpressionLengthRule? _rule;

    [SetUp]
    public void Setup()
    {
        _rule = new LinqExpressionLengthRule();
    }

    [Test]
    public void Validate_LinqExpressionWithTwoSteps_ShouldNotReturnWarning()
    {
        string code = @"
            using System;
            using System.Linq;
            using System.Collections.Generic;

            class Program
            {
                static void Main()
                {
                    List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
                    var evenNumbers = numbers.Where(number => number % 2 == 0)
                                             .Select(number => number);
                }
            }";

        var messages = _rule!.Validate("TestFile.cs", code);

        Assert.IsEmpty(messages);
    }

    [Test]
    public void Validate_LinqExpressionWithThreeSteps_ShouldReturnWarning()
    {
        string code = @"
            using System;
            using System.Linq;
            using System.Collections.Generic;

            class Program
            {
                static void Main()
                {
                    List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
                    var evenSquares = numbers.Where(number => number % 2 == 0)
                                             .Select(number => number * number)
                                             .OrderBy(square => square);
                }
            }";

        var messages = _rule!.Validate("TestFile.cs", code);

        Assert.IsNotEmpty(messages);
        Assert.AreEqual(1, messages.Length);
        Assert.AreEqual("LinqExpressionLengthRule", messages[0]?.RuleId);
    }
}