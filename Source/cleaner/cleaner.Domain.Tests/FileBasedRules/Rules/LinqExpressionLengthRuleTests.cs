using cleaner.Domain.FileBasedRules.Rules;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace cleaner.Domain.Tests.FileBasedRules.Rules
{
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

            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var messages = _rule!.Validate("TestFile.cs", code, tree, root);

            Assert.That(messages, Is.Empty);
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

            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var messages = _rule!.Validate("TestFile.cs", code, tree, root);

            Assert.That(messages, Is.Not.Empty);
            Assert.That(messages.Length, Is.EqualTo(1));
            Assert.That(messages[0]?.RuleId, Is.EqualTo("LinqExpressionLengthRule"));
        }
    }
}