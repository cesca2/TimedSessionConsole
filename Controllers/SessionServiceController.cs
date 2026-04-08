// designed for interaction with CATAPI calls generic APIHandler, performs GET, POST, etc 
using System.Data.Common;
using SessionLogger.Models;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
namespace SessionLogger.Controllers;
public class SessionServiceController : BaseController
{
    private APIHandler apiHandler = new APIHandler(Consts.API_ENDPOINT);
    

    public async Task<List<Session>> GetSessions(string DateFilter = "")
    {
        var url = "Sessions";
        if (!string.IsNullOrEmpty(DateFilter))
        {
            url += "?LastDate=" + DateTime.Parse(DateFilter).ToString("yyyy-MM-dd");
        }
        Console.WriteLine(url);
        var info = await apiHandler.RetrieveAPIInfo<SessionAPI>(url);

        return info.Select( session => new Session(type: session.Type, start: DateTime.Parse(session.Start), end: DateTime.Parse(session.End), date: DateTime.ParseExact(session.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture) )
                {
                    Id = session.Id, 

                }).ToList();
        

    }
    public async Task<bool> PostSession(Session PostSession)
    { 
        var post = new SessionAPI() { Type=PostSession.SessionType, Start = PostSession.StartTime, End=PostSession.EndTime, Date=DateTime.Parse(PostSession.Date).ToString("yyyy-MM-dd")};
        var success = await apiHandler.PostAPIInfo(post, "Sessions");

        return success;

    }

    public async Task<bool> PutSession(Session PutSession)
    { 
        var post = new SessionAPI() { Id = PutSession.Id, Type=PutSession.SessionType, Start = PutSession.StartTime, End=PutSession.EndTime, Date=DateTime.Parse(PutSession.Date).ToString("yyyy-MM-dd") };
        var success = await apiHandler.PutAPIInfo(post, $"Sessions/{PutSession.Id}");

        return success;

    }

    public async Task<bool> DeleteSession(int Id)
    { 
        var success = await apiHandler.DeleteAPIInfo($"Sessions/{Id}");

        return success;

    }

}

