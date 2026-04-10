using SessionLogger;
using Spectre.Console;

UserInterface userInterface = new(AnsiConsole.Console);
await userInterface.MainMenu();