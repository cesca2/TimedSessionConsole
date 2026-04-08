using Spectre.Console;
using static SessionLogger.UserInterfaceEnums;
using SessionLogger.Models;
using SessionLogger.Controllers;


namespace SessionLogger;


internal class UserInterface
{
    private readonly SessionServiceController _sessionServiceController = new();

    private void DisplayMessage(string message, string color = "yellow")
        {
            AnsiConsole.MarkupLine($"[{color}]{message}[/]");
        }

    private bool ConfirmAction(string actionName, string color = "yellow")
        {
            var ConfirmAction = AnsiConsole.Confirm($"Are you sure you want to [{color}]{actionName}[/]?");

            return ConfirmAction;
        }
    internal async Task MainMenu()
    {
        DisplayMessage("");

        AnsiConsole.Write(
            new FigletText("Session logger"));
        DisplayMessage("");
        DisplayMessage("Welcome to the Session Logger application!", "white");
        DisplayMessage("");
        DisplayMessage("Press Any Key to Continue.");
        Console.ReadKey();


        while (true)
        {
            Console.Clear();

            var actionChoice = AnsiConsole.Prompt(
                new SelectionPrompt<MenuAction>()
                .Title("Please select an action:")
                .UseConverter(e => System.Text.RegularExpressions.Regex.Replace(e.ToString(), "([a-z])([A-Z])", "$1 $2"))
                .AddChoices(Enum.GetValues<MenuAction>()));

            switch (actionChoice)
            {
                case MenuAction.ViewSessions:
                    await ViewSessions();
                    break;
                case MenuAction.AddSession:
                    await AddSession();
                    break;
                case MenuAction.DeleteSession:
                    await DeleteSession();
                    break;
                case MenuAction.UpdateSession:
                    await UpdateSession();
                    break;
                case MenuAction.Exit:
                    if ( ConfirmAction("exit")) 
                    {
                        Environment.Exit(1);
                    }
                    break;

            }


        }
    }
    

    public async Task ViewSessions()
    {

        var table = new Table();
        table.Border(TableBorder.Rounded);

        table.AddColumn("[yellow]Type[/]");
        table.AddColumn("[yellow]Date[/]");
        table.AddColumn("[yellow]Length[/]");
        
        List<Session> entries = new List<Session>(); 

        entries = await _sessionServiceController.GetSessions(); // (filterDate)

        foreach (var entry in entries)
        {
            table.AddRow(
                $"[cyan]{entry.SessionType}[/]",
                $"[blue]{entry.Date}[/]",
                $"[green]{entry.Duration}[/]"
                );
        }
        

        AnsiConsole.Write(table);
        DisplayMessage("Press Any Key to Continue.");
        Console.ReadKey();

    }
    

    private string filterSessions()
    {
        var filterChoice = AnsiConsole.Prompt(
            new SelectionPrompt<FilterAction>()
            .Title("Filter date?")
            .UseConverter(e => System.Text.RegularExpressions.Regex.Replace(e.ToString(), "([a-z])([A-Z])", "$1 $2"))
            .AddChoices(Enum.GetValues<FilterAction>()));


        switch (filterChoice)
            {
                case FilterAction.AllTime:
                    return "";
                case FilterAction.LastWeek:
                    return DateTime.Now.AddDays(-7).ToShortDateString();
                case FilterAction.LastMonth:
                    return DateTime.Now.AddMonths(-1).ToShortDateString();
                case FilterAction.Custom:                    
                    var filterUnit = AnsiConsole.Prompt(new SelectionPrompt<FilterUnit>()
                    .Title("Filter by number of years, months or days?")
                    .AddChoices(Enum.GetValues<FilterUnit>()));

                    var length= AnsiConsole.Ask<int>($"Enter the number of {filterUnit}:");
                    
                    Console.Clear();
                
                    switch(filterUnit)
                    {
                        case FilterUnit.Days:
                            return DateTime.Now.AddDays(-length).ToShortDateString();
                        case FilterUnit.Months:
                            return DateTime.Now.AddMonths(-length).ToShortDateString();
                        case FilterUnit.Years:
                            return DateTime.Now.AddYears(-length).ToShortDateString();
                        default: return "";
                    
                    }
                    
                default: return "";

            }
        
    }

