using SessionLogger.Controllers;
using SessionLogger.Models;
using Spectre.Console;
using System.Runtime.Serialization;
using static SessionLogger.UserInterfaceEnums;


namespace SessionLogger;


public class UserInterface(IAnsiConsole console)
{
    private readonly SessionServiceController _sessionServiceController = new();
    public readonly Dictionary<string, string> _errorMessages = new Dictionary<string, string>
    {
        ["FutureDate"] = "Date cannot be in the future",
        ["InvalidDateFormat"] = "Date format should match (DD/MM/YYYY)",
        ["InvalidTimeFormat"] = "Time format should match HH:MM",
        ["EndTimeBeforeStart"] = "End time cannot be before start time"
    };

    private void DisplayMessage(string message, string color = "yellow")
    {
        console.MarkupLine($"[{color}]{message}[/]");
    }

    private bool ConfirmAction(string actionName, string color = "yellow")
    {
        var confirmAction = console.Confirm($"Are you sure you want to [{color}]{actionName}[/]?");

        return confirmAction;
    }
    internal async Task MainMenu()
    {
        DisplayMessage("");

        console.Write(
            new FigletText("Session logger"));
        DisplayMessage("");
        DisplayMessage("Welcome to the Session Logger application!", "white");
        DisplayMessage("");
        DisplayMessage("Press Any Key to Continue.");
        Console.ReadKey();


        while (true)
        {
            Console.Clear();

            var actionChoice = console.Prompt(
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
                    if (ConfirmAction("exit"))
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

        entries = await _sessionServiceController.GetSessions();

        foreach (var entry in entries)
        {
            table.AddRow(
                $"[cyan]{entry.SessionType}[/]",
                $"[blue]{entry.Date}[/]",
                $"[green]{entry.Duration}[/]"
                );
        }


        console.Write(table);
        DisplayMessage("Press Any Key to Continue.");
        Console.ReadKey();

    }


    private string FilterSessions()
    {
        var filterChoice = console.Prompt(
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
                var filterUnit = console.Prompt(new SelectionPrompt<FilterUnit>()
                .Title("Filter by number of years, months or days?")
                .AddChoices(Enum.GetValues<FilterUnit>()));

                var length = console.Ask<int>($"Enter the number of {filterUnit}:");

                Console.Clear();

                switch (filterUnit)
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

    public Session UserInputSession(string message = "add")
    {

        var type = console.Ask<string>($"Enter the [cyan]type[/] of session to {message}:");

        var dateprompt = new TextPrompt<string>($"Enter the [blue]date (DD/MM/YYYY)[/] of the session to {message}:")
        .Validate(input =>
            {if (!
            DateTime.TryParse(
                input,
                out _)
            || (DateTime.Parse(input).Date == DateTime.Parse("12:00").Date)) return ValidationResult.Error($"[red]Invalid input: {_errorMessages["InvalidDateFormat"]}[/]");
            else if ( DateTime.Parse(input).Date >= DateTime.Now.Date) return ValidationResult.Error($"[red]Invalid input: {_errorMessages["FutureDate"]}[/]");
            else return ValidationResult.Success();
            })  
        .ValidationErrorMessage($"[red]Invalid input: {_errorMessages["InvalidDateFormat"]}[/]");
        
        var date = console.Prompt(dateprompt);

        var startprompt = new TextPrompt<DateTime>($"Enter the [green]start time (HH:MM)[/] of the session to {message}:")
        .Validate(input =>
                 input.Date == DateTime.Parse("12:00").Date  , $"[red]Invalid input: {_errorMessages["InvalidTimeFormat"]}[/]")
        .ValidationErrorMessage($"[red]Invalid input: {_errorMessages["InvalidTimeFormat"]}[/]");
        
        var startTime = console.Prompt(startprompt);

        var endprompt = new TextPrompt<DateTime>($"Enter the [green]end time (HH:MM)[/] of the session to {message}:")
        .Validate(input =>
                 { if (input.Date != DateTime.Parse("12:00").Date) return ValidationResult.Error($"[red]Invalid input: {_errorMessages["InvalidTimeFormat"]}[/]");
                   else if (input.TimeOfDay < startTime.TimeOfDay)  return ValidationResult.Error( $"[red]Invalid input: {_errorMessages["EndTimeBeforeStart"]}, {startTime.TimeOfDay}[/]");
                 else return ValidationResult.Success();})
        .ValidationErrorMessage($"[red]Invalid input: {_errorMessages["InvalidTimeFormat"]}[/]");

        var endTime = console.Prompt(endprompt);

        var newSession = new Session(type, DateTime.Parse(date), startTime, endTime);

        return newSession;

    }


    private async Task AddSession()
    {
        var newSession = UserInputSession();
        newSession.DisplayDetails();

        if (ConfirmAction("add this session"))
        {
            var success = await _sessionServiceController.PostSession(newSession);

            if (success) { DisplayMessage("Session added"); }


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
        var filterDate = FilterSessions();
        var deletionEntries = await _sessionServiceController.GetSessions(dateFilter: filterDate); // (filterDate);

        if (deletionEntries.Any())
        {
            var sessionToDelete = console.Prompt(
                   new SelectionPrompt<Session>()
                       .Title("Select a [red]session[/] to delete:")
                       .UseConverter(e => $"{e.SessionType}, {e.Date}, {e.Duration} ")
                       .AddChoices(deletionEntries));
            sessionToDelete.DisplayDetails();

            if (ConfirmAction("delete the above entry?", "red"))
            {

                var success = await _sessionServiceController.DeleteSession(sessionToDelete.Id);
                if (success) { DisplayMessage("Session deleted"); }
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
        var filterDate = FilterSessions();

        var updateEntries = await _sessionServiceController.GetSessions(dateFilter: filterDate);

        if (updateEntries.Any())
        {
            var sessionToUpdate = console.Prompt(
                    new SelectionPrompt<Session>()
                        .Title("Select a [yellow]session[/] to update:")
                        .UseConverter(e => $"{e.SessionType,-10} {e.Date,-10}, {e.Duration,-8} ")
                        .AddChoices(updateEntries));

            var newSession = UserInputSession("update");
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