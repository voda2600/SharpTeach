using System.Diagnostics;

namespace OnlineCompiler.Server.Handlers;

public static class CheckHashSet<T>
{
    public static bool CheckAdd(HashSet<T> set, Type type, object instance, T[] item, ref long addTime)
    {
        foreach (var a in item)
        {
            set.Add(a);
        }
        
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        foreach (var a in item)
        {
            type.GetMethod("Add", new[] {typeof(T)}).Invoke(instance, new object[] {a});
        }
        stopwatch.Stop();
        addTime = stopwatch.ElapsedTicks;

        return set.Count == (int)type.GetProperty("Count").GetValue(instance);
    }

    public static bool CheckRemove(HashSet<T> set, Type type, object instance, T[] item, ref long deleteTime)
    {
        foreach (var a in item)
        {
            set.Add(a);
        }
        foreach (var a in item)
        {
            type.GetMethod("Add", new[] {typeof(T)}).Invoke(instance, new object[] {a});
        }
        foreach (var a in item)
        {
            set.Remove(a);
        }
   
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        foreach (var a in item)
        {
            type.GetMethod("Remove", new[] { typeof(T) }).Invoke(instance, new object[] { a });
        }
        stopwatch.Stop();
        deleteTime = stopwatch.ElapsedTicks;

        return set.Count == (int)type.GetProperty("Count").GetValue(instance);
    }

    public static bool CheckClear(HashSet<T> set, Type type, object instance, T[] item)
    {
        set.Add(item.First());
        type.GetMethod("Add", new[] { typeof(T) }).Invoke(instance, new object[] { item.First() });
        set.Clear();
        type.GetMethod("Clear").Invoke(instance, null);

        return set.Count == (int)type.GetProperty("Count").GetValue(instance);
    }

    public static bool CheckContains(HashSet<T> set, Type type, object instance, T[] item, ref long findTime)
    {
        foreach (var a in item)
        {
            set.Add(a);
        }
        foreach (var a in item)
        {
            type.GetMethod("Add", new[] {typeof(T)}).Invoke(instance, new object[] {a});
        }

        bool originContains = set.Contains(item.First());

        var constructedContains = false;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        foreach (var a in item)
        {
            constructedContains = (bool)type.GetMethod("Contains").Invoke(instance, new object[] { a });
        }
        stopwatch.Stop();
        findTime = stopwatch.ElapsedTicks;
        return originContains == constructedContains;
    }

    public static bool CheckUnionSet(HashSet<T> set, Type type, object instance, HashSet<T> otherSet)
    {
        otherSet.Add(default(T));
        set.UnionWith(otherSet);

        type.GetMethod("UnionWith").Invoke(instance, new object[] { otherSet });

        return set.Count == (int)type.GetProperty("Count").GetValue(instance);
    }
}
