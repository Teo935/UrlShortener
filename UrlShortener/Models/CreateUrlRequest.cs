namespace UrlShortener.Models;

public class CreateUrlRequest // DTO (Data Transfer Object) per la richiesta di creazione di un URL corto
{
    public string Url { get; set; } = string.Empty;
}