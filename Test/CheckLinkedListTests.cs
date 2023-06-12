﻿using NUnit.Framework;
using OnlineCompiler.Client.Pages;
using OnlineCompiler.Server.Handlers;

namespace test;

[TestFixture]
public class CheckLinkedListTests
{
    private static string _invalidLinkedListCode = @"
using System.Diagnostics;
using System.Runtime.Serialization;

namespace System.Collections.Generic
{
    public class LinkedList<T> : ICollection<T>, ICollection, IReadOnlyCollection<T>
    {
        internal LinkedListNode<T>? head;
        internal int count;
        internal int version;
        public LinkedList()
        {
        }

        public LinkedList(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new InvalidOperationException();
            }

            foreach (T item in collection)
            {
                AddLast(item);
            }
        }

        protected LinkedList(StreamingContext context)
        {
        }

        public int Count
        {
            get { return count; }
        }

        public LinkedListNode<T>? First
        {
            get { return head; }
        }

        public LinkedListNode<T>? Last
        {
            get { return head == null ? null : head.prev; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        void ICollection<T>.Add(T value)
        {
            AddLast(value);
        }

        public LinkedListNode<T> AddAfter(LinkedListNode<T> node, T value)
        {
            //Нужна реализация
            //T value = default!;
            //var item = new LinkedListNode<T>(value);
            return node;
        }

        public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            ValidateNode(node);
            ValidateNewNode(newNode);
            InternalInsertNodeBefore(node.next!, newNode);
            newNode.list = this;
        }

        public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value)
        {
            //Нужна реализация
            //throw new NotImplementedException();
            return node;
        }

        public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            ValidateNode(node);
            ValidateNewNode(newNode);
            InternalInsertNodeBefore(node, newNode);
            newNode.list = this;
            if (node == head)
            {
                head = newNode;
            }
        }

        public LinkedListNode<T> AddFirst(T value)
        {
            //Нужна реализация
            //T value = default!;
            var item = new LinkedListNode<T>(value);
            //throw new NotImplementedException();
            return item;
        }

        public void AddFirst(LinkedListNode<T> node)
        {
            ValidateNewNode(node);

            if (head == null)
            {
                InternalInsertNodeToEmptyList(node);
            }
            else
            {
                InternalInsertNodeBefore(head, node);
                head = node;
            }
            node.list = this;
        }

        public LinkedListNode<T> AddLast(T value)
        {
            LinkedListNode<T> result = new LinkedListNode<T>(this, value);
            if (head == null)
            {
                InternalInsertNodeToEmptyList(result);
            }
            else
            {
                InternalInsertNodeBefore(head, result);
            }
            return result;
        }

        public void AddLast(LinkedListNode<T> node)
        {
            ValidateNewNode(node);

            if (head == null)
            {
                InternalInsertNodeToEmptyList(node);
            }
            else
            {
                InternalInsertNodeBefore(head, node);
            }
            node.list = this;
        }

        public void Clear()
        {
            LinkedListNode<T>? current = head;
            while (current != null)
            {
                LinkedListNode<T> temp = current;
                current = current.Next;   // use Next the instead of next, otherwise it will loop forever
                temp.Invalidate();
            }

            head = null;
            count = 0;
            version++;
        }

        public bool Contains(T value)
        {
            return Find(value) != null;
        }

        public void CopyTo(T[] array, int index)
        {
            if (array == null)
            {
                throw new InvalidOperationException();
            }

            if (index < 0)
            {
                throw new InvalidOperationException();
            }

            if (index > array.Length)
            {
                throw new InvalidOperationException();
            }

            if (array.Length - index < Count)
            {
                throw new InvalidOperationException();
            }

            LinkedListNode<T>? node = head;
            if (node != null)
            {
                do
                {
                    array[index++] = node!.item;
                    node = node.next;
                } while (node != head);
            }
        }

        public LinkedListNode<T>? Find(T value)
        {
            //Нужна реализация
            //T value = default!;
            var item = new LinkedListNode<T>(value);
            //throw new NotImplementedException();
            //throw new NotImplementedException();
            return item;
        }

        public LinkedListNode<T>? FindLast(T value)
        {
            if (head == null) return null;

            LinkedListNode<T>? last = head.prev;
            LinkedListNode<T>? node = last;
            EqualityComparer<T> c = EqualityComparer<T>.Default;
            if (node != null)
            {
                if (value != null)
                {
                    do
                    {
                        if (c.Equals(node!.item, value))
                        {
                            return node;
                        }

                        node = node.prev;
                    } while (node != last);
                }
                else
                {
                    do
                    {
                        if (node!.item == null)
                        {
                            return node;
                        }
                        node = node.prev;
                    } while (node != last);
                }
            }
            return null;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Remove(T value)
        {
            //Нужна реализация
            //throw new NotImplementedException();
            return false;
        }

        public void Remove(LinkedListNode<T> node)
        {
            ValidateNode(node);
            InternalRemoveNode(node);
        }

        public void RemoveFirst()
        {
            if (head == null) { throw new InvalidOperationException(); }
            InternalRemoveNode(head);
        }

        public void RemoveLast()
        {
            if (head == null) { throw new InvalidOperationException(); }
            InternalRemoveNode(head.prev!);
        }

        private void InternalInsertNodeBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            newNode.next = node;
            newNode.prev = node.prev;
            node.prev!.next = newNode;
            node.prev = newNode;
            version++;
            count++;
        }

