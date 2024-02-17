using HomeBankingMindHub.Data;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories
{
    public class CardRepository : RepositoryBase<Card>, ICardRepository
    {
        public CardRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public Card FindByNumber(string cardNumber)
        {
            return FindByCondition(card => card.Number == cardNumber).FirstOrDefault();
        }

        public void Save(Card card)
        {
            Create(card);
            SaveChanges();
        }
    }
}
