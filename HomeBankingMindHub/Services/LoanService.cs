using HomeBankingMindHub.DTO;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMindHub.Services
{
    public class LoanService : ILoanService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IClientLoanRepository _clientLoanRepository;
        private readonly ITransactionRepository _transactionRepository;

        public LoanService(IClientRepository clientRepository, IAccountRepository accountRepository, IClientLoanRepository clientLoanRepository,
                           ILoanRepository loanRepository, ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _loanRepository = loanRepository;
            _clientLoanRepository = clientLoanRepository;
            _transactionRepository = transactionRepository;
        }

        public Loan FindById(long id)
        { 
            return _loanRepository.FindById(id);
        }

        public async Task<IActionResult> GetAll()
        {
            try
            {
                // Obtener todos los préstamos
                var loans = _loanRepository.GetAll();

                // Verificar si hay préstamos disponibles
                if (loans == null || !loans.Any())
                {
                    return new ObjectResult("No loans available") { StatusCode = 404 };
                }

                // Mapear los préstamos a DTO para enviar al cliente
                var loanDTOs = loans.Select(loan => new LoanDTO
                {
                    Id = loan.Id,
                    Name = loan.Name,
                    MaxAmount = loan.MaxAmount,
                    Payments = loan.Payments
                }).ToList();

                return new ObjectResult(loanDTOs) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new ObjectResult($"Internal server error: {ex.Message}") { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> RequestLoan(LoanApplicationDTO loanAppDto, string userEmail)
        {
            try
            {
                // Validar que el usuario esté autenticado
                if (string.IsNullOrEmpty(userEmail))
                {
                    return new ObjectResult("Not authenticated") { StatusCode = 403 };
                }

                // Verificar que el préstamo exista
                var loan = _loanRepository.FindById(loanAppDto.LoanId);
                if (loan == null)
                {
                    return new ObjectResult("Loan does not exist") { StatusCode = 404 };
                }

                // Verificar que el monto no sea 0 y no sobrepase el máximo autorizado
                if (loanAppDto.Amount <= 0 || loanAppDto.Amount > loan.MaxAmount)
                {
                    return new ObjectResult("Invalid loan amount") { StatusCode = 400 };
                }

                // Verificar que los payments no lleguen vacíos
                if (string.IsNullOrEmpty(loanAppDto.Payments))
                {
                    return new ObjectResult("Payments information is required") { StatusCode = 400 };
                }

                // Verificar que exista la cuenta de destino
                var account = _accountRepository.GetAccountByNumber(loanAppDto.ToAccountNumber);
                if (account == null)
                {
                    return new ObjectResult("Destination account does not exist") { StatusCode = 404 };
                }

                // Verificar que la cuenta de destino pertenezca al Cliente autentificado
                var client = _clientRepository.FindByEmail(userEmail);
                if (account.ClientId != client.Id)
                {
                    return new ObjectResult("Destination account does not belong to the authenticated client.") { StatusCode = 403 };
                }

                var clientLoan = new ClientLoan
                {
                    ClientId = client.Id,
                    LoanId = loan.Id,
                    Payments = loanAppDto.Payments,
                    Amount = loanAppDto.Amount * 1.2,
                };

                _transactionRepository.Save(new Transaction
                {
                    Type = TransactionType.CREDIT,
                    Amount = clientLoan.Amount,
                    Description = $"{loan.Name} loan approved",
                    AccountId = account.Id,
                    Date = DateTime.Now,
                });

                account.Balance += loanAppDto.Amount;
                _accountRepository.Save(account);

                _clientLoanRepository.Save(clientLoan);

                return new ObjectResult("Loan request processed successfully.") { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new ObjectResult($"Internal server error: {ex.Message}") { StatusCode = 500 };
            }
        }
    }
}