    private  Session UserInputSession(string message = "add")
    {

        var type= AnsiConsole.Ask<string>($"Enter the [cyan]type[/] of session to {message}:");
        
        var dateprompt = new TextPrompt<string>($"Enter the [blue]date (DD/MM/YYYY)[/] of the session to {message}:")
        .Validate(input =>
            DateTime.TryParse(
                input,
                out _
            ) && DateTime.Parse(input).Date <= DateTime.Now.Date, "[red]Please check date format matches (DD/MM/YYYY), and is not in the future[/]");

        var date= AnsiConsole.Prompt(dateprompt);
    
        var startTime = AnsiConsole.Ask<DateTime>($"Enter the [green]start time (HH:MM) [/] of the session to {message}:");
        var endprompt = new TextPrompt<DateTime>($"Enter the [green]end time (HH:MM)[/] of the session to {message}:")
        .Validate(input => 
                  input > startTime, $"[red]End time must be after specified start time, {startTime.TimeOfDay}[/]");
        
        var endTime = AnsiConsole.Prompt(endprompt);
        
        var newSession= new Session(type, DateTime.Parse(date), startTime, endTime);

        return newSession;
        
    }


    private  async Task AddSession()
    {
        var newSession= UserInputSession();
        newSession.DisplayDetails();

        if (ConfirmAction("add this session"))
        {
            var success = await _sessionServiceController.PostSession(newSession);

            if (success) {DisplayMessage("Session added");}
            

        }
        else
        {
            DisplayMessage("Addition of session cancelled", "red");
        }

        DisplayMessage("Press Any Key to Continue.");
        Console.ReadKey();


    }

    private async Task DeleteSession()
    {
        var filterDate = filterSessions();
        var deletionEntries = await _sessionServiceController.GetSessions(DateFilter: filterDate); // (filterDate);
        
        if (deletionEntries.Any())
        {
         var sessionToDelete = AnsiConsole.Prompt(
                new SelectionPrompt<Session>()
                    .Title("Select a [red]session[/] to delete:")
                    .UseConverter(e => $"{e.SessionType}, {e.Date}, {e.Duration} ")
                    .AddChoices(deletionEntries));
        sessionToDelete.DisplayDetails();
        
        if (ConfirmAction("delete the above entry?", "red"))
        {
            
            var success = await _sessionServiceController.DeleteSession(sessionToDelete.Id);
            if (success) {DisplayMessage("Session deleted");}
        }
        else
        {
            DisplayMessage("Deletion of session cancelled", "red");
        }
        }
        else
        {
            DisplayMessage("No records available for deletion", "red");
        }
        
        DisplayMessage("Press Any Key to Continue.");
        Console.ReadKey();


    }

    private async Task UpdateSession()
    {
        var filterDate = filterSessions();
        
        var updateEntries =  await _sessionServiceController.GetSessions(DateFilter: filterDate);
        
        if (updateEntries.Any())
        {
        var sessionToUpdate = AnsiConsole.Prompt(
                new SelectionPrompt<Session>()
                    .Title("Select a [yellow]session[/] to update:")
                    .UseConverter(e => $"{e.SessionType, -10} {e.Date, -10}, {e.Duration, -8} ")
                    .AddChoices(updateEntries));
        
        var newSession= UserInputSession("update");
        newSession.Id = sessionToUpdate.Id;
        
        DisplayMessage("", "white");
        DisplayMessage("Replace");
        sessionToUpdate.DisplayDetails();
        DisplayMessage("with");
        newSession.DisplayDetails();
        
        if (ConfirmAction($"update this entry as detailed above", "yellow"))
        {
            await _sessionServiceController.PutSession(newSession);
            DisplayMessage("Session updated");
        }
        else
        {
            DisplayMessage("Update of session cancelled", "red");
        }
        }
        else
        {
            DisplayMessage("No records available to update", "red");
        }
        DisplayMessage("Press Any Key to Continue.");
        Console.ReadKey();

    }
}
