using System.Diagnostics;

namespace OnlineCompiler.Server.Handlers;

public static class CheckLinkedList<T>
{
    public static bool CheckAddLast(LinkedList<T> list, Type type, object instance, T[] item, ref long addTime)
    {
        foreach (var a in item)
        {
            list.AddLast(a);
        }
        
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        foreach (var a in item)
        {
            type.GetMethod("AddLast", new[] { typeof(T) }).Invoke(instance, new object[] { a });
        }
        stopwatch.Stop();
        addTime = stopwatch.ElapsedTicks;
        
        return list.Count == (int)type.GetProperty("Count").GetValue(instance);
    }

    public static bool CheckAddFirst(LinkedList<T> list, Type type, object instance, T[] item)
    {
        list.AddFirst(item.First());
        type.GetMethod("AddLast", new[] { typeof(T) }).Invoke(instance, new object[] { item.First() });

        return list.Count == (int)type.GetProperty("Count").GetValue(instance);
    }

    public static bool CheckRemove(LinkedList<T> list, Type type, object instance, T[] item, ref long deleteTime)
    {
        foreach (var a in item)
        {
            list.AddLast(a);
        }
        //type.GetMethod("AddLast").Invoke(instance, new object[] { item });
        foreach (var a in item)
        {
            type.GetMethod("AddLast", new[] { typeof(T) }).Invoke(instance, new object[] { a });
        }
        
        foreach (var a in item)
        {
            list.Remove(a);
        }
        
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        foreach (var a in item)
        {
            type.GetMethod("Remove", new[] { typeof(T) }).Invoke(instance, new object[] { a });
        }
        stopwatch.Stop();
        deleteTime = stopwatch.ElapsedTicks;
        
        return list.Count == (int)type.GetProperty("Count").GetValue(instance);
    }

    public static bool CheckClear(LinkedList<T> list, Type type, object instance, T[] item)
    {
        list.AddLast(item.First());
        type.GetMethod("AddLast", new[] { typeof(T) }).Invoke(instance, new object[] { item.First() });
        list.Clear();
        type.GetMethod("Clear").Invoke(instance, null);

        return list.Count == (int)type.GetProperty("Count").GetValue(instance);
    }

    public static bool CheckContains(LinkedList<T> list, Type type, object instance, T[] item, ref long findTime)
    {
        foreach (var a in item)
        {
            list.AddLast(a);
        }
        foreach (var a in item)
        {
            type.GetMethod("AddLast", new[] { typeof(T) }).Invoke(instance, new object[] { a });
        }

        bool originContains = list.Contains(item.First());

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
}

