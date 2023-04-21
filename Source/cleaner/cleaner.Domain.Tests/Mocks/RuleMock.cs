using cleaner.Domain.Rules;

namespace cleaner.Domain.Tests.Mocks;

public class RuleMock : IRule
{
    private readonly ValidationMessage? _message;
    private readonly string _id;
    private readonly bool _returnMessage;

    public RuleMock(string id, bool returnMessage, ValidationMessage? message = null)
    {
        _id = id;
        _returnMessage = returnMessage;
        _message = message;
    }

    public string Id => _id;
    public string Name => "Mock Rule";
    public string ShortDescription => "A mock rule for testing.";
    public string LongDescription => "This is a mock rule implementation for testing purposes. It can be configured to return a given message or not.";

    public ValidationMessage[] Validate(string filePath, string fileContent)
    {
        return (_returnMessage ? new[] { _message } : Array.Empty<ValidationMessage>())!;
    }
}