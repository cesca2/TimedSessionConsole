using Spectre.Console;

namespace SessionLogger.Controllers;

public abstract class BaseController
{
    protected void DisplayMessage(string message, string color = "blue")
    {
        AnsiConsole.MarkupLine($"[{color}]{message}[/]");
    }

    protected bool ConfirmAction(string itemName)
    {
        var confirm = AnsiConsole.Confirm($"Are you sure you want to [red]{itemName}[/]?");

        return confirm;
    }

    protected bool OfferAction(string itemName)
    {
        var confirm = AnsiConsole.Confirm($"Would you like to {itemName}?", false);

        return confirm;
    }
}