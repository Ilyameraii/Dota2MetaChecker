using Dota2MetaChecker.Common.Constants;
using Dota2MetaChecker.Common.Models;
using Services.Contracts.Data_sync;

namespace Services.Data_sync.CallbackHandlers;

public class ClearOptionsCallbackHandler : ICallbackHandler
{
    public bool CanHandle(string callbackData) => callbackData == CallbackConstants.ClearOptions;

    public void Handle(UserPreferences prefs, string callbackData = "")
    {
        prefs.Reset();
    }
}