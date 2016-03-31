using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Taxi
{
    public class MatrixItem
    {
        public Point Next { get; set; }
        public double Length { get; set; }
        public MatrixItem(Point nextpt, double len) { Next = nextpt; Length = len; }
    }
    public class DictionaryMatrix
    {
        // created especially for main program part
        // we saves matrix of the shortest ways, created with Floyd Algorithm, in object of this time
        // you can get info about length of the shortest way and where you should go just by two point in tuple
        private Dictionary<Tuple<Point, Point>, MatrixItem> Data = new Dictionary<Tuple<Point, Point>, MatrixItem>();
        public DictionaryMatrix() { }
        public MatrixItem this[Tuple<Point, Point> index]
        {
            set
            {
                if (!(Data.ContainsKey(index)))
                    Data.Add(index, value);
                else
                    if (Data.ContainsKey(index)) Data[index] = value;
            }
            get { return Data[index]; }
        }
    }

    class Field
    {
        const int infinity = Int32.MaxValue;
        DictionaryMatrix GraphField;
        List<Point> allPoints;
        Dictionary<string, Car> fleet;
        const int countOfCars = 2; 

        internal Dictionary<string, Car> Fleet
        {
            get
            {
                return fleet;
            }

            set
            {
                fleet = value;
            }
        }

        public Field()
        {
            #region reading data from files
            List<Tuple<Point, Point>> tempGraph = new List<Tuple<Point, Point>>();
            TextReader read = File.OpenText("CityData.txt");
            TextReader pnts = File.OpenText("points.txt");
            string tempStr = read.ReadLine();
            while (tempStr != null)
            {
                char[] separators = new char[2] { ' ', '-' };
                string[] parts = tempStr.Split(separators, StringSplitOptions.None);
                tempGraph.Add(new Tuple<Point, Point>(new Point(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1])),
                    new Point(Convert.ToInt32(parts[2]), Convert.ToInt32(parts[3]))));
                tempStr = read.ReadLine();
            }

            allPoints = new List<Point>();
            tempStr = pnts.ReadLine();
            while (tempStr != null)
            {
                string[] parts = new string[2];
                parts = tempStr.Split(' ');
                allPoints.Add(new Point(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1])));
                tempStr = pnts.ReadLine();
            }
            #endregion
            GraphField = new DictionaryMatrix();
            // this loop creates main matrix of program
            // later we will fill it with distance between this points
            // it demands a lot of memory but finding a shortest way will be overoptimazied during the whole program execution
            for (int i = 0; i < allPoints.Count; i++)
            {
                for (int j = 0; j < allPoints.Count; j++)
                {
                    GraphField[new Tuple<Point, Point>(allPoints[i], allPoints[j])] = new MatrixItem(allPoints[i], infinity);
                }
            }
            FloydAlgorithm(tempGraph);
            fleet = new Dictionary<string, Car>();
            for (int i = 0; i < countOfCars; i++)
            {
                Car car = new Car(fleet.Count);
                fleet.Add(car.Name, car);
            }
        }

        public void FloydAlgorithm(List<Tuple<Point, Point>> tempGraph)
        {
            foreach (var item in tempGraph)
            {
                GraphField[new Tuple<Point, Point>(item.Item1, item.Item2)] = new MatrixItem(item.Item2, Length(item.Item1, item.Item2));
                GraphField[new Tuple<Point, Point>(item.Item2, item.Item1)] = new MatrixItem(item.Item1, Length(item.Item1, item.Item2));
            }

            for (int i = 0; i < allPoints.Count; i++)
            {
                for (int j = 0; j < allPoints.Count; j++)
                {
                    for (int k = 0; k < allPoints.Count; k++)
                    {
                        if ( (i != j) && (GraphField[new Tuple<Point, Point>(allPoints[j], allPoints[i])].Length != infinity) &&
                             (i != k) && (GraphField[new Tuple<Point, Point>(allPoints[i], allPoints[k])].Length != infinity) &&
                             ((GraphField[new Tuple<Point, Point>(allPoints[j], allPoints[k])].Length == infinity) ||
                             (GraphField[new Tuple<Point, Point>(allPoints[j], allPoints[k])].Length >
                             GraphField[new Tuple<Point, Point>(allPoints[j], allPoints[i])].Length +
                             GraphField[new Tuple<Point, Point>(allPoints[i], allPoints[k])].Length)) )
                        {
                            GraphField[new Tuple<Point, Point>(allPoints[j], allPoints[k])] =
                                new MatrixItem(GraphField[new Tuple<Point, Point>(allPoints[j], allPoints[i])].Next,
                                GraphField[new Tuple<Point, Point>(allPoints[j], allPoints[i])].Length +
                                GraphField[new Tuple<Point, Point>(allPoints[i], allPoints[k])].Length);
                        }

                    }
                }
            }
        }
        
        public Point FindTheNearestPoint(Point pnt)
        {
            Point res = new Point();
            double minLen = infinity;
            foreach (var item in allPoints)
            {
                double len = Length(pnt, item);
                if (len < minLen)
                {
                    res = item;
                    minLen = len;
                }
            }
            return res;
        }

        public List<Point> ArrayOfPathPoints(Point first, Point second)
        {
            List<Point> res = new List<Point>();

            Point start = FindTheNearestPoint(first);
            Point finish = FindTheNearestPoint(second);
           
            while (start != finish)
            {
                start = GraphField[new Tuple<Point, Point>(start, finish)].Next;
                res.Add(start);    
            }

            return res;
        }

        public double Length(Point first, Point second)
        {
            return Math.Sqrt(Math.Pow(second.X - first.X, 2) + Math.Pow(second.Y - first.Y, 2));
        }

        public string FindACar(Point pos)
        {  
            string res = null; double minLen = Int32.MaxValue;
            foreach (var item in fleet)
            {
                double len = 0;
                len += item.Value.GetDistationToTheEndOfQueue();
                // so here we consider that sometimes it's cheaper to wait until one taxi
                // will get to the final point of its route then send a new car from depot
                if (item.Value.Route.Count != 0)
                    len += GraphField[new Tuple<Point, Point>(item.Value.Route[item.Value.Route.Count - 1], pos)].Length;
                else
                    len += GraphField[new Tuple<Point, Point>(FindTheNearestPoint(new Point((int)item.Value.xPos, (int)item.Value.yPos)), pos)].Length;
                if (len < minLen)
                {
                    minLen = len;
                    res = item.Key;
                }
            }
            return res;
        }

        public Car FindByName(string carName)
        {
            Car car = new Car();
            foreach (var item in Fleet.Values)
            {
                if (item.Name == carName)
                {
                    car = item;
                    break;
                }
            }
            return car;
        }

    }
}
