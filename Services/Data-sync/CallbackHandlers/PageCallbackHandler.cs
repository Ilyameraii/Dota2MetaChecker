using Dota2MetaChecker.Common.Models;
using Services.Contracts.Data_sync;

namespace Services.Data_sync.CallbackHandlers;

public class PageCallbackHandler:ICallbackHandler
{
    public bool CanHandle(string callbackData)=> callbackData.StartsWith("page:");

    public void Handle(UserPreferences prefs, string callbackData)
    {
        var direction = callbackData[5..];
        prefs.PageNumber = direction == "next"
            ? prefs.PageNumber + 1
            : Math.Max(0, prefs.PageNumber - 1);
    }
}