using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;

public class NavigationList<T> : List<T>
{
    private int _currentIndex = 0;
    public int CurrentIndex
    {
        get => _currentIndex;
        set { _currentIndex = math.clamp(value, 0, Count - 1); }
    }

    public T Next
    {
        get => ElementAtOrDefault(_currentIndex + 1, this.Last());
    }

    public T Previous
    {
        get => ElementAtOrDefault(_currentIndex - 1, this.First());
    }

    public T Current
    {
        get => this[CurrentIndex];
    }

    private T ElementAtOrDefault(int index, T @default)
    {
        return index >= 0 && index < Count ? this[index] : @default;
    }
}