using Dota2MetaChecker.Common.Constants;
using Dota2MetaChecker.Common.Models;
using Services.Contracts.Data_sync;

namespace Services.Data_sync.CallbackHandlers;

public class PageCallbackHandler : ICallbackHandler
{
    public bool CanHandle(string callbackData)
    {
        return callbackData.StartsWith(CallbackPrefixes.Page);
    }

    public void Handle(UserPreferences prefs, string callbackData)
    {
        var direction = callbackData[CallbackPrefixes.Page.Length..];
        prefs.PageNumber = direction == PageDirection.Next
            ? prefs.PageNumber + 1
            : Math.Max(0, prefs.PageNumber - 1);
    }
}