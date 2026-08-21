namespace Library.Model.Results;

public interface IDataResult<T> : IResult
{
    T Data { get; }
}