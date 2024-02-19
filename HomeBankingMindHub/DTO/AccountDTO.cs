using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.DTO
{
    public class AccountDTO
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public DateTime CreationDate { get; set; }
        public double Balance { get; set; }
        public ICollection<TransactionDTO> Transactions { get; set; }

        public AccountDTO(Account account) 
        { 
            Id = account.Id;
            Number = account.Number;
            CreationDate = account.CreationDate;
            Balance = account.Balance;

            // Verificar si la colección de transacciones es nula
            Transactions = account.Transactions?.Select(tr => new TransactionDTO(tr)).ToList();
        }
    }
}
