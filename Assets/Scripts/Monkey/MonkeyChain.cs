using System.Collections.Generic;

public class MonkeyChain
{
    Monkey _currentMonkey;

    public Monkey current => _currentMonkey;

    public MonkeyChain(Monkey monkey)
    {
        _currentMonkey = monkey;
    }

    public void GoToLeft()
    {
        Monkey nextMonkey = _currentMonkey;
        while (nextMonkey != null)
        {
            _currentMonkey = nextMonkey;
            nextMonkey = _currentMonkey.leftMonkey;
        }
    }

    public void GoToRight()
    {
        Monkey nextMonkey = _currentMonkey;
        while (nextMonkey != null)
        {
            _currentMonkey = nextMonkey;
            nextMonkey = _currentMonkey.rightMonkey;
        }
    }

    public Monkey MonkeyAtRight()
    {
        Monkey monkey = null;
        foreach (var m in LeftToRight())
        {
            monkey = m;
        }
        return monkey;
    }

    public Monkey MonkeyAtLeft()
    {
        Monkey monkey = null;
        foreach (var m in RightToLeft())
        {
            monkey = m;
        }
        return monkey;
    }

    public IEnumerable<Monkey> LeftToRight()
    {
        HashSet<Monkey> set = new HashSet<Monkey>();
        Monkey current = _currentMonkey;
        while (current != null)
        {
            yield return current;
            if (!set.Add(current))
            {
                UnityEngine.Debug.LogError("Error : Infinite monkey chain detected !");
                yield break;
            }
            current = current.rightMonkey;
        }
    }

    public IEnumerable<Monkey> RightToLeft()
    {
        HashSet<Monkey> set = new HashSet<Monkey>();
        Monkey current = _currentMonkey;
        while (current != null)
        {
            yield return current;
            if (!set.Add(current))
            {
                UnityEngine.Debug.LogError("Error : Infinite monkey chain detected !");
                yield break;
            }
            current = current.leftMonkey;
        }
    }
}