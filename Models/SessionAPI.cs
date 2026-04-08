namespace SessionLogger.Models;

// model for API
public class SessionAPI
{
    public int Id { get; set; }
    public required string Type { get; set; }
    public required string Date { get; set; }
    public required string Start { get; set; }
    public required string End { get; set; }


}