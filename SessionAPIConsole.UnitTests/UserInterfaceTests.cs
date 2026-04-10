namespace UnitTests;

using SessionLogger;
using SessionLogger.Models;
using Spectre.Console.Testing;

public class UserInterfaceTests
{
    private UserInterface _input;


    private static readonly object[] _invalidTimeCases =
    {
        new TestCaseData("xx:xx"),
        new TestCaseData("abc"),
        new TestCaseData("12/02/2025")

    };

    private static readonly object[] _invalidDateCases =
    {
        new TestCaseData("ab/cd/ef"),
        new TestCaseData("bc"),
        new TestCaseData("12:30")

    };

    [TestCase("Python", "12/02/2026", "10:30", "12:30")]
    public void CorrectUserInput_ReturnsCorrectSession(string type, string date, string start, string end)
    {

        var console = new TestConsole();
        console.Input.PushTextWithEnter(type);
        console.Input.PushTextWithEnter(date);
        console.Input.PushTextWithEnter(start);
        console.Input.PushTextWithEnter(end);

        console.Profile.Capabilities.Interactive = true;

        _input = new UserInterface(console);

        var stringReturn = _input.UserInputSession();

        var expectedSession = new Session(type, DateTime.Parse(date), DateTime.Parse(start), DateTime.Parse(end));

        Assert.That(stringReturn.SessionType, Is.EqualTo(expectedSession.SessionType));
        Assert.That(stringReturn.Date, Is.EqualTo(expectedSession.Date));
        Assert.That(stringReturn.StartTime, Is.EqualTo(expectedSession.StartTime));
        Assert.That(stringReturn.EndTime, Is.EqualTo(expectedSession.EndTime));

    }

    [Test]
    public void EndTimeBeforeStartTime_DisplaysValidationErrorText()
    {

        var console = new TestConsole();
        console.Input.PushTextWithEnter("Python");
        console.Input.PushTextWithEnter("12/02/2026");
        console.Input.PushTextWithEnter("10:30");
        console.Input.PushTextWithEnter("09:30");
        console.Input.PushTextWithEnter("11:30");

        console.Profile.Capabilities.Interactive = true;

        _input = new UserInterface(console);

        _input.UserInputSession();

        Assert.That(console.Output, Does.Contain(_input._errorMessages["EndTimeBeforeStart"]));
    }
    [Test]
    public void DateInFuture_DisplaysValidationErrorText()
    {

        var console = new TestConsole();
        console.Input.PushTextWithEnter("Python");
        console.Input.PushTextWithEnter("12/02/4000");
        console.Input.PushTextWithEnter("12/02/2000");
        console.Input.PushTextWithEnter("10:30");
        console.Input.PushTextWithEnter("12:30");

        console.Profile.Capabilities.Interactive = true;

        _input = new UserInterface(console);

        _input.UserInputSession();

        Assert.That(console.Output, Does.Contain(_input._errorMessages["FutureDate"]));


    }
    [TestCaseSource(nameof(_invalidDateCases))]
    public void IncorrectDateInput_DisplaysValidationErrorText(string incorrectDate)
    {

        var console = new TestConsole();
        console.Input.PushTextWithEnter("Python");
        console.Input.PushTextWithEnter(incorrectDate);
        console.Input.PushTextWithEnter("12/02/2026");
        console.Input.PushTextWithEnter("10:30");
        console.Input.PushTextWithEnter("12:30");

        console.Profile.Capabilities.Interactive = true;

        _input = new UserInterface(console);

        _input.UserInputSession();

        Assert.That(console.Output, Does.Contain(_input._errorMessages["InvalidDateFormat"]));

    }
    [TestCaseSource(nameof(_invalidTimeCases))]
    public void IncorrectTimeInput_DisplaysValidationErrorText(string incorrectTime)
    {

        var console = new TestConsole();
        console.Input.PushTextWithEnter("Python");
        console.Input.PushTextWithEnter("12/02/2026");
        console.Input.PushTextWithEnter(incorrectTime);
        console.Input.PushTextWithEnter("10:30");
        console.Input.PushTextWithEnter("12:30");

        console.Profile.Capabilities.Interactive = true;

        _input = new UserInterface(console);

        var stringReturn = _input.UserInputSession();

        var expectedSession = new Session("Python", DateTime.Parse("12/02/2026"), DateTime.Parse("10:30"), DateTime.Parse("12:30"));

        Assert.That(console.Output, Does.Contain(_input._errorMessages["InvalidTimeFormat"]));

    }
}