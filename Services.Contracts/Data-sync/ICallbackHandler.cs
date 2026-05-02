using Dota2MetaChecker.Common.Models;

namespace Services.Contracts.Data_sync;

public interface ICallbackHandler
{
    bool CanHandle(string callbackData);

    void Handle(UserPreferences prefs, string callbackData);
}