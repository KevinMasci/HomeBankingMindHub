using ClientService.DTO;
using ClientService.Models;
using ClientService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ClientService.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(IClientRepository clientRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        public void MakeTransfer(TransferDTO transferDTO)
        {
            Account fromAccount = _accountRepository.GetAccountByNumber(transferDTO.FromAccountNumber);
            Account toAccount = _accountRepository.GetAccountByNumber(transferDTO.ToAccountNumber);

            //comenzamos con la inserrción de las 2 transacciones realizadas
            //desde toAccount se debe generar un debito por lo tanto lo multiplicamos por -1
            _transactionRepository.Save(new Transaction
            {
                Type = TransactionType.DEBIT,
                Amount = transferDTO.Amount * -1,
                Description = transferDTO.Description + " " + toAccount.Number,
                AccountId = fromAccount.Id,
                Date = DateTime.Now,
            });

            //ahora una credito para la cuenta fromAccount
            _transactionRepository.Save(new Transaction
            {
                Type = TransactionType.CREDIT,
                Amount = transferDTO.Amount,
                Description = transferDTO.Description + " " + fromAccount.Number,
                AccountId = toAccount.Id,
                Date = DateTime.Now,
            });

            //seteamos los valores de las cuentas, a la ccuenta de origen le restamos el monto
            fromAccount.Balance = fromAccount.Balance - transferDTO.Amount;
            //actualizamos la cuenta de origen
            _accountRepository.Save(fromAccount);

            //a la cuenta de destino le sumamos el monto
            toAccount.Balance = toAccount.Balance + transferDTO.Amount;
            //actualizamos la cuenta de destino
            _accountRepository.Save(toAccount);
        }
    }
}
