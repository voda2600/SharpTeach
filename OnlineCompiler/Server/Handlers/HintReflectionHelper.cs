﻿using System.Reflection;

namespace OnlineCompiler.Server.Handlers;

public static class HintReflectionHelper
{
    private static string GetUserCode(string a, string b, string code)
    {
        int length = code.IndexOf(b) - code.IndexOf(a);
        var userMethodCode =
            code.Substring(code.IndexOf(a), length);
        return userMethodCode;
    }

    public static void GetReflectionHintsDictionary(string code, Type constructedType, List<string> hints)
    {
        //TryInsertTest
        var insertMethod = constructedType.GetMethod("TryInsert",
            BindingFlags.NonPublic | BindingFlags.Instance);
        var insertBody = insertMethod.GetMethodBody().LocalVariables;
        if (!insertBody.Any(x =>
                x.LocalType.ToString().Contains("Dictionary`2+Entry[System.String,System.String][]")))
        {
            hints.Add(
                "Метод TryInsert: Попробуйте добавить в код использование Entry[], присвоив значение _entries");
        }

        if (!insertBody.Any(x =>
                x.LocalType.ToString()
                    .Contains("System.Collections.Generic.IEqualityComparer`1[System.String]")))
        {
            hints.Add(
                "Метод TryInsert: Попробуйте добавить в код использование IEqualityComparer<TKey>, присвоив значение _comparer. После чего вычислять по нему GetHashCode()");
        }

        var userMethodCode = GetUserCode(
            "private bool TryInsert(TKey key, TValue value, InsertionBehavior behavior)",
            "private void Resize() => Resize(ExpandPrime(_count), false);", code);

        if (!userMethodCode.Contains("if (key == null)") && !userMethodCode.Contains("if (key is null)"))
        {
            hints.Add("Метод TryInsert: Добавьте проверку key на null в начале метода");
        }

        if (!userMethodCode.Contains("ref GetBucket("))
        {
            hints.Add(
                "Метод TryInsert: Попробуйте использовать метод GetBucket, передавая в него hashCode. Важно, что GetBucket нужно вызывать с ref");
        }

        if (!userMethodCode.Contains("GetHashCode("))
        {
            hints.Add("Метод TryInsert: В методе необходимо использовать метод GetHashCode()");
        }

        if (!userMethodCode.Contains("Resize()"))
        {
            hints.Add("Метод TryInsert: В методе необходимо использовать метод Resize()");
        }

        //Remove test
        var removeMethod = constructedType.GetMethod("Remove");
        var removeBody = removeMethod.GetMethodBody().LocalVariables;

        if (!removeBody.Any(x =>
                x.LocalType.ToString().Contains("Dictionary`2+Entry[System.String,System.String][]")))
        {
            hints.Add(
                "Метод Remove: Попробуйте добавить в код использование Entry[], присвоив значение _entries");
        }

        userMethodCode = GetUserCode("public bool Remove(TKey key)",
            "public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)", code);

        if (!userMethodCode.Contains("ref GetBucket("))
        {
            hints.Add(
                "Метод Remove: Попробуйте использовать метод GetBucket, передавая в него hashCode. Важно, что GetBucket нужно вызывать с ref");
        }

        if (!userMethodCode.Contains("StartOfFreeList"))
        {
            hints.Add(
                "Метод Remove:  Попробуйте добавить использование StartOfFreeList, для вычисления  entry.next");
        }

        if (!userMethodCode.Contains("uint"))
        {
            hints.Add(
                "Метод Remove: Для хранения hashCode и количества коллизий необходимо использовать uint");
        }

        //FindTest
        var findMethod = constructedType.GetMethod("FindValue",
            BindingFlags.NonPublic | BindingFlags.Instance);

        var findBody = findMethod.GetMethodBody().LocalVariables;

        if (!findBody.Any(x =>
                x.LocalType.ToString().Contains("Dictionary`2+Entry[System.String,System.String]")))
        {
            hints.Add("Метод FindValue: Для работы с Entry используйте ref Unsafe.NullRef<Entry>()");
        }

        if (!findBody.Any(x => x.LocalType.ToString().Contains("System.UInt32")))
        {
            hints.Add(
                "Метод FindValue: Для хранения hashCode и количества коллизий необходимо использовать uint");
        }

        userMethodCode = GetUserCode("private ref TValue FindValue(TKey key)",
            "private static int GetPrime(int min)", code);

        if (!userMethodCode.Contains("GetHashCode("))
        {
            hints.Add("Метод FindValue: В методе необходимо использовать метод GetHashCode()");
        }

        if (!userMethodCode.Contains("IsValueType"))
        {
            hints.Add("Метод FindValue: Необходимо обрабатывать ValueType, используя IsValueType");
        }

        if (!userMethodCode.Contains("_comparer"))
        {
            hints.Add("Метод FindValue: Используйте _comparer для сравнения значений");
        }
    }

