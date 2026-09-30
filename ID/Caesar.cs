using System;

namespace ID
{
    public static class Caesar
    {
        private const int Shift = 3;

        public static string Encode(string text)
        {
            if (text == null)
                return string.Empty;

            char[] arr = text.ToCharArray();

            for (int i = 0; i < arr.Length; i++)
            {
                char c = arr[i];

                if (c >= 'a' && c <= 'z')
                {
                    arr[i] = (char)('a' + (c - 'a' + Shift) % 26);
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    arr[i] = (char)('A' + (c - 'A' + Shift) % 26);
                }
            }

            return new string(arr);
        }

        public static string Decode(string text)
        {
            if (text == null)
                return string.Empty;

            char[] arr = text.ToCharArray();

            for (int i = 0; i < arr.Length; i++)
            {
                char c = arr[i];

                if (c >= 'a' && c <= 'z')
                {
                    arr[i] = (char)('a' + (c - 'a' - Shift + 26) % 26);
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    arr[i] = (char)('A' + (c - 'A' - Shift + 26) % 26);
                }
            }

            return new string(arr);
        }
    }
}