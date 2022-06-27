using System.Security.Cryptography;

namespace Timesheet.SharedKernel;

public static class RandomHelper
{
    public static string GetRandomNumeric(int length)
    {
        Random random = new Random();

        string result = string.Empty;

        for (int i = 0; i < length; i++)
        {
            result += random.Next(0, 9);
        }

        return result.ToString();
    }

    public static string GetSecureRandomDigitString(int length)
    {
        using var generator = RandomNumberGenerator.Create();

        const string chars = "1234567890";

        byte[] data = new byte[length];

        // If chars.Length isn't a power of 2 then there is a bias if we simply use the modulus operator. The first characters of chars will be more probable than the last ones.
        // buffer used if we encounter an unusable random byte. We will regenerate it in this buffer
        byte[]? buffer = null;

        // Maximum random number that can be used without introducing a bias
        int maxRandom = byte.MaxValue - ((byte.MaxValue + 1) % chars.Length);

        generator.GetBytes(data);

        char[] result = new char[length];

        for (int i = 0; i < length; i++)
        {
            byte value = data[i];

            while (value > maxRandom)
            {
                if (buffer == null)
                    buffer = new byte[1];

                generator.GetBytes(buffer);
                value = buffer[0];
            }

            result[i] = chars[value % chars.Length];
        }

        return new string(result);
    }

    public static string GetSecureRandomString(int length)
    {
        using var generator = RandomNumberGenerator.Create();

        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        byte[] data = new byte[length];

        // If chars.Length isn't a power of 2 then there is a bias if we simply use the modulus operator. The first characters of chars will be more probable than the last ones.
        // buffer used if we encounter an unusable random byte. We will regenerate it in this buffer
        byte[]? buffer = null;

        // Maximum random number that can be used without introducing a bias
        int maxRandom = byte.MaxValue - ((byte.MaxValue + 1) % chars.Length);

        generator.GetBytes(data);

        char[] result = new char[length];

        for (int i = 0; i < length; i++)
        {
            byte value = data[i];

            while (value > maxRandom)
            {
                if (buffer == null)
                {
                    buffer = new byte[1];
                }

                generator.GetBytes(buffer);
                value = buffer[0];
            }

            result[i] = chars[value % chars.Length];
        }

        return new string(result);
    }
}