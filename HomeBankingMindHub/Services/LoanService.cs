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

        public IEnumerable<LoanDTO> GetAll()
        {
            var loans = _loanRepository.GetAll();

            var loanDTOs = loans.Select(loan => new LoanDTO
            {
                Id = loan.Id,
                Name = loan.Name,
                MaxAmount = loan.MaxAmount,
                Payments = loan.Payments
            }).ToList();

            return loanDTOs;
        }

        public ClientLoanDTO RequestLoan(LoanApplicationDTO loanAppDto, string userEmail)
        {
            // Verificar que el préstamo exista
            var loan = _loanRepository.FindById(loanAppDto.LoanId);
            if (loan == null)
            {
                throw new InvalidOperationException("Loan does not exist");
            }

            // Verificar que el monto no sea 0 y no sobrepase el máximo autorizado
            if (loanAppDto.Amount <= 0 || loanAppDto.Amount > loan.MaxAmount)
            {
                throw new InvalidOperationException("Invalid loan amount");
            }

            // Verificar que los payments no lleguen vacíos
            if (string.IsNullOrEmpty(loanAppDto.Payments))
            {
                throw new InvalidOperationException("Payments information is required");
            }

            // Verificar que exista la cuenta de destino
            var account = _accountRepository.GetAccountByNumber(loanAppDto.ToAccountNumber);
            if (account == null)
            {
                throw new InvalidOperationException("Destination account does not exist");
            }

            // Verificar que la cuenta de destino pertenezca al Cliente autentificado
            var client = _clientRepository.FindByEmail(userEmail);
            if (account.ClientId != client.Id)
            {
                throw new InvalidOperationException("Destination account does not belong to the authenticated client.");
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

            var clientLoanDTO = new ClientLoanDTO
            {
                Id = client.Id,
                LoanId = loan.Id,
                Name = loan.Name,
                Amount = clientLoan.Amount,
                Payments = int.Parse(loanAppDto.Payments),
            };

            return clientLoanDTO;
        }
    }
}