        private void InternalInsertNodeToEmptyList(LinkedListNode<T> newNode)
        {
       
            newNode.next = newNode;
            newNode.prev = newNode;
            head = newNode;
            version++;
            count++;
        }

        internal void InternalRemoveNode(LinkedListNode<T> node)
        {
 
            if (node.next == node)
            {

                head = null;
            }
            else
            {
                node.next!.prev = node.prev;
                node.prev!.next = node.next;
                if (head == node)
                {
                    head = node.next;
                }
            }
            node.Invalidate();
            count--;
            version++;
        }

        internal void ValidateNewNode(LinkedListNode<T> node)
        {
            if (node == null)
            {
                throw new InvalidOperationException();
            }

            if (node.list != null)
            {
                throw new InvalidOperationException();
            }
        }

        internal void ValidateNode(LinkedListNode<T> node)
        {
            if (node == null)
            {
                throw new InvalidOperationException();
            }

            if (node.list != this)
            {
                throw new InvalidOperationException();
            }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot => this;

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new InvalidOperationException();
            }

            if (array.Rank != 1)
            {
                throw new InvalidOperationException();
            }

            if (array.GetLowerBound(0) != 0)
            {
                throw new InvalidOperationException();
            }

            if (index < 0)
            {
                throw new InvalidOperationException();
            }

            if (array.Length - index < Count)
            {
                throw new InvalidOperationException();
            }

            T[]? tArray = array as T[];
            if (tArray != null)
            {
                CopyTo(tArray, index);
            }
            else
            {
                // No need to use reflection to verify that the types are compatible because it isn't 100% correct and we can rely
                // on the runtime validation during the cast that happens below (i.e. we will get an ArrayTypeMismatchException).
                object?[]? objects = array as object[];
                if (objects == null)
                {
                    throw new InvalidOperationException();
                }
                LinkedListNode<T>? node = head;
                try
                {
                    if (node != null)
                    {
                        do
                        {
                            objects[index++] = node!.item;
                            node = node.next;
                        } while (node != head);
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public struct Enumerator : IEnumerator<T>, IEnumerator, ISerializable, IDeserializationCallback
        {
            private readonly LinkedList<T> _list;
            private LinkedListNode<T>? _node;
            private readonly int _version;
            private T? _current;
            private int _index;

            internal Enumerator(LinkedList<T> list)
            {
                _list = list;
                _version = list.version;
                _node = list.head;
                _current = default;
                _index = 0;
            }

            public T Current => _current!;

            object? IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || (_index == _list.Count + 1))
                    {
                        throw new InvalidOperationException();
                    }

                    return Current;
                }
            }

            public bool MoveNext()
            {
                if (_version != _list.version)
                {
                    throw new InvalidOperationException();
                }

                if (_node == null)
                {
                    _index = _list.Count + 1;
                    return false;
                }

                ++_index;
                _current = _node.item;
                _node = _node.next;
                if (_node == _list.head)
                {
                    _node = null;
                }
                return true;
            }

            void IEnumerator.Reset()
            {
                if (_version != _list.version)
                {
                    throw new InvalidOperationException();
                }

                _current = default;
                _node = _list.head;
                _index = 0;
            }

            public void Dispose()
            {
            }

            void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
            {
                throw new PlatformNotSupportedException();
            }

            void IDeserializationCallback.OnDeserialization(object? sender)
            {
                throw new PlatformNotSupportedException();
            }
        }
    }

    // Note following class is not serializable since we customized the serialization of LinkedList.
    public sealed class LinkedListNode<T>
    {
        internal LinkedList<T>? list;
        internal LinkedListNode<T>? next;
        internal LinkedListNode<T>? prev;
        internal T item;

        public LinkedListNode(T value)
        {
            item = value;
        }

        internal LinkedListNode(LinkedList<T> list, T value)
        {
            this.list = list;
            item = value;
        }

        public LinkedList<T>? List
        {
            get { return list; }
        }

        public LinkedListNode<T>? Next
        {
            get { return next == null || next == list!.head ? null : next; }
        }

        public LinkedListNode<T>? Previous
        {
            get { return prev == null || this == list!.head ? null : prev; }
        }

        public T Value
        {
            get { return item; }
            set { item = value; }
        }

        /// <summary>Gets a reference to the value held by the node.</summary>
        public ref T ValueRef => ref item;

        internal void Invalidate()
        {
            list = null;
            next = null;
            prev = null;
        }
    }
}
";
    private static string _validLinkedListCode = TemplateLinkedList.LinkedListCode;

    [Test]
    public void TestLinkedList()
    {
        Assert.AreEqual(CodeCompileChecker<int>.CheckLinkedList(_validLinkedListCode, 8), true);
        Assert.AreEqual(CodeCompileChecker<int>.CheckLinkedList(_invalidLinkedListCode, 8), false);
    }

}
