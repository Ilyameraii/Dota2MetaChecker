using Dota2MetaChecker.Common.Constants;
using Dota2MetaChecker.Common.Enums;
using Dota2MetaChecker.Common.Models;
using Services.Contracts.Data_sync;
using Services.Extensions;

namespace Services.Data_sync.CallbackHandlers;

public class RoleCallbackHandler : ICallbackHandler
{
    public bool CanHandle(string callbackData) => callbackData.StartsWith(CallbackPrefixes.Role);

    public void Handle(UserPreferences prefs, string callbackData)
    {
        var role = Enum.Parse<RoleFlags>(callbackData[CallbackPrefixes.Role.Length..]);
        prefs.ProcessingOptions.Roles ^= role;
    }
}