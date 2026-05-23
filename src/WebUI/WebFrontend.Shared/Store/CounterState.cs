using Fluxor;

namespace WebFrontend.Client.Store;

[FeatureState]
public record CounterState
{
    public int ClickCount { get; init; }
}

public record IncrementCounterAction();

public static class CounterReducers
{
    [ReducerMethod]
    public static CounterState ReduceIncrementCounterAction(CounterState state, IncrementCounterAction action)
        => state with { ClickCount = state.ClickCount + 1 };
}
