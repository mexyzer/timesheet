using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace Timesheet.SharedKernel;

public static class StringExtension
{
    /// <summary>
    /// Create new hash string using SHA256
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string ToSHA256(this string s)
    {
        using (SHA256 shaManager = SHA256.Create())
        {
            string hash = string.Empty;
            byte[] bytes = shaManager.ComputeHash(Encoding.UTF8.GetBytes(s), 0, Encoding.UTF8.GetByteCount(s));
            foreach (byte b in bytes)
            {
                hash += b.ToString("x2");
            }

            return hash;
        }
    }

    public static string ToHMACSHA256(this string s)
    {
        using (HMACSHA256 sha256 = new HMACSHA256())
        {
            string hash = string.Empty;
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(s), 0, Encoding.UTF8.GetByteCount(s));
            foreach (byte b in bytes)
            {
                hash += b.ToString("x2");
            }

            return hash;
        }
    }

    /// <summary>
    /// Create new hash string using SHA512
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string ToSHA512(this string s)
    {
        using (SHA512 shaManager = SHA512.Create())
        {
            string hash = string.Empty;
            byte[] bytes = shaManager.ComputeHash(Encoding.UTF8.GetBytes(s), 0, Encoding.UTF8.GetByteCount(s));
            foreach (byte b in bytes)
            {
                hash += b.ToString("x2");
            }

            return hash;
        }
    }

    /// <summary>
    /// Create new hash string using MD5
    /// </summary>
    /// <remarks>
    /// Resource from https://stackoverflow.com/questions/11454004/calculate-a-md5-hash-from-a-string
    /// </remarks>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string ToMD5(this string s)
    {
        // Use input string to calculate MD5 hash
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(s);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }

    public static bool IsEmailAddress(this string s)
    {
        try
        {
            MailAddress addr = new MailAddress(s);
            return addr.Address == s;
        }
        catch
        {
            return false;
        }
    }

    public static string ToBase64String(this string s)
    {
        var bytes = Encoding.UTF8.GetBytes(s);

        return Convert.ToBase64String(bytes);
    }

    public static string FromBase64String(this string s)
    {
        var bytes = Convert.FromBase64String(s);

        return Encoding.UTF8.GetString(bytes);
    }
}