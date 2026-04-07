using Spectre.Console;

namespace SessionLogger.Models;

// model for UI
public class Session
{
    public int Id { get; set; } = 1;
    public string SessionType { get; set; }
    public string Date { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public string Duration {get; set;}

    public Session(string type, DateTime date, DateTime start, DateTime end) 
    {
       SessionType = type;
       StartTime = start.ToShortTimeString();
       EndTime= end.ToShortTimeString();
       Date = date.ToShortDateString();

       double hours = Math.Floor((end-start).TotalHours);
       double minutes = (end-start).TotalMinutes - Math.Floor((end-start).TotalHours) * 60;

       Duration = $"{hours}h {minutes}m";

    }    

    public void DisplayDetails()
    {
        var panel = new Panel(new Markup(
                                         $"[bold]Type:[/]  [cyan]{SessionType}[/]" + 
                                         $"   [bold]Date:[/]  [blue]{Date}[/]" + 
                                         $"   [bold]Duration:[/]  [green]{Duration}[/]"       
        ))
        {
            Border = BoxBorder.Rounded
        };

        AnsiConsole.Write(panel);
    }
  
}