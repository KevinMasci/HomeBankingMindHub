using ClientService.Models;

namespace ClientService.Repositories
{
    public interface ICardRepository
    {
        Card FindByNumber(string cardNumber);
        void Save(Card card);
    }
}
