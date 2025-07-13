using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Models.Core;

/// <summary>
/// Coleção de lances da Lotofácil
/// </summary>
public class Lances : IEnumerable<Lance>
{
    private readonly List<Lance> _lances = new();

    public int Count => _lances.Count;
    public Lance this[int index] => _lances[index];

    public Lances()
    {
    }

    public Lances(IEnumerable<Lance> lances)
    {
        if (lances != null)
        {
            _lances.AddRange(lances);
        }
    }

    public void Add(Lance lance)
    {
        if (lance != null)
        {
            _lances.Add(lance);
        }
    }

    public void AddRange(IEnumerable<Lance> lances)
    {
        if (lances != null)
        {
            _lances.AddRange(lances);
        }
    }

    public bool Remove(Lance lance)
    {
        return _lances.Remove(lance);
    }

    public void Clear()
    {
        _lances.Clear();
    }

    public Lance GetByConcurso(int concurso)
    {
        return _lances.FirstOrDefault(l => l.Concurso == concurso);
    }

    public Lances GetRange(int startConcurso, int endConcurso)
    {
        var range = _lances.Where(l => l.Concurso >= startConcurso && l.Concurso <= endConcurso);
        return new Lances(range);
    }

    public Lances GetLast(int count)
    {
        var last = _lances.OrderByDescending(l => l.Concurso).Take(count);
        return new Lances(last);
    }

    public Lances TakeLast(int count)
    {
        return GetLast(count);
    }

    public List<int> GetAllNumbers()
    {
        return _lances.SelectMany(l => l.Dezenas).Distinct().OrderBy(n => n).ToList();
    }

    public Dictionary<int, int> GetFrequencyMap()
    {
        var frequency = new Dictionary<int, int>();
        
        for (int i = 1; i <= 25; i++)
        {
            frequency[i] = 0;
        }

        foreach (var lance in _lances)
        {
            foreach (var dezena in lance.Dezenas)
            {
                if (frequency.ContainsKey(dezena))
                {
                    frequency[dezena]++;
                }
            }
        }

        return frequency;
    }

    public bool Any()
    {
        return _lances.Any();
    }

    public Lance First()
    {
        return _lances.First();
    }

    public Lance FirstOrDefault()
    {
        return _lances.FirstOrDefault();
    }

    public Lance Last()
    {
        return _lances.Last();
    }

    public Lance LastOrDefault()
    {
        return _lances.LastOrDefault();
    }

    public IEnumerator<Lance> GetEnumerator()
    {
        return _lances.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public List<Lance> ToList()
    {
        return new List<Lance>(_lances);
    }

    public Lances OrderBy(Func<Lance, object> keySelector)
    {
        return new Lances(_lances.OrderBy(keySelector));
    }

    public Lances OrderByDescending(Func<Lance, object> keySelector)
    {
        return new Lances(_lances.OrderByDescending(keySelector));
    }

    public Lances Where(Func<Lance, bool> predicate)
    {
        return new Lances(_lances.Where(predicate));
    }
}