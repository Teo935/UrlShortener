using System.Text;

namespace UrlShortener.Services;

public static class Base62Encoder
{
    private const string Alphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    /// <summary>
    /// restituisce il codice Base62 dato un ID numerico 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string Encode(long id)
    {
        if (id == 0) return Alphabet[0].ToString();

        var sb = new StringBuilder();
        while (id > 0)
        {
            sb.Insert(0, Alphabet[(int)(id % 62)]);
            id /= 62;
        }
        return sb.ToString();
    }
    /// <summary>
    /// Restituisce l'ID numerico dato il codice Base62
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public static long Decode(string code)
    {
        long id = 0;
        foreach (char c in code)
        {
            id = id * 62 + Alphabet.IndexOf(c);
        }
        return id;
    }
}