using backend.Application.Common.Interfaces;
using backend.Application.Common.Interfaces.AI;
using backend.Application.Movies.Queries.GetMovies;
using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace backend.Application.Movies.Queries.SearchMoviesByVector;

public sealed record SearchMoviesByVectorQuery(string QueryText, int Limit = 5) : IRequest<IReadOnlyList<MovieBriefDto>>;

public sealed class SearchMoviesByVectorQueryHandler(
    IApplicationDbContext db, 
    IEmbeddingProvider embeddingProvider)
    : IRequestHandler<SearchMoviesByVectorQuery, IReadOnlyList<MovieBriefDto>>
{
    public async Task<IReadOnlyList<MovieBriefDto>> Handle(SearchMoviesByVectorQuery request, CancellationToken ct)
    {
        var queryText = request.QueryText.Trim();
        if (string.IsNullOrWhiteSpace(queryText))
        {
            return Array.Empty<MovieBriefDto>();
        }

        var limit = Math.Clamp(request.Limit, 1, 20);
        var detectedGenres = DetectGenres(queryText);
        if (detectedGenres.Count > 0)
        {
            return await SearchByGenres(detectedGenres, limit, ct);
        }

        var keywordResults = await SearchByKeyword(queryText, limit, ct);
        if (keywordResults.Count > 0)
        {
            return keywordResults;
        }

        var meaningfulTerms = ExtractMeaningfulTerms(queryText).Take(5).ToList();
        if (IsLikelySpecificUnmatchedLookup(queryText, meaningfulTerms))
        {
            return Array.Empty<MovieBriefDto>();
        }

        var hasEmbeddings = await db.Movies
            .AsNoTracking()
            .AnyAsync(m => m.Embedding != null, ct);

        if (!hasEmbeddings)
        {
            return await SearchPopular(limit, ct);
        }

        var vectorArray = await embeddingProvider.GenerateEmbeddingAsync(queryText, ct);
        var expectedDimensions = embeddingProvider.GetEmbeddingDimension();
        if (vectorArray.Length != expectedDimensions)
        {
            throw new InvalidOperationException(
                $"Search embedding dimension mismatch. Expected {expectedDimensions}, got {vectorArray.Length}.");
        }

        var queryVector = new Vector(vectorArray);

        var movies = await db.Movies
            .AsNoTracking()
            .Where(m => m.Embedding != null)
            .OrderBy(m => m.Embedding!.CosineDistance(queryVector))
            .Take(limit)
            .Select(x => new MovieBriefDto(
                x.Id,
                x.Title,
                x.Summary,
                x.Year,
                x.Rating,
                x.TrailerUrl,
                x.PosterUrl,
                x.MovieType))
            .ToListAsync(ct);

        return movies.Count > 0 ? movies : await SearchPopular(limit, ct);
    }

    private Task<List<MovieBriefDto>> SearchByGenres(IReadOnlyCollection<string> genres, int limit, CancellationToken ct)
    {
        return db.Movies
            .AsNoTracking()
            .Where(m => m.MovieGenres.Any(mg => genres.Contains(mg.Genre.Name)))
            .OrderByDescending(m => m.Rating ?? 0)
            .ThenByDescending(m => m.Year)
            .Take(limit)
            .Select(x => new MovieBriefDto(
                x.Id,
                x.Title,
                x.Summary,
                x.Year,
                x.Rating,
                x.TrailerUrl,
                x.PosterUrl,
                x.MovieType))
            .ToListAsync(ct);
    }

    private async Task<List<MovieBriefDto>> SearchByKeyword(string queryText, int limit, CancellationToken ct)
    {
        var loweredQuery = queryText.ToLower();
        var directMatches = await db.Movies
            .AsNoTracking()
            .Where(m =>
                m.Title.ToLower().Contains(loweredQuery) ||
                m.Summary.ToLower().Contains(loweredQuery) ||
                m.MovieGenres.Any(mg => mg.Genre.Name.ToLower().Contains(loweredQuery)))
            .OrderByDescending(m => m.Rating ?? 0)
            .ThenByDescending(m => m.Year)
            .Take(limit)
            .Select(x => new MovieBriefDto(
                x.Id,
                x.Title,
                x.Summary,
                x.Year,
                x.Rating,
                x.TrailerUrl,
                x.PosterUrl,
                x.MovieType))
            .ToListAsync(ct);

        if (directMatches.Count > 0)
        {
            return directMatches;
        }

        var terms = ExtractMeaningfulTerms(queryText).Take(5).ToList();
        if (terms.Count == 0)
        {
            return [];
        }

        var matchesById = new Dictionary<int, MovieBriefDto>();
        foreach (var term in terms)
        {
            var termMatches = await SearchSingleKeyword(term, limit * 2, ct);
            foreach (var movie in termMatches)
            {
                matchesById.TryAdd(movie.Id, movie);
            }
        }

        return matchesById.Values
            .OrderByDescending(m => m.Rating ?? 0)
            .ThenByDescending(m => m.Year)
            .Take(limit)
            .ToList();
    }

    private Task<List<MovieBriefDto>> SearchSingleKeyword(string term, int limit, CancellationToken ct)
    {
        return db.Movies
            .AsNoTracking()
            .Where(m =>
                m.Title.ToLower().Contains(term) ||
                m.Summary.ToLower().Contains(term) ||
                m.MovieGenres.Any(mg => mg.Genre.Name.ToLower().Contains(term)))
            .OrderByDescending(m => m.Rating ?? 0)
            .ThenByDescending(m => m.Year)
            .Take(limit)
            .Select(x => new MovieBriefDto(
                x.Id,
                x.Title,
                x.Summary,
                x.Year,
                x.Rating,
                x.TrailerUrl,
                x.PosterUrl,
                x.MovieType))
            .ToListAsync(ct);
    }

    private Task<List<MovieBriefDto>> SearchPopular(int limit, CancellationToken ct)
    {
        return db.Movies
            .AsNoTracking()
            .OrderByDescending(m => m.Rating ?? 0)
            .ThenByDescending(m => m.Year)
            .Take(limit)
            .Select(x => new MovieBriefDto(
                x.Id,
                x.Title,
                x.Summary,
                x.Year,
                x.Rating,
                x.TrailerUrl,
                x.PosterUrl,
                x.MovieType))
            .ToListAsync(ct);
    }

    private static IReadOnlyCollection<string> DetectGenres(string queryText)
    {
        var normalized = NormalizeForSearch(queryText);
        var genres = GenreAliases
            .Where(pair => pair.Value.Any(alias => normalized.Contains(alias)))
            .Select(pair => pair.Key)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return genres;
    }

    private static IEnumerable<string> ExtractMeaningfulTerms(string queryText)
    {
        var normalized = NormalizeForSearch(queryText);
        return normalized
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(term => term.Length > 1 && !SearchStopWords.Contains(term))
            .Distinct();
    }

    private static bool IsLikelySpecificUnmatchedLookup(string queryText, IReadOnlyCollection<string> terms)
    {
        if (terms.Count != 1)
        {
            return false;
        }

        var normalized = NormalizeForSearch(queryText);
        return !DiscoveryIntentMarkers.Any(marker => normalized.Contains(marker));
    }

    private static string NormalizeForSearch(string value)
    {
        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(character);
            if (category != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character == 'đ' ? 'd' : character);
            }
        }

        return string.Join(
            ' ',
            builder
                .ToString()
                .Normalize(NormalizationForm.FormC)
                .Split([' ', ',', '.', ':', ';', '!', '?', '-', '_', '/', '\\', '"', '\''],
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
    }

    private static readonly IReadOnlyDictionary<string, string[]> GenreAliases =
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["Action"] = ["action", "hanh dong", "danh nhau", "chien dau", "gay can"],
            ["Horror"] = ["horror", "kinh di", "ma", "rung ron"],
            ["Fantasy"] = ["fantasy", "gia tuong", "ky ao", "phep thuat"],
            ["Comedy"] = ["comedy", "hai", "hai huoc"],
            ["Drama"] = ["drama", "tam ly", "chinh kich"],
            ["Thriller"] = ["thriller", "giat gan", "hoi hop"],
            ["Mystery"] = ["mystery", "bi an", "trinh tham"],
            ["Romance"] = ["romance", "lang man", "tinh cam"],
            ["Sci-Fi"] = ["sci fi", "sci-fi", "science fiction", "khoa hoc vien tuong", "vien tuong"],
            ["Crime"] = ["crime", "toi pham", "hinh su"]
        };

    private static readonly string[] DiscoveryIntentMarkers =
    [
        "goi y",
        "de xuat",
        "recommend",
        "phim ve",
        "noi ve",
        "chu de",
        "tuong tu",
        "phu hop",
        "muon xem"
    ];

    private static readonly IReadOnlySet<string> SearchStopWords =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "bo", "cac", "can", "co", "cho", "cua", "de", "duoc", "gi", "goi", "he", "hay", "la", "loai", "mot",
            "nao", "nhung", "phim", "so", "the", "thong", "tim", "tin", "toi", "trong", "ve", "xem", "xuat", "y",
            "movie", "movies", "recommend", "recommendation", "system", "some", "the", "me"
        };
}
