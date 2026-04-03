using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cleaner.Domain.FileBasedRules.Rules;

public interface IRule
{
    string Id { get; }
    string Name { get; }
    string ShortDescription { get; }
    string LongDescription { get; }
    ValidationMessage[] Validate(string filePath, string fileContent, SyntaxTree tree, CompilationUnitSyntax root);
}