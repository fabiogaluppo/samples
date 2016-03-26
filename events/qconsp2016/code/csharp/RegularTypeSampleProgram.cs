//Sample provided by Fabio Galuppo  
//March 2016 

using System;
using System.Diagnostics.Contracts;

static class Util
{
    public static void Swap<T>(ref T lhs, ref T rhs) where T : struct
    {
        T temp = lhs;
        lhs = rhs;
        rhs = temp;
    }

    public static int StraightLineSegment1dComparison<TPositiveInteger, TPositiveIntegerTrait>
        (StraightLineSegment1d<TPositiveInteger, TPositiveIntegerTrait> lhs,
         StraightLineSegment1d<TPositiveInteger, TPositiveIntegerTrait> rhs) 
        where TPositiveInteger : struct /* PositiveInteger */    
        where TPositiveIntegerTrait : IPositiveIntegerTrait<TPositiveInteger>, new()
    {
        if (lhs == rhs) return 0;
        if (lhs > rhs) return 1;
        /*if (lhs > rhs)*/
        return -1;
    }
}

interface IPositiveIntegerTrait<T> where T : struct /* PositiveInteger */
{
    T Zero { get; }
    T Successor(T value);
    ulong Less(T lhs, T rhs);
    [Pure] bool GreaterOrEqualThan(T lhs, T rhs);
    void StableSort2(ref T lhs, ref T rhs);
}

class Int16Trait : IPositiveIntegerTrait<short>
{
    public short Successor(short value) { return (short)(value + 1); }
    public short Zero { get { return 0; } }
    public ulong Less(short lhs, short rhs) { return (ulong)(lhs - rhs); }
    public bool GreaterOrEqualThan(short lhs, short rhs) { return lhs >= rhs; }
    public void StableSort2(ref short lhs, ref short rhs)
    {
        if (rhs < lhs) Util.Swap(ref lhs, ref rhs);
        Contract.Ensures(lhs == Math.Min(lhs, rhs));
    }
}

class Int32Trait : IPositiveIntegerTrait<int>
{
    public int Successor(int value) { return value + 1; }
    public int Zero { get { return 0; } }
    public ulong Less(int lhs, int rhs) { return (ulong)(lhs - rhs); }
    public bool GreaterOrEqualThan(int lhs, int rhs) { return lhs >= rhs; }
    public void StableSort2(ref int lhs, ref int rhs)
    {
        if (rhs < lhs) Util.Swap(ref lhs, ref rhs);
        Contract.Ensures(lhs == Math.Min(lhs, rhs));
    }
}

class StraightLineSegment1d<TPositiveInteger, TPositiveIntegerTrait> : IDisposable
    where TPositiveInteger : struct /* PositiveInteger */ 
    where TPositiveIntegerTrait : IPositiveIntegerTrait<TPositiveInteger>, new()   
{
    private static TPositiveIntegerTrait positiveIntegerTrait = new TPositiveIntegerTrait();

    private void Init(TPositiveInteger a, TPositiveInteger b)
    {
        Contract.Requires(positiveIntegerTrait.GreaterOrEqualThan(a, positiveIntegerTrait.Zero)); //a >= 0
        Contract.Requires(positiveIntegerTrait.GreaterOrEqualThan(b, positiveIntegerTrait.Zero)); //b >= 0

        //convention a < b
        positiveIntegerTrait.StableSort2(ref a, ref b);
        this.a = a;
        this.b = b;
        
        Contract.Assert(length(a, b) > 0);
    }

    //default constructor
    public StraightLineSegment1d()
	{
        Init(positiveIntegerTrait.Zero, 
             positiveIntegerTrait.Successor(positiveIntegerTrait.Zero));
	}

    //constructor
    public StraightLineSegment1d(TPositiveInteger a, TPositiveInteger b)
    {
        Init(a, b);
    }

    //copy constructor
    public StraightLineSegment1d(StraightLineSegment1d<TPositiveInteger, TPositiveIntegerTrait> that)
    {
        a = that.a;
        b = that.b;
    }

    //copy assignment
    public StraightLineSegment1d<TPositiveInteger, TPositiveIntegerTrait> Copy(StraightLineSegment1d<TPositiveInteger, TPositiveIntegerTrait> that)
    {
        if (this != that)
        {
            a = that.a;
            b = that.b;
        }
        return this;
    }

    //destruction
    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.

            disposedValue = true;
        }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~StraightLineSegment1d() {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
        // TODO: uncomment the following line if the finalizer is overridden above.
        // GC.SuppressFinalize(this);
    }
    #endregion

    public override bool Equals(object obj)
    {
        if (base.Equals(obj))
            return true;
        var that = (StraightLineSegment1d<TPositiveInteger, TPositiveIntegerTrait>)obj;
        return Length == that.Length;
    }

    public override int GetHashCode()
    {
        return Length.GetHashCode();
    }

    //equality 
    public static bool operator==(StraightLineSegment1d<TPositiveInteger, TPositiveIntegerTrait> lhs, StraightLineSegment1d<TPositiveInteger, TPositiveIntegerTrait> rhs)
 	{ 
        return lhs.Equals(rhs);
    }

    //inequality
    public static bool operator !=(StraightLineSegment1d<TPositiveInteger, TPositiveIntegerTrait> lhs, StraightLineSegment1d<TPositiveInteger, TPositiveIntegerTrait> rhs)
    {
        return !(lhs == rhs);
    }

    public static bool operator<(StraightLineSegment1d<TPositiveInteger, TPositiveIntegerTrait> lhs, StraightLineSegment1d<TPositiveInteger, TPositiveIntegerTrait> rhs)
    {
        return lhs.Length < rhs.Length;
    }

    public static bool operator>(StraightLineSegment1d<TPositiveInteger, TPositiveIntegerTrait> lhs, StraightLineSegment1d<TPositiveInteger, TPositiveIntegerTrait> rhs)
    {
        return rhs < lhs;
    }

    public static bool operator<=(StraightLineSegment1d<TPositiveInteger, TPositiveIntegerTrait> lhs, StraightLineSegment1d<TPositiveInteger, TPositiveIntegerTrait> rhs)
    {
		return !(rhs < lhs);
	}

    public static bool operator>=(StraightLineSegment1d<TPositiveInteger, TPositiveIntegerTrait> lhs, StraightLineSegment1d<TPositiveInteger, TPositiveIntegerTrait> rhs)
    {
		return !(lhs < rhs);
    }

    public ulong Length { get { return length(a, b); } }

    public TPositiveInteger A { get { return a; } }
    public TPositiveInteger B { get { return b; } }

    public override string ToString()
    {
        return a + " -- " + Length + " -- " + b;
    }

    [Pure]
    private static ulong length(TPositiveInteger a, TPositiveInteger b)
    {
        Contract.Requires(positiveIntegerTrait.GreaterOrEqualThan(a, positiveIntegerTrait.Zero)); //a >= 0
        Contract.Requires(positiveIntegerTrait.GreaterOrEqualThan(b, positiveIntegerTrait.Zero)); //b >= 0
        return positiveIntegerTrait.Less(b, a); //b - a
    }

    private	TPositiveInteger a, b; /* endpoints */
}

