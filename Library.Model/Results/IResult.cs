namespace Library.Model.Results;

public interface IResult
{
    bool Success { get; }
    string Message { get; }
}