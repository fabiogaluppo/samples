//Sample provided by Fabio Galuppo
//February 2013

using System;
using System.Collections.Generic;

public sealed class Node<T>
{
    public Node(T value, Node<T> left = null, Node<T> right = null)
    {
        Value = value;
        Left = left;
        Right = right;
    }
    
    public T Value { get; private set; }
    public Node<T> Left { get; private set; }
    public Node<T> Right { get; private set; }
}

public static class TreeTraversal
{
    private static void Enqueue<T>(Queue<Node<T>> q, Node<T> n)
    {
        if (null != n) q.Enqueue(n);
    }

    public static IEnumerable<T> AsBreadthFirst<T>(Node<T> node)
    {
        var q = new Queue<Node<T>>();
        Enqueue(q, node);
        while (q.Count > 0)
        {
            var current = q.Dequeue();
            yield return current.Value;
            Enqueue(q, current.Left);
            Enqueue(q, current.Right);
        }
    }

    //test case
    static void Main(string[] args)
    {
        var c = new Node<char>('C');
        var e = new Node<char>('E');
        var h = new Node<char>('H');
        var a = new Node<char>('A');
        var d = new Node<char>('D', c, e);
        var i = new Node<char>('I', h);
        var b = new Node<char>('B', a, d);
        var g = new Node<char>('G', right: i);
        var f = new Node<char>('F', b, g);

        Console.Write("Level order traversal sequence: ");
        foreach (var x in TreeTraversal.AsBreadthFirst(f))
            Console.Write(x + " ");
        Console.WriteLine();
    }
}