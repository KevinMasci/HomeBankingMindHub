using HomeBankingMindHub.DTO;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface IAccountService
    {
        IEnumerable<AccountDTO> GetAllAccounts();
        AccountDTO FindById(long id);
        void Save(Account account);
        IEnumerable<Account> GetAccountsByClient(long clientId);
        Account GetAccountByNumber(string accNumber);
    }
}
