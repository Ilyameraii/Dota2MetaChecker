using Dota2MetaChecker.Common.Constants;
using Dota2MetaChecker.Common.Enums;
using Dota2MetaChecker.Common.Models;
using Services.Contracts.Data_sync;

namespace Services.Data_sync.CallbackHandlers;

public class RankCallbackHandler : ICallbackHandler
{
    public bool CanHandle(string callbackData)
    {
        return callbackData.StartsWith(CallbackPrefixes.Rank);
    }

    public void Handle(UserPreferences prefs, string callbackData)
    {
        var rank = Enum.Parse<RankFlags>(callbackData[CallbackPrefixes.Rank.Length..]);
        prefs.ProcessingOptions.Ranks ^= rank;
    }
}