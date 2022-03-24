using System;
namespace backend.Helpers
{
	public class PeselValidator
	{
		private static readonly int length = 11;
		private static readonly int[] coeff = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };

		public static bool Validate(string pesel)
		{
			// Basic validation
			if (pesel == null ||
				pesel.Length != PeselValidator.length ||
				!pesel.All(char.IsDigit))
				return false;

			// Check sum validation
			int sum = 0;
			for(int i = 0; i < PeselValidator.length - 1; i++)
            {
				sum += PeselValidator.coeff[i] * int.Parse(pesel[i].ToString());
            }

			int checkSum = sum % 10;
			if (checkSum > 0)
				checkSum = 10 - checkSum;

			if (checkSum != int.Parse(pesel[PeselValidator.length - 1].ToString()))
				return false;

			return true;
		}
	}
}