    public static void GetReflectionHintsHashSet(string code, Type constructedType, List<string> hints)
    {
        //AddTest
        var insertMethod = constructedType.GetMethod("AddIfNotPresent",
            BindingFlags.NonPublic | BindingFlags.Instance);
        var insertBody = insertMethod.GetMethodBody().LocalVariables;
        if (!insertBody.Any(x =>
                x.LocalType.ToString().Contains("System.Collections.Generic.HashSet`1+Entry[System.String]")))
        {
            hints.Add(
                "Метод AddIfNotPresent: Попробуйте добавить в код использование Entry[], присвоив значение _entries");
        }

        if (!insertBody.Any(x =>
                x.LocalType.ToString().Contains("System.Collections.Generic.IEqualityComparer`1[System.String]")))
        {
            hints.Add(
                "Метод AddIfNotPresent: Попробуйте добавить в код использование IEqualityComparer<T>, присвоив значение _comparer. После чего вычислять по нему GetHashCode(). При null в _comparer код должен продолжать работать.");
        }

        var userMethodCode = GetUserCode("private bool AddIfNotPresent(T value, out int location)",
            "private static IEqualityComparer<string?> GetUnderlyingEqualityComparer(IEqualityComparer<string?>? outerComparer)",
            code);

        if (!userMethodCode.Contains("Initialize(0)"))
        {
            hints.Add("Метод AddIfNotPresent: Добавьте проверку _buckets на null. И сделайте Initialize, при null");
        }

        if (!userMethodCode.Contains("ref"))
        {
            hints.Add(
                "Метод AddIfNotPresent: С buckets нужно работать при помощи ref. Попробуйте использовать GetBucketRef");
        }

        if (!userMethodCode.Contains("GetHashCode("))
        {
            hints.Add("Метод AddIfNotPresent: В методе необходимо использовать метод GetHashCode()");
        }

        if (!userMethodCode.Contains(".IsValueType"))
        {
            hints.Add(
                "Метод AddIfNotPresent: Типы значений никогда не вызывают rehash. Но нужно обработать collision threshold для ссылочных типов.");
        }

        if (!userMethodCode.Contains("Resize("))
        {
            hints.Add(
                "Метод AddIfNotPresent: Попробуйте использовать Resize() и FindItemIndex() при решении вопроса с коллизией");
        }

        //Remove test
        var removeMethod = constructedType.GetMethod("Remove");
        var removeBody = removeMethod.GetMethodBody().LocalVariables;

        if (!removeBody.Any(x =>
                x.LocalType.ToString().Contains("System.Collections.Generic.HashSet`1+Entry[System.String]")))
        {
            hints.Add("Метод Remove: Попробуйте добавить в код использование Entry[], присвоив значение _entries");
        }

        userMethodCode = GetUserCode("public bool Remove(T item)",
            "public int Count => _count - _freeCount;", code);

        if (!userMethodCode.Contains(".GetHashCode("))
        {
            hints.Add("Метод Remove: При работе со значением, постарайтесь использовать .GetHashCode()");
        }

        if (!userMethodCode.Contains("ref GetBucketRef("))
        {
            hints.Add("Метод Remove: При работе с bucket необходимо использовать ref GetBucketRef()");
        }

        if (!userMethodCode.Contains("EqualityComparer<T>.Default"))
        {
            hints.Add("Метод Remove: Если comparer null, то используйте EqualityComparer<T>.Default");
        }

        if (!userMethodCode.Contains(".Next"))
        {
            hints.Add("Метод Remove: Обратите внимание на структуру Entries и свойство entry.Next");
        }

        if (!userMethodCode.Contains("collis"))
        {
            hints.Add("Метод Remove: Обратите внимание на обработку коллизий");
        }

        //FindTest
        var findMethod = constructedType.GetMethod("FindItemIndex",
            BindingFlags.NonPublic | BindingFlags.Instance);

        var findBody = findMethod.GetMethodBody().LocalVariables;

        if (!findBody.Any(x => x.LocalType.ToString().Contains("System.Int32[]")))
        {
            hints.Add("Метод FindItemIndex: Для работы с buckets рекомендуется создать локальную переменную");
        }

        if (!findBody.Any(x =>
                x.LocalType.ToString().Contains("System.Collections.Generic.IEqualityComparer`1[System.String]")))
        {
            hints.Add("Метод FindItemIndex: Для работы с comparer рекомендуется создать локальную переменную");
        }

        userMethodCode = GetUserCode("private int FindItemIndex(T item)",
            "private ref int GetBucketRef(int hashCode)", code);

        if (!userMethodCode.Contains("GetHashCode("))
        {
            hints.Add("Метод FindItemIndex: В методе необходимо использовать метод GetHashCode()");
        }

        if (!userMethodCode.Contains("IsValueType"))
        {
            hints.Add("Метод FindItemIndex: Необходимо обрабатывать ValueType, используя IsValueType");
        }

        if (!userMethodCode.Contains("GetBucketRef("))
        {
            hints.Add("Метод FindItemIndex: При работе с bucket необходимо использовать GetBucketRef()");
        }

        if (!userMethodCode.Contains("return -1"))
        {
            hints.Add("Метод FindItemIndex: При отсутствии вхождения элемента - необходимо возвращать -1");
        }
    }

