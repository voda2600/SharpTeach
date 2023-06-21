using System.Diagnostics;

namespace OnlineCompiler.Server.Handlers;

public static class CheckStack<T>
{
    public static bool CheckPush(Stack<T> stack, Type type, object instance, T[] item, ref long addTime)
    {
        foreach (var a in item)
        {
            stack.Push(a);
        }

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        foreach (var a in item)
        {
            type.GetMethod("Push").Invoke(instance, new object[] {a});
        }

        stopwatch.Stop();
        addTime = stopwatch.ElapsedTicks;


        var origin = stack.Pop();
        var constructed = (T) type.GetMethod("Pop").Invoke(instance, null);

        return origin.Equals(constructed);
    }

    public static bool CheckPeek(Stack<T> stack, Type type, object instance, T[] item, ref long deleteTime)
    {
        foreach (var a in item)
        {
            stack.Push(a);
        }

        foreach (var a in item)
        {
            type.GetMethod("Push").Invoke(instance, new object[] {a});
        }

        var origin = stack.Peek();
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        var constructed = (T) type.GetMethod("Peek").Invoke(instance, null);
        stopwatch.Stop();
        deleteTime = stopwatch.ElapsedTicks;

        return origin.Equals(constructed);
    }

    public static bool CheckTryPeek(Stack<T> stack, Type type, object instance, T[] item)
    {
        stack.Push(item.First());
        type.GetMethod("Push").Invoke(instance, new object[] {item.First()});

        bool originSuccess = stack.TryPeek(out T originResult);
        bool constructedSuccess = (bool) type.GetMethod("TryPeek").Invoke(instance, new object[] {null});

        return originSuccess == constructedSuccess;
    }

    public static bool CheckClear(Stack<T> stack, Type type, object instance, T[] item)
    {
        stack.Push(item.First());
        type.GetMethod("Push").Invoke(instance, new object[] {item.First()});
        stack.Clear();
        type.GetMethod("Clear").Invoke(instance, null);

        return stack.Count == (int) type.GetProperty("Count").GetValue(instance);
    }

    public static bool CheckContains(Stack<T> stack, Type type, object instance, T[] item, ref long findTime)
    {
        foreach (var a in item)
        {
            stack.Push(a);
        }
        foreach (var a in item)
        {
            type.GetMethod("Push").Invoke(instance, new object[] {a});
        }

        bool originContains = stack.Contains(item);
        bool constructedContains = (bool) type.GetMethod("Contains").Invoke(instance, new object[] {item});

        return originContains == constructedContains;
    }

    public static bool CheckToArray(Stack<T> stack, Type type, object instance, T[] item)
    {
        stack.Push(item);
        type.GetMethod("Push").Invoke(instance, new object[] {item});

        var originArray = stack.ToArray();
        var constructedArray = (T[]) type.GetMethod("ToArray").Invoke(instance, null);

        return originArray.SequenceEqual(constructedArray);
    }
}