using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using UrlShortener.Data;
using UrlShortener.Models;
using UrlShortener.Services;

namespace UrlShortener.Controllers;

[ApiController]
public class ShortenController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;

    public ShortenController(AppDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    [HttpPost("api/shorten")]
    public async Task<IActionResult> CreateShortUrl([FromBody] CreateUrlRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
            return BadRequest("Url is required.");

        var entity = new ShortUrl
        {
            OriginalUrl = request.Url,
            CreatedAt = DateTime.UtcNow
        };

        _db.ShortUrls.Add(entity);
        await _db.SaveChangesAsync();

        entity.ShortCode = Base62Encoder.Encode(entity.Id);
        await _db.SaveChangesAsync();

        var shortUrl = $"{Request.Scheme}://{Request.Host}/{entity.ShortCode}";
        return Ok(new { shortUrl });
    }

    [HttpGet("/{code}")]
    public async Task<IActionResult> RedirectToOriginal(string code)
    {
        if (_cache.TryGetValue(code, out string? cachedUrl))
        {
            return RedirectPermanent(cachedUrl!);
        }

        var entity = await _db.ShortUrls.FirstOrDefaultAsync(x => x.ShortCode == code);
        if (entity == null) return NotFound();

        entity.ClickCount++;
        await _db.SaveChangesAsync();

        _cache.Set(code, entity.OriginalUrl, TimeSpan.FromHours(24));

        return RedirectPermanent(entity.OriginalUrl);
    }
}