    public static void GetReflectionHintsLinkedList(string code, Type constructedType, List<string> hints)
    {
        //AddTest
        var addFirstMethod = constructedType
            .GetMethods()
            .First(mi => mi.Name == "AddFirst" && mi.ReturnType != typeof(void));
        var insertBody = addFirstMethod.GetMethodBody().LocalVariables;
        if (!insertBody.Any(x =>
                x.LocalType.ToString().Contains("System.Collections.Generic.LinkedListNode`1[System.String]")))
        {
            hints.Add("Метод AddFirst: Попробуйте использовать LinkedListNode");
        }

        var userMethodCode = GetUserCode("public LinkedListNode<T> AddFirst(T value)",
            "public void AddFirst(LinkedListNode<T> node)",
            code);

        if (!userMethodCode.Contains("head == null") && !userMethodCode.Contains("head is null"))
        {
            hints.Add("Метод AddFirst: Добавьте проверку head на null");
        }

        if (!userMethodCode.Contains("InternalInsertNodeToEmptyList"))
        {
            hints.Add(
                "Метод AddFirst: Попробуйте использовать InternalInsertNodeToEmptyList");
        }

        var addFirstMethod1 = constructedType
            .GetMethods()
            .First(mi => mi.Name == "AddBefore" && mi.ReturnType != typeof(void));
        var insertBody1 = addFirstMethod1.GetMethodBody().LocalVariables;

        if (!insertBody1.Any(x =>
                x.LocalType.ToString().Contains("System.Collections.Generic.LinkedListNode`1[System.String]")))
        {
            hints.Add("Метод AddBefore: Попробуйте использовать LinkedListNode");
        }

        var userMethodCode1 = GetUserCode("public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value)",
            "public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)",
            code);

        if (!userMethodCode1.Contains("ValidateNode("))
        {
            hints.Add("Метод AddBefore: Добавьте проверку ValidateNode()");
        }

        if (!userMethodCode1.Contains("InternalInsertNodeBefore"))
        {
            hints.Add("Метод AddBefore: Попробуйте использовать InternalInsertNodeToEmptyList");
        }

        if (!userMethodCode1.Contains("node == head"))
        {
            hints.Add("Метод AddBefore: Добавьте проверку того, что node == head, с присвоением result к head");
        }

        var addFirstMethod2 = constructedType
            .GetMethods()
            .First(mi => mi.Name == "AddAfter" && mi.ReturnType != typeof(void));
        var insertBody2 = addFirstMethod2.GetMethodBody().LocalVariables;
        if (!insertBody2.Any(x =>
                x.LocalType.ToString().Contains("System.Collections.Generic.LinkedListNode")))
        {
            hints.Add("Метод AddAfter: Попробуйте использовать LinkedListNode");
        }

        var userMethodCode2 = GetUserCode("public LinkedListNode<T> AddAfter(LinkedListNode<T> node, T value)",
            "public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode)",
            code);
        if (!userMethodCode2.Contains("ValidateNode("))
        {
            hints.Add("Метод AddAfter: Добавьте проверку ValidateNode() для node");
        }

        if (!userMethodCode2.Contains("InternalInsertNodeBefore"))
        {
            hints.Add("Метод AddAfter: Попробуйте использовать InternalInsertNodeToEmptyList");
        }

        //Remove test
        userMethodCode = GetUserCode("public bool Remove(T value)",
            "public void Remove(LinkedListNode<T> node)", code);

        if (!userMethodCode.Contains("Find("))
        {
            hints.Add("Метод Remove: Необходимо использовать метод Find() для значения value");
        }

        if (!userMethodCode.Contains("InternalRemoveNode("))
        {
            hints.Add("Метод Remove: Добавьте использование метода InternalRemoveNode()");
        }

        if (!userMethodCode.Contains("node != null") && !userMethodCode.Contains("node is not null"))
        {
            hints.Add("Метод Remove: Добавьте проверку node на null");
        }

        //FindTest
        var findMethod = constructedType.GetMethod("Find");

        var findBody = findMethod.GetMethodBody().LocalVariables;

        if (!findBody.Any(x => x.LocalType.ToString().Contains("System.Collections.Generic.LinkedListNode")))
        {
            hints.Add("Метод Find: Используйте LinkedListNode с присвоением в него head");
        }

        if (!findBody.Any(x =>
                x.LocalType.ToString().Contains("System.Collections.Generic.EqualityComparer`1[System.String]")))
        {
            hints.Add("Метод Find: Для значения comparer используйте EqualityComparer<T>.Default");
        }

        userMethodCode = GetUserCode("public LinkedListNode<T>? Find(T value)",
            "public LinkedListNode<T>? FindLast(T value)", code);

        if (!userMethodCode.Contains(".Equals("))
        {
            hints.Add("Метод Find: Для сравнения значений value и node.item используйте Equals()");
        }

        if (!userMethodCode.Contains("return null"))
        {
            hints.Add("Метод Find: Если значение не было найдено, возвращайте null");
        }

        if (!userMethodCode.Contains("node != null") && !userMethodCode.Contains("node is not null"))
        {
            hints.Add("Метод Find: Добавьте проверку node на null");
        }

        if (!userMethodCode.Contains(".next"))
        {
            hints.Add("Метод Find: Для обработки всех значений node воспользуйтесь node.next");
        }
    }