class Program
{
    static void Main(string[] args)
    {
        {
            var ab = new StraightLineSegment1d<short, Int16Trait>(2, 4); //constructor
            var cd = new StraightLineSegment1d<short, Int16Trait>(); //default constructor
            var ef = new StraightLineSegment1d<short, Int16Trait>(); //default constructor and
            ef.Copy(new StraightLineSegment1d<short, Int16Trait>(1, 6)); //copy assignment
            var gh = new StraightLineSegment1d<short, Int16Trait>(new StraightLineSegment1d<short, Int16Trait>(2, 0)); //copy constructor

            Console.WriteLine("{0} == {1}? {2}", ab, gh, ab == gh);
            Console.WriteLine("{0} == {1}? {2}", cd, ef, cd == ef);
            Console.WriteLine("{0} != {1}? {2}", cd, ef, cd != ef);
            Console.WriteLine("{0} <  {1}? {2}", ab, gh, ab < gh);
            Console.WriteLine("{0} <= {1}? {2}", ab, gh, ab <= gh);
            Console.WriteLine("{0} >= {1}? {2}", ab, gh, ab >= gh);
            Console.WriteLine("{0} >  {1}? {2}", ab, gh, ab > gh);

            var xs = new StraightLineSegment1d<short, Int16Trait>[] { ab, cd, ef, gh };
            Array.Sort(xs, Util.StraightLineSegment1dComparison);
            foreach (var x in xs)
                Console.Write("[{0}] ", x);
            Console.WriteLine();
        }
        Console.WriteLine(new string('-', 40));
        {
            var ab = new StraightLineSegment1d<int, Int32Trait>(42, 14); //constructor
            var cd = new StraightLineSegment1d<int, Int32Trait>(); //default constructor
            var ef = new StraightLineSegment1d<int, Int32Trait>(); //default constructor and
            ef.Copy(new StraightLineSegment1d<int, Int32Trait>(31, 16)); //copy assignment
            var gh = new StraightLineSegment1d<int, Int32Trait>(new StraightLineSegment1d<int, Int32Trait>(20, 0)); //copy constructor

            Console.WriteLine("{0} == {1}? {2}", ab, gh, ab == gh);
            Console.WriteLine("{0} == {1}? {2}", cd, ef, cd == ef);
            Console.WriteLine("{0} != {1}? {2}", cd, ef, cd != ef);
            Console.WriteLine("{0} <  {1}? {2}", ab, gh, ab < gh);
            Console.WriteLine("{0} <= {1}? {2}", ab, gh, ab <= gh);
            Console.WriteLine("{0} >= {1}? {2}", ab, gh, ab >= gh);
            Console.WriteLine("{0} >  {1}? {2}", ab, gh, ab > gh);

            var xs = new StraightLineSegment1d<int, Int32Trait>[] { ab, cd, ef, gh };
            Array.Sort(xs, Util.StraightLineSegment1dComparison);
            foreach (var x in xs)
                Console.Write("[{0}] ", x);
            Console.WriteLine();
        }
    }
}
