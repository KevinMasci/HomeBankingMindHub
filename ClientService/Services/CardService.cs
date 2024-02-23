using ClientService.Models;
using ClientService.Repositories;

namespace HomeBankingMindHub.Services
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;

        public CardService(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public Card FindByNumber(string cardNumber)
        {
            return _cardRepository.FindByNumber(cardNumber);
        }

        public void Save(Card card)
        {
            _cardRepository.Save(card);
        }
    }
}
