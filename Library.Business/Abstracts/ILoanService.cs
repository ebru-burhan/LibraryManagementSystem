using Library.Model.Dtos.Operations;
using Library.Model.Results;

namespace Library.Business.Abstracts;

public interface ILoanService
{
    Task<IResult> CreateLoanAsync(CreateLoanDto createLoanDto);
}