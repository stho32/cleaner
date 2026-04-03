using cleaner.Domain.FileBasedRules.Rules;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace cleaner.Domain.Tests.FileBasedRules.Rules
{
    [TestFixture]
    public class PublicPropertiesPrivateSettersRuleTests
    {
        private PublicPropertiesPrivateSettersRule _rule = null!;

        [SetUp]
        public void Setup()
        {
            _rule = new PublicPropertiesPrivateSettersRule();
        }

        [Test]
        public void Validate_PublicPropertyWithPrivateSetter_ShouldNotReturnWarning()
        {
            string code = @"
            using System;

            class TestClass
            {
                public int TestProperty { get; private set; }
            }";

            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var messages = _rule.Validate("TestFile.cs", code, tree, root);

            Assert.That(messages, Is.Empty);
        }

        [Test]
        public void Validate_PublicPropertyWithPublicSetter_ShouldReturnWarning()
        {
            string code = @"
            using System;

            class TestClass
            {
                public int TestProperty { get; set; }
            }";

            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var messages = _rule.Validate("TestFile.cs", code, tree, root);

            Assert.That(messages, Is.Not.Empty);
            Assert.That(messages.Length, Is.EqualTo(1));
            Assert.That(messages[0]?.RuleId, Is.EqualTo("PublicPropertiesPrivateSettersRule"));
        }
    }
}