// designed for interaction with CATAPI calls generic APIHandler, performs GET, POST, etc 
using System.Data.Common;
using SessionLogger.Models;
namespace SessionLogger.Controllers;
public class SessionServiceController : BaseController
{
    private APIHandler apiHandler = new APIHandler(Consts.API_ENDPOINT);
    

    public async Task<List<Session>> GetSessions(string query = "")
    {
        
        var info = await apiHandler.RetrieveAPIInfo<SessionAPI>("Sessions");

        return info.Select( session => new Session(type: session.Type, start: DateTime.Parse(session.Start), end: DateTime.Parse(session.End), date: DateTime.Parse(session.Date) )
                {
                    Id = session.Id, 

                }).ToList();
        

    }
    public async Task<bool> PostSession(Session PostSession)
    { 
        var post = new SessionAPI() { Type=PostSession.SessionType, Start = PostSession.StartTime, End=PostSession.EndTime, Date=PostSession.Date };
        var success = await apiHandler.PostAPIInfo(post, "Sessions");

        return success;

    }

    public async Task<bool> PutSession(Session PutSession)
    { 
        var post = new SessionAPI() { Id = PutSession.Id, Type=PutSession.SessionType, Start = PutSession.StartTime, End=PutSession.EndTime, Date=PutSession.Date };
        var success = await apiHandler.PutAPIInfo(post, $"Sessions/{PutSession.Id}");

        return success;

    }

    public async Task<bool> DeleteSession(int Id)
    { 
        var success = await apiHandler.DeleteAPIInfo($"Sessions/{Id}");

        return success;

    }

}

