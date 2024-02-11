using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;

namespace HomeBankingMindHub.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountResporitory;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountResporitory = accountRepository;
        }

        public Account FindById(long id)
        {
            return _accountResporitory.FindById(id);
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return _accountResporitory.GetAllAccounts();
        }

        public void Save(Account account)
        {
            _accountResporitory.Save(account);
        }

        public IEnumerable<Account> GetAccountsByClient(long clientId)
        {
            return _accountResporitory.GetAccountsByClient(clientId);
        }

        public Account GetAccountByNumber(string accNumber)
        {
            return _accountResporitory.GetAccountByNumber(accNumber);
        }
    }
}
