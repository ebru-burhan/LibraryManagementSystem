using Library.Model.Dtos.Operations;
using Library.Model.Results;

namespace Library.Business.Abstracts;

public interface ILoanService
{
    Task<IResult> CreateLoanAsync(CreateLoanDto createLoanDto);
    Task<IResult> ReturnLoanAsync(Guid loanExternalId);

    Task<IDataResult<List<LoanListDto>>> GetActiveLoansAsync();
}