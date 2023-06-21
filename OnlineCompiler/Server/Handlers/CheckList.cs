using System.Diagnostics;

namespace OnlineCompiler.Server.Handlers;

public static class CheckList<T>
{
    public static bool CheckAdd(List<T> list, Type type, object instance, T[] item,  ref long addTime)
    {
        foreach (var a in item)
        {
            list.Add(a);
        }
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        foreach (var a in item)
        {
            type.GetMethod("Add").Invoke(instance, new object[] { a });
        }
        stopwatch.Stop();
        addTime = stopwatch.ElapsedTicks;
        return list.Count == (int)type.GetProperty("Count").GetValue(instance);
    }

    public static bool CheckRemove(List<T> list, Type type, object instance, T[] item, ref long deleteTime)
    {
        foreach (var a in item)
        {
            list.Add(a);
        }
        foreach (var a in item)
        {
            type.GetMethod("Add").Invoke(instance, new object[] { a });
        }
        foreach (var a in item)
        {
            list.Remove(a);
        }

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        foreach (var a in item)
        {
            type.GetMethod("Remove").Invoke(instance, new object[] { a });
        }
        stopwatch.Stop();
        deleteTime = stopwatch.ElapsedTicks;

        return list.Count == (int)type.GetProperty("Count").GetValue(instance);
    }

    public static bool CheckInsert(List<T> list, Type type, object instance, T[] item)
    {
        list.Insert(0, item.First());
        type.GetMethod("Insert").Invoke(instance, new object[] { 0, item.First() });

        var origin = list[0];
        var constructed = (T)type.GetProperty("Item").GetValue(instance, new object[] { 0 });

        return origin.Equals(constructed);
    }

    public static bool CheckClear(List<T> list, Type type, object instance, T[] item)
    {
        list.Add(item.First());
        type.GetMethod("Add").Invoke(instance, new object[] { item.First() });
        list.Clear();
        type.GetMethod("Clear").Invoke(instance, null);

        return list.Count == (int)type.GetProperty("Count").GetValue(instance);
    }

    public static bool CheckContains(List<T> list, Type type, object instance, T[] item, ref long findTime)
    {
        foreach (var a in item)
        {
            list.Add(a);
        }
        foreach (var a in item)
        {
            type.GetMethod("Add").Invoke(instance, new object[] { a });
        }
        
        bool originContains = false;
        foreach (var a in item)
        {
            originContains = list.Contains(a);
        }

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        bool constructedContains = false;
        foreach (var a in item)
        {
            constructedContains = (bool)type.GetMethod("Contains").Invoke(instance, new object[] { a });
        }
        stopwatch.Stop();
        findTime = stopwatch.ElapsedTicks;

        return originContains == constructedContains;
    }
}

