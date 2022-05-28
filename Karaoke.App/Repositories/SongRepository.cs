using System.Net.Http.Json;
using Karaoke.App.Models;

namespace Karaoke.App.Repositories;

public class SongRepository
{
    private readonly HttpClient _httpClient;
    private List<Song> _songs = new();
    
    public SongRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
        // TODO: Read in songs from file and cache
        /*
        _songs.Add(new Song
        {
            Title = "Uneasy Hearts Weigh The Most",
            Artist = "Dance Gavin Dance",
            Url = "https://youtube.com/dgd-uneasy"
        });
        _songs.Add(new Song
        {
            Title = "Tree Village",
            Artist = "Dance Gavin Dance",
            Url = "https://youtube.com/dgd-tree"
        });
        */
    }

    public List<Song> Import(string csv)
    {
        var lines = csv.Split("\r\n");
        if (!lines.Any()) lines = csv.Split("\n");
        if (!lines.Any()) lines = csv.Split(Environment.NewLine);
            
        var id = 0;
        var currentArtist = string.Empty;
        foreach (var line in lines)
        {
            var columns = line.Split(",");
            if (!string.IsNullOrWhiteSpace(columns[0]))
            {
                currentArtist = columns[0].Trim().Replace(":", "");
            }
            else if (!string.IsNullOrWhiteSpace(columns[1]) && !string.IsNullOrWhiteSpace(columns[2]))
            {
                _songs.Add(new Song
                {
                    Id = (++id).ToString(),
                    Title = columns[1].Trim(),
                    Artist = currentArtist,
                    Url = columns[2].Trim()
                });
            }
            else continue;
        }
            
        return _songs;
    }

    public async Task<IEnumerable<Song>> GetAll()
    {
        if (!_songs.Any())
        {
            _songs  = await _httpClient.GetFromJsonAsync<List<Song>>("songs.json");
        }
            
        return _songs;
    }
    
    public async Task<List<Song>> GetAllOld()
    {
        if (!_songs.Any())
        {
            var csv = await _httpClient.GetStringAsync("songs.csv");
            var lines = csv.Split("\r\n");
            
            var id = 0;
            var currentArtist = string.Empty;
            foreach (var line in lines)
            {
                var columns = line.Split(",");
                if (!string.IsNullOrWhiteSpace(columns[0]))
                {
                    currentArtist = columns[0].Trim().Replace(":", "");
                }
                else if (!string.IsNullOrWhiteSpace(columns[1]) && !string.IsNullOrWhiteSpace(columns[2]))
                {
                    _songs.Add(new Song
                    {
                        Id = (++id).ToString(),
                        Title = columns[1].Trim(),
                        Artist = currentArtist,
                        Url = columns[2].Trim()
                    });
                }
                else continue;
            }

            //_songs = _songs.OrderBy(x => x.Artist).ThenBy(x => x.Title).ToList();
        }
            
        return _songs;
    }

    public async Task<List<Song>> GetAllByArtist(string artist)
    {
        return (await GetAll())
            .Where(s => string.Equals(s.Artist, artist, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public async Task<Song> GetByTitle(string title)
    {
        return (await GetAll())
            .FirstOrDefault(s => string.Equals(s.Title, title, StringComparison.OrdinalIgnoreCase));
    }
}