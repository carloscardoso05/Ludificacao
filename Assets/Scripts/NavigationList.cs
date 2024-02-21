using System.Collections.Generic;
using System.Linq;

public class NavigationList<T> : List<T>
{
    private int _currentIndex = 0;
    public int CurrentIndex
    {
        get
        {
            if (_currentIndex > Count - 1) { _currentIndex = Count - 1; }
            if (_currentIndex < 0) { _currentIndex = 0; }
            return _currentIndex;
        }
        set { _currentIndex = value; }
    }

    public T Next
    {
        get => ElementAtOrDefault(_currentIndex+1, this.Last());
    }

    public T Previous
    {
        get => ElementAtOrDefault(_currentIndex-1, this.First());
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