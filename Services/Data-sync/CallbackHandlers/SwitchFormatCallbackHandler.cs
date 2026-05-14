using Dota2MetaChecker.Common.Constants;
using Dota2MetaChecker.Common.Models;
using Services.Contracts.Data_sync;

namespace Services.Data_sync.CallbackHandlers;

public class SwitchFormatCallbackHandler : ICallbackHandler
{
    public bool CanHandle(string callbackData)
    {
        return callbackData.StartsWith(CallbackConstants.SwitchFormat);
    }

    public void Handle(UserPreferences prefs, string callbackData)
    {
        prefs.SwitchFormat();
    }
}