using System;
using System.Collections.Generic;

public class UnorderedPairDictionary<T> where T : notnull
{
    private readonly Dictionary<T, T> _forward = new();
    private readonly Dictionary<T, T> _reverse = new();
    private readonly HashSet<UnorderedPair<T>> _pairs = new();

    private struct UnorderedPair<TKey> : IEquatable<UnorderedPair<TKey>>
    {
        public TKey First { get; }
        public TKey Second { get; }

        public UnorderedPair(TKey first, TKey second)
        {
            // Сортируем, чтобы обеспечить "неупорядоченность"
            if (Comparer<TKey>.Default.Compare(first, second) <= 0)
            {
                First = first;
                Second = second;
            }
            else
            {
                First = second;
                Second = first;
            }
        }

        public bool Equals(UnorderedPair<TKey> other) =>
            EqualityComparer<TKey>.Default.Equals(First, other.First) &&
            EqualityComparer<TKey>.Default.Equals(Second, other.Second);

        public override bool Equals(object obj) => obj is UnorderedPair<TKey> pair && Equals(pair);

        public override int GetHashCode() =>
            HashCode.Combine(First, Second);
    }

    public void Add(T a, T b)
    {
        var pair = new UnorderedPair<T>(a, b);
        if (_pairs.Contains(pair))
            return;

        // Удаляем старые связи
        Remove(a);
        Remove(b);

        // Добавляем новые
        _forward[a] = b;
        _reverse[b] = a;
        _pairs.Add(pair);
    }

    public bool Remove(T key)
    {
        if (_forward.TryGetValue(key, out var value))
        {
            _forward.Remove(key);
            _reverse.Remove(value);
            _pairs.Remove(new UnorderedPair<T>(key, value));
            return true;
        }

        if (_reverse.TryGetValue(key, out var key2))
        {
            _reverse.Remove(key);
            _forward.Remove(key2);
            _pairs.Remove(new UnorderedPair<T>(key, key2));
            return true;
        }

        return false;
    }

    public bool TryGetValue(T key, out T value)
    {
        if (_forward.TryGetValue(key, out value))
            return true;

        return _reverse.TryGetValue(key, out value);
    }

    public bool ContainsPair(T a, T b) =>
        _pairs.Contains(new UnorderedPair<T>(a, b));
}