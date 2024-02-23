using ClientService.Models;

namespace ClientService.Services
{
    public interface ICardService
    {
        Card FindByNumber(string cardNumber);
        void Save(Card card);
    }
}
