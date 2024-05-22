using cleaner.Domain.Helpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cleaner.Domain.Tests.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

public class RuleHelperTests
{
    [Test]
    public void GetLineNumber_ShouldReturnCorrectLineNumber()
    {
        string code = @"
                using System;

                namespace TestApp
                {
                    class Program
                    {
                        static void Main(string[] args)
                        {
                            Console.WriteLine(""Hello World!"");
                        }
                    }
                }
            ";

        SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
        var root = tree.GetCompilationUnitRoot();
        var mainMethod = root.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault();

        int lineNumber = RuleHelper.GetLineNumber(mainMethod);

        Assert.That(lineNumber, Is.EqualTo(8));
    }

    [Test]
    public void GetLineNumber_ShouldReturnMinusOne_WhenNodeIsNull()
    {
        int lineNumber = RuleHelper.GetLineNumber(null);

        Assert.That(lineNumber, Is.EqualTo(-1));
    }
}