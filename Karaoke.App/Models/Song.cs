namespace Karaoke.App.Models;

public class Song
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; }
    public string Artist { get; set; }
    public string Url { get; set; }
}