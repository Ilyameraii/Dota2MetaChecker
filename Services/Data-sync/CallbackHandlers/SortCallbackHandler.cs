using Dota2MetaChecker.Common.Constants;
using Dota2MetaChecker.Common.Models;
using Services.Contracts.Data_sync;

namespace Services.Data_sync.CallbackHandlers;

public class SortCallbackHandler: ICallbackHandler
{
    public bool CanHandle(string callbackData) => callbackData.StartsWith(CallbackPrefixes.Sort);

    public void Handle(UserPreferences prefs, string callbackData)
    {
        var newSortType = Enum.Parse<SortType>(callbackData[CallbackPrefixes.Sort.Length..]);

        if (prefs.ProcessingOptions.SortBy == newSortType)
            prefs.ProcessingOptions.IsDescending = !prefs.ProcessingOptions.IsDescending;
        else
        {
            prefs.ProcessingOptions.SortBy = newSortType;
            prefs.ProcessingOptions.IsDescending = true;
        }

        prefs.PageNumber = 0;
    }
}