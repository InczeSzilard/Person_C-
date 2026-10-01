using ID;
using System.Text;

public class Caesar : ICaesar
{
    private const int SHIFT = 3;

    public string Encode(string text)
    {
        StringBuilder result = new StringBuilder();

        foreach (char original in text)
        {
            char c = original;

            // Encode lowercase letters.
            if (c >= 'a' && c <= 'z')
            {
                c = (char)(
                    'a' +
                    (c - 'a' + SHIFT) % 26
                );
            }

            // Encode uppercase letters.
            else if (c >= 'A' && c <= 'Z')
            {
                c = (char)(
                    'A' +
                    (c - 'A' + SHIFT) % 26
                );
            }

            // Add the encoded character to the result.
            result.Append(c);
        }

        return result.ToString();
    }

    public string Decode(string text)
    {
        StringBuilder result = new StringBuilder();

        foreach (char original in text)
        {
            char c = original;

            // Decode lowercase letters.
            if (c >= 'a' && c <= 'z')
            {
                c = (char)(
                    'a' +
                    (c - 'a' - SHIFT + 26) % 26
                );
            }

            // Decode uppercase letters.
            else if (c >= 'A' && c <= 'Z')
            {
                c = (char)(
                    'A' +
                    (c - 'A' - SHIFT + 26) % 26
                );
            }

            // Add the decoded character to the result.
            result.Append(c);
        }

        return result.ToString();
    }
}