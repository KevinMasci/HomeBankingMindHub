using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;

namespace HomeBankingMindHub.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public AccountDTO FindById(long id)
        {
            var account = _accountRepository.FindById(id);
            var accountDTO = new AccountDTO
            {
                Id = account.Id,
                Number = account.Number,
                CreationDate = account.CreationDate,
                Balance = account.Balance,
                Transactions = account.Transactions.Select(tr => new TransactionDTO
                {
                    Id = tr.Id,
                    Type = tr.Type.ToString(),
                    Amount = tr.Amount,
                    Description = tr.Description,
                    Date = tr.Date,
                }).ToList()
            };
            return accountDTO;
        }

        public IEnumerable<AccountDTO> GetAllAccounts()
        {
            var accounts = _accountRepository.GetAllAccounts();
            var accountsDTO = new List<AccountDTO>();

            foreach (Account account in accounts)
            {
                var newAccountDTO = new AccountDTO
                {
                    Id = account.Id,
                    Number = account.Number,
                    CreationDate = account.CreationDate,
                    Balance = account.Balance,
                    Transactions = account.Transactions.Select(tr => new TransactionDTO
                    {
                        Id = tr.Id,
                        Type = tr.Type.ToString(),
                        Amount = tr.Amount,
                        Description = tr.Description,
                        Date = tr.Date,
                    }).ToList()
                };

                accountsDTO.Add(newAccountDTO);
            }

            return accountsDTO;
        }

        public void Save(Account account)
        {
            _accountRepository.Save(account);
        }

        public IEnumerable<Account> GetAccountsByClient(long clientId)
        {
            return _accountRepository.GetAccountsByClient(clientId);
        }

        public Account GetAccountByNumber(string accNumber)
        {
            return _accountRepository.GetAccountByNumber(accNumber);
        }
    }
}
