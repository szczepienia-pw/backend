using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace backend.Helpers
{
	public class Validator
	{
		private static readonly int peselLength = 11;
		private static readonly int[] peselCoeff = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
		private static string emailPattern = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$";
		private static Regex emailRegex = new Regex(emailPattern, RegexOptions.IgnoreCase);

		[ExcludeFromCodeCoverage]
		public static bool ValidatePesel(string pesel)
		{
			// Basic validation
			if (pesel == null ||
				pesel.Length != Validator.peselLength ||
				!pesel.All(char.IsDigit))
				return false;

			// Check sum validation
			int sum = 0;
			for (int i = 0; i < Validator.peselLength - 1; i++)
            {
				sum += Validator.peselCoeff[i] * int.Parse(pesel[i].ToString());
            }

			int checkSum = sum % 10;
			if (checkSum > 0)
				checkSum = 10 - checkSum;

			if (checkSum != int.Parse(pesel[Validator.peselLength - 1].ToString()))
				return false;

			return true;
		}

		[ExcludeFromCodeCoverage]
		public static bool ValidateEmail(string email)
        {
			return Validator.emailRegex.IsMatch(email);
        }
	}
}

