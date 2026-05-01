using Dota2MetaChecker.Common.Enums;
using Dota2MetaChecker.Common.Models;
using Services.Contracts.Data_sync;
using Services.Extensions;

namespace Services.Data_sync.CallbackHandlers;

public class RankCallbackHandler : ICallbackHandler
{
    public bool CanHandle(string callbackData) => callbackData.StartsWith("rank:");

    public void Handle(UserPreferences prefs, string callbackData)
    {
        var rank = Enum.Parse<Rank>(callbackData[5..]);
        prefs.ProcessingOptions.Ranks ^= rank.ToFlag();
    }
}