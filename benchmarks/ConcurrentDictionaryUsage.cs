using System;
using System.Collections.Concurrent;

public static class ConcurrentDictionaryUsage 
{
    public static void Use()
    {
        var someState1 = new object();
        var someOtherState = 42;

        var dictionary = new ConcurrentDictionary<string, string>();

        dictionary.GetOrAdd("SomeKey", static (key, state) => {
            var (someState, someOtherState) = state;
            return $"{someState}_{someOtherState}";
        }, (someState1, someOtherState));
    }
}