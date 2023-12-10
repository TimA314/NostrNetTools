namespace NostrNetTools.Utils
{
    public static class Bech32
    {
        private static readonly uint[] Generator = new uint[5] { 996825010u, 642813549u, 513874426u, 1027748829u, 705979059u };

        private const string Charset = "qpzry9x8gf2tvdw0s3jn54khce6mua7l";

        private static readonly short[] Icharset = new short[128]
        {
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, 15, -1,
        10, 17, 21, 20, 26, 30, 7, 5, -1, -1,
        -1, -1, -1, -1, -1, 29, -1, 24, 13, 25,
        9, 8, 23, -1, 18, 22, 31, 27, 19, -1,
        1, 0, 3, 16, 11, 28, 12, 14, 6, 4,
        2, -1, -1, -1, -1, -1, -1, 29, -1, 24,
        13, 25, 9, 8, 23, -1, 18, 22, 31, 27,
        19, -1, 1, 0, 3, 16, 11, 28, 12, 14,
        6, 4, 2, -1, -1, -1, -1, -1
        };

        public static uint PolyMod(byte[] values)
        {
            uint num = 1u;
            foreach (byte b in values)
            {
                uint num2 = num >> 25;
                num = ((num & 0x1FFFFFF) << 5) ^ b;
                for (int j = 0; j < 5; j++)
                {
                    if (((num2 >> j) & 1) == 1)
                    {
                        num ^= Generator[j];
                    }
                }
            }

            return num;
        }

        public static void Decode(string encoded, out string? hrp, out byte[]? data)
        {
            DecodeSquashed(encoded, out hrp, out var data2);
            if (data2 == null)
            {
                data = null;
            }
            else
            {
                data = Bytes5To8(data2);
            }
        }

        private static void DecodeSquashed(string? adr, out string? hrp, out byte[]? data)
        {
            adr = CheckAndFormat(adr);
            if (adr == null)
            {
                data = null;
                hrp = null;
                return;
            }

            int num = adr!.LastIndexOf("1", StringComparison.Ordinal);
            if (num == -1)
            {
                data = null;
                hrp = null;
                return;
            }

            hrp = adr!.Substring(0, num);
            byte[] array = StringToSquashedBytes(adr!.Substring(num + 1));
            if (array == null)
            {
                data = null;
                return;
            }

            if (!VerifyChecksum(hrp, array))
            {
                data = null;
                return;
            }

            int num2 = array.Length - 6;
            data = new byte[num2];
            Array.Copy(array, 0, data, 0, num2);
        }

        private static string? CheckAndFormat(string? adr)
        {
            if (adr == null)
            {
                return adr;
            }

            string text = adr!.ToLower();
            string text2 = adr!.ToUpper();
            if (adr != text && adr != text2)
            {
                return null;
            }

            return text;
        }

        private static bool VerifyChecksum(string hrp, byte[] data)
        {
            return PolyMod(HrpExpand(hrp).Concat(data).ToArray()) == 1;
        }

        private static byte[]? StringToSquashedBytes(string input)
        {
            byte[] array = new byte[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                short num = Icharset[(uint)c];
                if (num == -1)
                {
                    return null;
                }

                array[i] = (byte)num;
            }

            return array;
        }

        public static string? Encode(string hrp, byte[] data)
        {
            byte[] array = Bytes8To5(data);
            if (array == null)
            {
                return string.Empty;
            }

            return EncodeSquashed(hrp, array);
        }

        private static string? EncodeSquashed(string hrp, byte[] data)
        {
            byte[] second = CreateChecksum(hrp, data);
            string text = SquashedBytesToString(data.Concat(second).ToArray());
            if (text == null)
            {
                return null;
            }

            return hrp + "1" + text;
        }

        private static byte[] CreateChecksum(string hrp, byte[] data)
        {
            uint num = PolyMod(HrpExpand(hrp).Concat(data).ToArray().Concat(new byte[6])
                .ToArray()) ^ 1u;
            byte[] array = new byte[6];
            for (int i = 0; i < 6; i++)
            {
                array[i] = (byte)((num >> 5 * (5 - i)) & 0x1Fu);
            }

            return array;
        }

        private static byte[] HrpExpand(string input)
        {
            byte[] array = new byte[input.Length * 2 + 1];
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                array[i] = (byte)((int)c >> 5);
            }

            for (int j = 0; j < input.Length; j++)
            {
                char c2 = input[j];
                array[j + input.Length + 1] = (byte)(c2 & 0x1Fu);
            }

            return array;
        }

        private static string? SquashedBytesToString(byte[] input)
        {
            string text = string.Empty;
            foreach (byte b in input)
            {
                if ((b & 0xE0u) != 0)
                {
                    return null;
                }

                text += "qpzry9x8gf2tvdw0s3jn54khce6mua7l"[b];
            }

            return text;
        }

        private static byte[]? Bytes8To5(byte[] data)
        {
            return ByteSquasher(data, 8, 5);
        }

        private static byte[]? Bytes5To8(byte[] data)
        {
            return ByteSquasher(data, 5, 8);
        }

        private static byte[]? ByteSquasher(byte[] input, int inputWidth, int outputWidth)
        {
            int num = 0;
            int num2 = 0;
            List<byte> list = new List<byte>();
            int num3 = (1 << outputWidth) - 1;
            foreach (byte b in input)
            {
                if (b >> inputWidth != 0)
                {
                    return null;
                }

                num2 = (num2 << inputWidth) | b;
                num += inputWidth;
                while (num >= outputWidth)
                {
                    num -= outputWidth;
                    list.Add((byte)((num2 >> num) & num3));
                }
            }

            if (inputWidth == 8 && outputWidth == 5)
            {
                if (num != 0)
                {
                    list.Add((byte)((num2 << outputWidth - num) & num3));
                }
            }
            else if (num >= inputWidth || ((num2 << outputWidth - num) & num3) != 0)
            {
                return null;
            }

            return list.ToArray();
        }
    }
}
