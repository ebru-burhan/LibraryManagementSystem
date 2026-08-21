namespace Library.Model.Results;

public class Result : IResult
{
    public bool Success { get; }
    public string Message { get; }

    public Result(bool success)
    {
        Success = success;
        Message = string.Empty; // Null olmasını engelliyor, güvenli standart.
    }

    public Result(bool success, string message) : this(success)
    {
        Message = message ?? string.Empty; // Eğer dışarıdan null gelirse yine boş string'e çevirir.
        //"" tutar ama performansa etkisi devede kulak :))
    }
}


public class SuccessResult : Result
{
    public SuccessResult() : base(true) { }
    public SuccessResult(string message) : base(true, message) { }
}

public class ErrorResult : Result
{
    public ErrorResult() : base(false) { }
    public ErrorResult(string message) : base(false, message) { }
}