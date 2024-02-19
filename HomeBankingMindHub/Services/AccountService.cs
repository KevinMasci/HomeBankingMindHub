using HomeBankingMindHub.DTO;
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
            var accountDTO = new AccountDTO(account);
            return accountDTO;
        }

        public IEnumerable<AccountDTO> GetAllAccounts()
        {
            var accounts = _accountRepository.GetAllAccounts();
            var accountsDTO = accounts.Select(account => new AccountDTO(account));
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