    public static void GetReflectionHintsList(string code, Type constructedType, List<string> hints)
    {
        //AddTest
        var addFirstMethod = constructedType.GetMethod("Add");
        var insertBody = addFirstMethod.GetMethodBody().LocalVariables;
        if (!insertBody.Any(x =>
                x.LocalType.ToString().Contains("System.String[]")))
        {
            hints.Add("Метод Add: Рекомендуется использовать _items, присвоив значение в локальную переменную");
        }

        if (!insertBody.Any(x =>
                x.LocalType.ToString().Contains("System.Int32")))
        {
            hints.Add("Метод Add: Рекомендуется использовать _size, присвоив значение в локальную переменную");
        }

        var userMethodCode = GetUserCode("public void Add(T item)",
            "private void AddWithResize(T item)",
            code);

        if (!userMethodCode.Contains("uint"))
        {
            hints.Add("Метод Add: Используйте каст к uint при сравнении size");
        }

        if (!userMethodCode.Contains("_version"))
        {
            hints.Add("Метод Add: Обратите внимание на _version, необходимо увеличивать его значение");
        }

        if (!userMethodCode.Contains("AddWithResize"))
        {
            hints.Add(
                "Метод Add: При вставке элемента необходимо проверять _size и при необходимости вызывать метод AddWithResize()");
        }

        //Remove test
        userMethodCode = GetUserCode("public void RemoveAt(int index)",
            "public struct Enumerator : IEnumerator<T>, IEnumerator", code);

        if (!userMethodCode.Contains("uint"))
        {
            hints.Add("Метод RemoveAt: Используйте каст к uint при сравнении size и работой с index");
        }

        if (!userMethodCode.Contains("Array.Copy"))
        {
            hints.Add("Метод RemoveAt: Используйте Array.Copy, если index меньше _size");
        }

        if (!userMethodCode.Contains("IsReferenceOrContainsReferences"))
        {
            hints.Add(
                "Метод RemoveAt: Попробуйте добавить использование RuntimeHelpers.IsReferenceOrContainsReferences");
        }

        if (!userMethodCode.Contains("_version"))
        {
            hints.Add("Метод RemoveAt: Обратите внимание на _version, необходимо увеличивать его значение");
        }

        //FindTest
        userMethodCode = GetUserCode("public T? Find(Predicate<T> match)",
            "public List<T> FindAll(Predicate<T> match)", code);

        if (!userMethodCode.Contains("match == null") && !userMethodCode.Contains("match is null"))
        {
            hints.Add("Метод Find: Обработайте случай, когда match равен null");
        }

        if (!userMethodCode.Contains("_size"))
        {
            hints.Add("Метод Find: Для поиска значений рекомендуется использовать for с 0 элемента до _size");
        }

        if (!userMethodCode.Contains("match("))
        {
            hints.Add("Метод Find: Попробуйте использовать предикат match при работе с элементом List");
        }

        if (!userMethodCode.Contains("default"))
        {
            hints.Add("Метод Find: При отсутствии элемента, рекомендуется вернуть default");
        }
    }

