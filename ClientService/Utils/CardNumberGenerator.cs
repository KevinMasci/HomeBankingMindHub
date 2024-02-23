namespace ClientService.Utils
{
    public static class CardNumberGenerator
    {
        public static string GenerateCardNumber()
        {
            Random random = new Random();
            long min = 1000000000000000;
            long max = 9999999999999999;

            long randomNumber = (long)(random.NextDouble() * (max - min) + min);

            // Formatear el número con guiones cada 4 dígitos
            string formattedNumber = randomNumber.ToString("####-####-####-####");

            return formattedNumber;
        }

        public static int GenerateCvv()
        {
            Random random = new Random();
            return random.Next(001, 999);
        }
    }
}
