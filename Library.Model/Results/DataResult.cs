namespace Library.Model.Results;

public class DataResult<T> : Result, IDataResult<T>
{
    public T Data { get; }

    public DataResult(T data, bool success) : base(success)
    {
        Data = data;
    }

    public DataResult(T data, bool success, string message) : base(success, message)
    {
        Data = data;
    }
}

public class SuccessDataResult<T> : DataResult<T>
{
    public SuccessDataResult(T data) : base(data, true) { }
    public SuccessDataResult(T data, string message) : base(data, true, message) { }
    public SuccessDataResult(string message) : base(default!, true, message) { }
    public SuccessDataResult() : base(default!, true) { }
}

public class ErrorDataResult<T> : DataResult<T>
{
    public ErrorDataResult(T data) : base(data, false) { }
    public ErrorDataResult(T data, string message) : base(data, false, message) { }
    public ErrorDataResult(string message) : base(default!, false, message) { }
    public ErrorDataResult() : base(default!, false) { }
}