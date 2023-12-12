using System.Text;

namespace NostrNetTools.Utils
{
    public static class KeyUtils
    {
        public static byte[] ToByteArray(string hex)
        {
            if (hex.Length % 2 == 1)
            {
                throw new Exception("The binary key cannot have an odd number of digits");
            }

            byte[] array = new byte[hex.Length >> 1];
            for (int i = 0; i < hex.Length >> 1; i++)
            {
                array[i] = (byte)((GetHexValue(hex[i << 1]) << 4) + GetHexValue(hex[(i << 1) + 1]));
            }

            return array;
        }

        public static int GetHexValue(char hex)
        {
            return hex - ((hex < ':') ? 48 : ((hex < 'a') ? 55 : 87));
        }

        public static string ToHexFromBytes(this byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            StringBuilder stringBuilder = new();
            foreach (byte b in bytes)
            {
                stringBuilder.Append(b.ToString("x2"));
            }

            return stringBuilder.ToString();
        }
    }
}
