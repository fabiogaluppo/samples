//Sample provided by Fabio Galuppo
//March 2018

//Test file
/*
2.37354618925767,5.39810588036707,0
3.18364332422208,4.38797360674923,0
2.16437138758995,5.34111969142442,0
4.59528080213779,3.87063690391921,0
3.32950777181536,6.43302370170104,0
2.17953161588198,6.98039989850586,0
3.48742905242849,4.63277852353349,0
3.73832470512922,3.95586537368347,0
3.57578135165349,5.56971962744241,0
2.69461161284364,4.86494539611918,0
4.51178116845085,7.40161776050478,0
3.38984323641143,4.96075999726683,0
2.3787594194582,5.68973936245078,0
0.7853001128225,5.02800215878067,0
4.12493091814311,4.25672679111759,0
2.95506639098477,5.18879229951434,0
2.98380973690105,3.19504137110896,0
3.9438362106853,6.46555486156289,0
3.82122119509809,5.1532533382119,0
3.59390132121751,7.17261167036215,0
3.91897737160822,5.47550952889966,0
3.78213630073107,4.29005356907819,0
3.07456498336519,5.61072635348905,0
1.01064830413663,4.06590236835575,0
3.61982574789471,3.7463665997609,0
2.943871260471,5.29144623551746,0
2.84420449329467,4.55670812678157,0
1.52924761610073,5.00110535163162,0
2.52184994489138,5.07434132415166,0
3.4179415601997,4.41047905381193,0
4.35867955152904,4.4313312671815,0
2.897212272657,4.86482138487617,0
3.38767161155937,6.1780869965732,0
2.94619495941709,3.47643319957024,0
1.62294044317139,5.59394618762842,0
2.58500543670032,5.33295037121352,0
2.60571004628965,6.06309983727636,0
2.94068660328881,4.6958160763657,0
4.10002537198388,5.37001880991629,0
3.76317574845754,5.26709879077223,0
2.83547640374641,4.45747996900835,0
2.74663831986349,6.20786780598317,0
3.69696337540474,6.16040261569495,0
3.55666319867366,5.700213649515,0
2.31124430545048,6.58683345454085,0
2.29250484303788,5.5584864255653,0
3.36458196213683,3.72340779154196,0
3.76853292451542,4.42673458576311,0
2.88765378784977,3.77538738510164,0
3.88110772645421,4.52659936356069,0
4.37963332277588,7.45018710127266,1
5.04211587314424,6.98144016728536,1
4.08907835144755,6.68193162545616,1
5.15802877240407,6.0706378525463,1
4.34541535608118,5.51253968985852,1
6.76728726937265,5.92480770338432,1
5.71670747601721,8.00002880371391,1
5.91017422949523,6.37873330520318,1
5.38418535782634,5.61557315261551,1
6.68217608051942,8.86929062242358,1
4.36426354605102,7.42510037737245,1
4.53835526963943,6.76135289908697,1
6.43228223854166,8.05848304870902,1
4.34930364668963,7.88642265137494,1
4.79261925639803,6.38075695176885,1
4.60719207055802,9.20610246454047,1
4.68000713145149,6.74497296985898,1
4.72088669702344,5.57550534978719,1
5.49418833126783,6.85560039804578,1
4.82266951773039,7.20753833923234,1
4.49404253788574,9.30797839905936,1
6.34303882517041,7.10580236789371,1
4.78542059145313,7.45699880542341,1
4.82044346995661,6.92284706464347,1
4.89980925878644,6.66599915763346,1
5.71266630705141,6.96527397168872,1
4.92643559587367,7.78763960563016,1
4.96236582853295,9.07524500865228,1
4.31833952124434,8.02739243876377,1
4.67572972775368,8.2079083983867,1
5.06016044043452,5.76867657844196,1
4.41110551374034,7.98389557005338,1
5.53149619263257,7.21992480366065,1
3.48160591821321,5.53274997090776,1
5.30655786078977,7.52102274264814,1
3.46355017646241,6.84124539528398,1
4.69902387316339,8.4645873119698,1
4.47172009555499,6.23391800039534,1
4.347905219319,6.56978824607145,1
4.94310322215261,6.07389050262256,1
3.08564057431999,6.82289603856346,1
6.17658331201856,7.40201177948634,1
3.335027563788,6.26825182688039,1
4.53646959852761,7.83037316798167,1
3.88407989495715,5.79191721369553,1
4.24918099880655,5.95201558719226,1
7.08716654562835,8.44115770684428,1
5.01739561969325,5.98415253469535,1
3.71369946956567,7.41197471231752,1
3.35939446558142,6.61892394889108,1 
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Classification
{
    struct Point2D : IComparable<Point2D>, IComparable
    {
        public readonly double X;
        public readonly double Y;

        public Point2D(double x, double y)
        {
            if (Double.IsInfinity(x) || Double.IsInfinity(y))
                throw new ArgumentException("Coordinates must be finite");
            if (Double.IsNaN(x) || Double.IsNaN(y))
                throw new ArgumentException("Coordinates cannot be NaN");
            X = (x == 0.0) ? 0.0 : x; //convert -0.0 to +0.0
            Y = (y == 0.0) ? 0.0 : y; //convert -0.0 to +0.0
        }

        public double DistanceSquaredTo(Point2D that)
        {
            double dx = this.X - that.X;
            double dy = this.Y - that.Y;
            return dx * dx + dy * dy;
        }

        public int CompareTo(Point2D that)
        {
            if (this.Y < that.Y) return -1;
            if (this.Y > that.Y) return +1;
            if (this.X < that.X) return -1;
            if (this.X > that.X) return +1;
            return 0;
        }

        public int CompareTo(object obj)
        {
            return CompareTo((Point2D)obj);
        }

        public override String ToString()
        {
            return "[" + X + ", " + Y + "]";
        }
    }

    struct RectHV
    {
        public readonly double Xmin, Ymin;
        public readonly double Xmax, Ymax;

        public RectHV(double xmin, double ymin, double xmax, double ymax)
        {
            if (xmax < xmin || ymax < ymin)            
                throw new ArgumentException("Invalid rectangle");            
            Xmin = xmin;
            Ymin = ymin;
            Xmax = xmax;
            Ymax = ymax;
        }

        public double Width { get { return Xmax - Xmin; } }
        public double Height { get { return Ymax - Ymin; } }

        public bool Intersects(RectHV that)
        {
            return this.Xmax >= that.Xmin && this.Ymax >= that.Ymin
                && that.Xmax >= this.Xmin && that.Ymax >= this.Ymin;
        }

        public double distanceTo(Point2D p)
        {
            return Math.Sqrt(this.distanceSquaredTo(p));
        }

        public double distanceSquaredTo(Point2D p)
        {
            double dx = 0.0, dy = 0.0;
            if (p.X < Xmin) dx = p.X - Xmin;
            else if (p.X > Xmax) dx = p.X - Xmax;
            if (p.Y < Ymin) dy = p.Y - Ymin;
            else if (p.Y > Ymax) dy = p.Y - Ymax;
            return dx * dx + dy * dy;
        }

        public bool contains(Point2D p)
        {
            return (p.X >= Xmin) && (p.X <= Xmax) && (p.Y >= Ymin) && (p.Y <= Ymax);
        }

        public override bool Equals(object obj)
        {
            if (obj == (object)this) return true;
            if (obj == null) return false;
            if (obj.GetType() != this.GetType()) return false;
            RectHV that = (RectHV)obj;
            if (this.Xmin != that.Xmin) return false;
            if (this.Ymin != that.Ymin) return false;
            if (this.Xmax != that.Xmax) return false;
            if (this.Ymax != that.Ymax) return false;
            return true;
        }

        public override int GetHashCode()
        {
            const int PRIME = 31;
            int hash1 = Xmin.GetHashCode();
            int hash2 = Ymin.GetHashCode();
            int hash3 = Xmax.GetHashCode();
            int hash4 = Ymax.GetHashCode();
            return PRIME * (PRIME * (PRIME * hash1 + hash2) + hash3) + hash4;
        }
        
        public override String ToString()
        {
            return "[" + Xmin + ", " + Xmax + "] x [" + Ymin + ", " + Ymax + "]";
        }
    }

    sealed class KdTree
    {
        private sealed class Node
        {
            public Point2D p;   //point
            public RectHV rect; //axis-aligned rectangle, boundaries of this node
            public Node lb;     //left/bottom subtree
            public Node rt;     //right/top subtree
        }

        private Node root = null;
        private int N;
        
        public KdTree()
        {
        }

        public bool IsEmpty
        {
            get { return Size() == 0; }
        }

        public int Size()
        {
            return N;
        }

        private Node InsertRecursive(Node x, Point2D p, bool isVertical, double xmin, double ymin, double xmax, double ymax)
        {
            if (null == x)
            {
                Node newNode = new Node() { p = p, rect = new RectHV(xmin, ymin, xmax, ymax) };
                ++N;
                return newNode;
            }

            if (0 == x.p.CompareTo(p))            
                return x;
            
            if (isVertical)
            {
                if (p.X < x.p.X)
                    x.lb = InsertRecursive(x.lb, p, !isVertical, x.rect.Xmin, x.rect.Ymin, x.p.X, x.rect.Ymax);
                else                
                    x.rt = InsertRecursive(x.rt, p, !isVertical, x.p.X, x.rect.Ymin, x.rect.Xmax, x.rect.Ymax);
            }
            else
            {
                if (p.Y < x.p.Y)                
                    x.lb = InsertRecursive(x.lb, p, !isVertical, x.rect.Xmin, x.rect.Ymin, x.rect.Xmax, x.p.Y);
                else                
                    x.rt = InsertRecursive(x.rt, p, !isVertical, x.rect.Xmin, x.p.Y, x.rect.Xmax, x.rect.Ymax);
            }
            return x;
        }

        public void Insert(Point2D p)
        {
            root = InsertRecursive(root, p, true, 0.0, 0.0, 1.0, 1.0);
        }

        private Node FindRecursive(Node x, Point2D p, bool isVertical)
        {
            if (x == null)
                return null;
            int cmp = x.p.CompareTo(p);
            if (0 == cmp)            
                return x;
            if (isVertical)
                return FindRecursive(p.X < x.p.X ? x.lb : x.rt, p, !isVertical);
            return FindRecursive(p.Y < x.p.Y ? x.lb : x.rt, p, !isVertical);
        }

        public bool Contains(Point2D p)
        {
            return FindRecursive(root, p, true) != null;
        }

        private void FindRangeRecursive(Node x, RectHV rect, Queue<Point2D> acc, bool isVertical)
        {
            if (rect.contains(x.p))
                acc.Enqueue(x.p);

            RectHV line = isVertical ? new RectHV(x.p.X, x.rect.Ymin, x.p.X, x.rect.Ymax) : 
                new RectHV(x.rect.Xmin, x.p.Y, x.rect.Xmax, x.p.Y);
            if (rect.Intersects(line))
            {
                //both subtrees
                if (null != x.lb)
                    FindRangeRecursive(x.lb, rect, acc, !isVertical);
                if (null != x.rt)
                    FindRangeRecursive(x.rt, rect, acc, !isVertical);
            }
            else
            {
                //just one subtree
                if (null != x.lb && rect.Intersects(x.lb.rect))
                    FindRangeRecursive(x.lb, rect, acc, !isVertical);
                else if (null != x.rt)
                    FindRangeRecursive(x.rt, rect, acc, !isVertical);
            }
        }

        public IEnumerable<Point2D> Range(RectHV rect)
        {
            Queue<Point2D> acc = new Queue<Point2D>();
            if (!IsEmpty)
                FindRangeRecursive(root, rect, acc, true);
            return acc;
        }

        private Node FindInsertionPointRecursive(Node parent, Node x, Point2D p, bool isVertical)
        {
            if (null == x)
                return parent;

            if (isVertical)
            {
                if (p.X < x.p.X)
                    return FindInsertionPointRecursive(x, x.lb, p, !isVertical);
                return FindInsertionPointRecursive(x, x.rt, p, !isVertical);                
            }
            else
            {
                if (p.Y < x.p.Y)                
                    return FindInsertionPointRecursive(x, x.lb, p, !isVertical);
                return FindInsertionPointRecursive(x, x.rt, p, !isVertical);
            }
        }

        private double PerpendicularSquared(Node x, Point2D p, bool isVertical)
        {
            Point2D q = isVertical ? new Point2D(x.p.X, p.Y) : new Point2D(p.X, x.p.Y);
            return p.DistanceSquaredTo(q);
        }

        private Point2D FindNearestRecursive(Node x, Point2D p, Point2D closest, bool isVertical)
        {
            double a = p.DistanceSquaredTo(x.p);
            double b = p.DistanceSquaredTo(closest);
            Point2D c = a < b ? x.p : closest;
            
            double dpSquared = PerpendicularSquared(x, p, isVertical);
            double minSquared = p.DistanceSquaredTo(c);
            if (dpSquared < minSquared)
            {
                if (null != x.rt)
                {
                    Point2D q = FindNearestRecursive(x.rt, p, c, !isVertical);
                    double qSquared = p.DistanceSquaredTo(q);
                    if (qSquared < minSquared)
                    {
                        c = q;
                        minSquared = qSquared;
                    }
                }

                if (null != x.lb)
                {
                    Point2D q = FindNearestRecursive(x.lb, p, c, !isVertical);
                    double qSquared = p.DistanceSquaredTo(q);
                    if (qSquared < minSquared)
                    {
                        c = q;
                        //minSquared = qSquared;
                    }
                }
            }
            else
            {
                bool isLB = isVertical ? p.X < x.p.X : p.Y < x.p.Y;
                if (isLB)
                {
                    if (x.lb != null)
                        return FindNearestRecursive(x.lb, p, c, !isVertical);
                }
                else
                {
                    if (x.rt != null)
                        return FindNearestRecursive(x.rt, p, c, !isVertical);
                }
            }
            return c;
        }

        public Point2D Nearest(Point2D p)
        {
            if (IsEmpty)
                throw new InvalidOperationException("Object is empty");
            Node closest = FindInsertionPointRecursive(null, root, p, true);
            return FindNearestRecursive(root, p, closest.p, true);
        }
    }

    class Program
    {
        struct Data
        {
            public double x, y;
            public int label;
        }

        static IEnumerable<Data> GetAllData(string path, out double xmax, out double ymax)
        {
            List<Data> xs = new List<Data>();
            xmax = 0.0;
            ymax = 0.0;
            string[] lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                string[] data = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                double x = Double.Parse(data[0]);
                double y = Double.Parse(data[1]);
                int label = Int32.Parse(data[2]);
                xs.Add(new Data() { x = x, y = y, label = label });
                if (x > xmax) xmax = x;
                if (y > ymax) ymax = y;
            }
            return xs;
        }

        static void TestCase1(IEnumerable<Data> xs, double xmax, double ymax)
        {
            if (xs.Count() > 0)
            {
                KdTree kd = new KdTree();
                HashSet<Point2D> zeroes = new HashSet<Point2D>();
                HashSet<Point2D> ones = new HashSet<Point2D>();

                foreach (var x in xs)
                {
                    Point2D p = new Point2D(x.x / xmax, x.y / ymax);
                    if (x.label == 0) zeroes.Add(p); else ones.Add(p);
                    kd.Insert(p);
                }

                Point2D target = new Point2D(5.3 / xmax, 4.3 / ymax);
                //Point2D target = new Point2D(4.8 / xmax, 8.1 / ymax);
                //Point2D target = new Point2D(3.5 / xmax, 4.5 / ymax);
                Point2D nearest = kd.Nearest(target);
                bool zero = zeroes.Contains(nearest);
                bool one = ones.Contains(nearest);
                Console.WriteLine("Target point is " + target);
                Console.WriteLine("The nearest point {0} is classified as '{1}'", nearest, zero ? 0 : 1);

                double delta = 0.10;
                //double delta = 0.15;
                IEnumerable<Point2D> points = kd.Range(new RectHV(target.X - delta, target.Y - delta, target.X + delta, target.Y + delta));
                var counters = points.Aggregate(Tuple.Create(0, 0), 
                                                (acc, p) => zeroes.Contains(p) ? Tuple.Create(acc.Item1 + 1, acc.Item2) : Tuple.Create(acc.Item1, acc.Item2 + 1));
                if (counters.Item1 < counters.Item2)
                {
                    Console.WriteLine("It's classified as '1' ({0:N1}% occurrences in the neighborhood)", 100.0 * counters.Item2 / points.Count());
                }
                else if (counters.Item1 > counters.Item2)
                {
                    Console.WriteLine("It's classified as '0' ({0:N1}% occurrences in the neighborhood)", 100.0 * counters.Item1 / points.Count());
                }
                else
                {
                    Console.WriteLine("With inconclusive classification");
                }
            }
            else
            {
                Console.WriteLine("No data");
            }
        }

        static void Main(string[] args)
        {
            if (args.Length > 0 && File.Exists(args[0]))
            {
                double xmax, ymax;
                TestCase1(GetAllData(args[0], out xmax, out ymax), xmax, ymax);
            }
            else
            {
                Console.WriteLine("Not a file");
            }
            return;

            //sample KdTree API
            /*
            KdTree kd = new KdTree();
            kd.Insert(new Point2D(0.1, 0.2));
            kd.Insert(new Point2D(0.3, 0.1));
            kd.Insert(new Point2D(0.3, 0.3));
            Point2D nearest = kd.Nearest(new Point2D(0.3, 0.35));
            IEnumerable<Point2D> points = kd.Range(new RectHV(0.3, 0.0, 1.0, 1.0));
            */
        }
    }
}