    public static void GetReflectionHintsQueue(string code, Type constructedType, List<string> hints)
    {
        //AddTest
        var userMethodCode = GetUserCode("public void Enqueue(T item)",
            "internal static T[] ToArray<T>(IEnumerable<T> source, out int length)",
            code);

        if (!userMethodCode.Contains("_size"))
        {
            hints.Add("Метод Enqueue: Добавить проверку _size с длинной масива");
        }

        if (!userMethodCode.Contains("Grow("))
        {
            hints.Add("Метод Enqueue: Используйте Grow() для увеличения _size массива");
        }

        if (!userMethodCode.Contains("MoveNext("))
        {
            hints.Add("Метод Enqueue: Сместите указатель tail с помощью метода MoveNext(), используя ref");
        }

        //Remove test
        var removeMethod = constructedType.GetMethod("Dequeue");
        var removeBOdy = removeMethod.GetMethodBody().LocalVariables;
        if (!removeBOdy.Any(x =>
                x.LocalType.ToString().Contains("System.String[]")))
        {
            hints.Add("Метод Dequeue: Рекомендуется использовать _array, присвоив значение в локальную переменную");
        }

        if (!removeBOdy.Any(x =>
                x.LocalType.ToString().Contains("System.Int32")))
        {
            hints.Add("Метод Dequeue: Рекомендуется использовать _head, присвоив значение в локальную переменную");
        }

        userMethodCode = GetUserCode("public T Dequeue()",
            "public bool TryDequeue([MaybeNullWhen(false)] out T result)", code);

        if (!userMethodCode.Contains("IsReferenceOrContainsReferences"))
        {
            hints.Add("Метод Dequeue: Добавьте проверку RuntimeHelpers.IsReferenceOrContainsReferences");
        }

        if (!userMethodCode.Contains("ThrowForEmptyQueue"))
        {
            hints.Add("Метод Dequeue: При пустои queue рекомендуется использовать ThrowForEmptyQueue()");
        }

        if (!userMethodCode.Contains("MoveNext"))
        {
            hints.Add("Метод Dequeue: Сместите указатель _head при помощи MoveNext()");
        }
    }

