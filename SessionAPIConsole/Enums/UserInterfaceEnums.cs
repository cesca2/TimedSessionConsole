namespace SessionLogger;

internal class UserInterfaceEnums
{
    internal enum MenuAction
    {
        ViewSessions,
        AddSession,
        DeleteSession,
        UpdateSession,
        Exit
    }

    internal enum FilterAction
    {
        AllTime,
        LastWeek,
        LastMonth,
        Custom
    }

    internal enum FilterUnit
    {
        Days,
        Months,
        Years
    }


}