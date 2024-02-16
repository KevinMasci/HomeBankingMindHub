using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMindHub.Services
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

        public IActionResult MakeTransfer(TransferDTO transferDTO, string userEmail)
        {
            try
            {
                Client client = _clientRepository.FindByEmail(userEmail);

                if (client == null)
                {
                    return new ObjectResult("No existe el cliente") { StatusCode = 403 };
                }

                if (transferDTO.FromAccountNumber == string.Empty || transferDTO.ToAccountNumber == string.Empty)
                {
                    return new ObjectResult("Cuenta de origen o cuenta de destino no proporcionada.") { StatusCode = 403 };
                }

                if (transferDTO.FromAccountNumber == transferDTO.ToAccountNumber)
                {
                    return new ObjectResult("No se permite la transferencia a la misma cuenta.") { StatusCode = 403};
                }

                if (transferDTO.Amount <= 0 || transferDTO.Amount < 0.001)
                {
                    return new ObjectResult("Monto no valido.") { StatusCode = 403 };
                }

                if (transferDTO.Description == string.Empty)
                {
                    return new ObjectResult("Descripcion no proporcionada.") { StatusCode = 403 };
                }

                //buscamos las cuentas
                Account fromAccount = _accountRepository.GetAccountByNumber(transferDTO.FromAccountNumber);
                if (fromAccount == null)
                {
                    return new ObjectResult("Cuenta de origen no existe") { StatusCode = 403};
                }

                //controlamos el monto
                if (fromAccount.Balance < transferDTO.Amount)
                {
                    return new ObjectResult("Fondos insuficientes") { StatusCode = 403};
                }

                //buscamos la cuenta de destino
                Account toAccount = _accountRepository.GetAccountByNumber(transferDTO.ToAccountNumber);
                if (toAccount == null)
                {
                    return new ObjectResult("Cuenta de destino no existe") { StatusCode = 403};
                }

                //demas logica para guardado
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

                return new ObjectResult("Transferencia realizada con exito") { StatusCode = 201 };
            }
            catch (Exception ex)
            {
                return new ObjectResult($"Error interno del servidor: {ex.Message}") { StatusCode = 500 };
            }
        }
    }
}
