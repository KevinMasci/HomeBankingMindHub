using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface ICardService
    {
        Card FindByNumber(string cardNumber);
        void Save(Card card);
    }
}