    public static void GetReflectionHintsSortedList(string code, Type constructedType, List<string> hints)
    {
        //AddTest
        var userMethodCode = GetUserCode("public void Add(TKey key, TValue value)",
            "void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)",
            code);

        if (!userMethodCode.Contains("key == null") && !userMethodCode.Contains("key is null"))
        {
            hints.Add("Метод Add: Добавить проверку key на null");
        }

        if (!userMethodCode.Contains("Array.BinarySearch"))
        {
            hints.Add("Метод Add: Для поиска по keys используйте Array.BinarySearch");
        }

        if (!userMethodCode.Contains("Insert("))
        {
            hints.Add("Метод Add: Для вставки элемента рекомендуется воспользоваться Insert()");
        }

        //Remove test

        userMethodCode = GetUserCode("public void RemoveAt(int index)",
            "public bool Remove(TKey key)", code);

        if (!userMethodCode.Contains("_size"))
        {
            hints.Add(
                "Метод RemoveAt: Добавьте обработку _size. Необходимо проверять выход за пределы массива и уменьшать _size при удалении");
        }

        if (!userMethodCode.Contains("Array.Copy"))
        {
            hints.Add("Метод RemoveAt: Если index меньше _size рекомендуется использовать Array.Copy");
        }

        if (!userMethodCode.Contains("IsReferenceOrContainsReferences"))
        {
            hints.Add("Метод RemoveAt: Добавьте проверку с условием RuntimeHelpers.IsReferenceOrContainsReferences");
        }

        if (!userMethodCode.Contains("version"))
        {
            hints.Add("Метод RemoveAt: Необходимо увеличивать значение version");
        }


        //FindTest IndexOfKey
        userMethodCode = GetUserCode("public int IndexOfKey(TKey key)",
            "public int IndexOfValue(TValue value)", code);

        if (!userMethodCode.Contains("BinarySearch"))
        {
            hints.Add("Метод IndexOfKey: Для поиска значений рекомендуется использовать Array.BinarySearch");
        }
    }

    public static void GetReflectionHintsStack(string code, Type constructedType, List<string> hints)
    {
        //AddTest
        var userMethodCode = GetUserCode("public void Push(T item)",
            "private void PushWithResize(T item)",
            code);

        if (!userMethodCode.Contains("PushWithResize"))
        {
            hints.Add("Метод Push: Добавить использоваие PushWithResize при нехватке размера массива");
        }

        if (!userMethodCode.Contains("uint"))
        {
            hints.Add("Метод Push: Для работы с size необходим каст к uint");
        }

        if (!userMethodCode.Contains("_version"))
        {
            hints.Add("Метод Push: Необходимо обрабатывать _version");
        }
        
        if (!userMethodCode.Contains("_array"))
        {
            hints.Add("Метод Push: Используйте _array для вставки");
        }

        //Remove test

        userMethodCode = GetUserCode("public T Pop()",
            "public bool TryPop(out T result)", code);
        
        if (!userMethodCode.Contains("_version"))
        {
            hints.Add("Метод Pop: Необходимо обрабатывать _version");
        }

        if (!userMethodCode.Contains("ThrowForEmptyStack"))
        {
            hints.Add(
                "Метод Pop: Для обработки исключения пустого Stack используйте ThrowForEmptyStack()");
        }

        if (!userMethodCode.Contains("IsReferenceOrContainsReferences"))
        {
            hints.Add("Метод Pop: Добавьте проверку с использованием RuntimeHelpers.IsReferenceOrContainsReferences");
        }

        if (!userMethodCode.Contains("default"))
        {
            hints.Add("Метод Pop: Для удаления элемента рекомендуется использовать присвоение default");
        }
    }
}