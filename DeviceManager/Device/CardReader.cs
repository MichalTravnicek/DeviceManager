using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace DeviceManager
{
    public class CardReader : Device
    {
        public override string Type { get; init; } = "CardReader";        

        [SetsRequiredMembers]
        public CardReader(string name, string accessCardNumber) : base(name)
        {
            AccessCardNumber = accessCardNumber;
        }

        [Logged(1)]
        [EditableProperty]
        public string AccessCardNumber
        {
            get;
            set
            {
                ValidateAccessCardNumber(value);
                SetProperty(ref field, ReverseBytesAndPad(value));
            }
        }

        private void ValidateAccessCardNumber(string value)
        {
            if (value.Length % 2 != 0 || value.Length > 16 || !Regex.IsMatch(value, "^[0-9a-fA-F]*$"))
            {
                throw new InvalidCardNumberException("Invalid card number format: " + value);
            }
        }

        private static string ReverseBytesAndPad(string hex)
        {
            // Přidáme na konec nuly, pokud je délka řetězce menší než 16 znaků.
            while (hex.Length < 16)
            {
                hex += "0";
            }

            var sb = new StringBuilder();
            for (int i = hex.Length - 2; i >= 0; i -= 2)
            {
                sb.Append(hex.Substring(i, 2));
            }
            return sb.ToString();
        }
    }
}
