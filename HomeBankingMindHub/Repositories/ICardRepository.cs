using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories
{
    public interface ICardRepository
    {
        Card FindByNumber(string cardNumber);
        void Save(Card card);
    }
}
