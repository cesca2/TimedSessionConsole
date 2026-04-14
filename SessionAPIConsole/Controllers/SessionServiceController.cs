using SessionLogger.Models;
using System.Globalization;
namespace SessionLogger.Controllers;

public class SessionServiceController : BaseController
{
    private readonly APIHandler _apiHandler = new APIHandler(Consts.API_ENDPOINT);


    public async Task<List<Session>> GetSessions(string dateFilter = "")
    {
        var url = "Sessions";
        if (!string.IsNullOrEmpty(dateFilter))
        {
            url += "?LastDate=" + DateTime.Parse(dateFilter).ToString("yyyy-MM-dd");
        }
        var info = await _apiHandler.RetrieveAPIInfo<SessionAPI>(url);

        return info.Select(session => new Session(type: session.Type, start: DateTime.Parse(session.Start), end: DateTime.Parse(session.End), date: DateTime.Parse(session.Date))
        {
            Id = session.Id.ToString(),

        }).ToList();


    }
    public async Task<bool> PostSession(Session postSession)
    {
        var post = new SessionAPI() { Type = postSession.SessionType, Start = postSession.StartTime, End = postSession.EndTime, Date = DateTime.Parse(postSession.Date).ToString() };
        var success = await _apiHandler.PostAPIInfo(post, "Sessions");

        return success;

    }

    public async Task<bool> PutSession(Session putSession)
    {
        var post = new SessionAPI() { Id = Guid.Parse(putSession.Id), Type = putSession.SessionType, Start = putSession.StartTime, End = putSession.EndTime, Date = DateTime.Parse(putSession.Date).ToString() };
        var success = await _apiHandler.PutAPIInfo(post, $"Sessions/{putSession.Id}");

        return success;

    }

    public async Task<bool> DeleteSession(string id)
    {
        var success = await _apiHandler.DeleteAPIInfo($"Sessions/{id}");

        return success;

    }

}