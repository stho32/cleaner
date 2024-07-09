using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace cleaner.Domain.FileBasedRules.Rules;

public class FileNameMatchingDeclarationRule : IRule
{
    public string Id => GetType().Name;
    public string Name => "File Name Matching Declaration Rule";
    public string ShortDescription => "Verify that the file name matches the declared type";
    public string LongDescription => "This rule checks if the file name matches the name of the declared type inside the file and raises a warning if they don't match.";

    public ValidationMessage[] Validate(string filePath, string fileContent)
    {
        var messages = new List<ValidationMessage>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
        var root = tree.GetCompilationUnitRoot();

        var declaredTypeSyntax = root.DescendantNodes()
            .OfType<TypeDeclarationSyntax>()
            .FirstOrDefault();

        if (declaredTypeSyntax != null)
        {
            string declaredTypeName = declaredTypeSyntax.Identifier.Text;
            string actualFileName = Path.GetFileName(filePath);

            // Define allowed file extensions
            var allowedExtensions = new[] { ".cs", ".aspx.cs", ".aspx.designer.cs", ".xaml.cs" };

            bool isValidFileName = allowedExtensions.Any(ext => 
                string.Equals(actualFileName, $"{declaredTypeName}{ext}", StringComparison.OrdinalIgnoreCase));

            if (!isValidFileName)
            {
                var expectedFileNames = string.Join(", ", allowedExtensions.Select(ext => $"'{declaredTypeName}{ext}'"));
                var message = new ValidationMessage(
                    Id,
                    Name,
                    $"The file '{filePath}' should be named one of the following: {expectedFileNames} to match the declared type '{declaredTypeName}'."
                );
                messages.Add(message);
            }
        }

        return messages.ToArray();
    }
}