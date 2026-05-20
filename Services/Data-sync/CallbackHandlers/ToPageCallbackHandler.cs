using Dota2MetaChecker.Common.Constants;
using Dota2MetaChecker.Common.Models;
using Services.Contracts.Data_sync;

namespace Services.Data_sync.CallbackHandlers;

public class ToPageCallbackHandler:ICallbackHandler
{
    public bool CanHandle(string callbackData) => callbackData.StartsWith(CallbackPrefixes.ToPage);

    public void Handle(UserPreferences prefs, string callbackData)
    {
        if (!TryParsePageData(callbackData, out var page, out var max))
            return;

        prefs.PageNumber = Math.Clamp(page, 0, max);
    }

    private static bool TryParsePageData(string callbackData, out int page, out int max)
    {
        page = max = 0;
        var parts = callbackData[CallbackPrefixes.ToPage.Length..].TrimStart(':').Split(':');
    
        if (parts.Length == 1 && int.TryParse(parts[0], out page))
        {
            max = int.MaxValue;
            return true;
        }
    
        return parts.Length == 2
               && int.TryParse(parts[0], out page)
               && int.TryParse(parts[1], out max);
    }
}