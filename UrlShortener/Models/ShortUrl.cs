namespace UrlShortener.Models;

public class ShortUrl
{
    public long Id { get; set; } // usato per generare il codice Base62
    public string OriginalUrl { get; set; } = string.Empty;
    public string ShortCode { get; set; } = string.Empty;  // salvato per query veloci
    public DateTime CreatedAt { get; set; }
    public int ClickCount { get; set; } // opzionale, per analytics
}