using System;
using System.Collections.Generic;
using System.Drawing;

namespace Taxi
{ 
    class Car
    {
        readonly Point depot = new Point(732, 59);
        const float eps = 4f;
        const float speed = 0.5f; // per ms

        public string Name { get; private set; }
        public List<Point> Route { get; private set; } = new List<Point>();
        public float xPos { get; private set; }
        public float yPos { get; private set; } 


        public Car() { Name = "BC0000AA"; xPos = depot.X; yPos = depot.Y; }
        public Car(int number) { Name = "BC"+number.ToString("D4")+"AA"; xPos = depot.X; yPos = depot.Y; }
        public void AddRoute(List<Point> pnt)
        {
            foreach (var item in pnt)
            {
                Route.Add(item);
            }
        }

        public float GetDistationToTheEndOfQueue()
        {
            float res = 0;
            if (Route.Count == 0) return 0;
            res += Length(new PointF(xPos,yPos), Route[0]);
            for (int i = 0; i < Route.Count-1; i++)
            {
                res += Length(Route[i], Route[i + 1]);
            }
            return res;
        }
        public float Length(PointF first, PointF second)
        {
            return (float)Math.Sqrt(Math.Pow(second.X - first.X, 2) + Math.Pow(second.Y - first.Y, 2));
        }
        public bool isInDistrict(PointF x, PointF y) 
        {
            return (x.X <= y.X + eps) && (x.X >= y.X - eps) && (x.Y <= y.Y + eps) && (x.Y >= y.Y - eps);
        }
        
        internal void DoMove(object sender, EventArgs e, bool tf)
        {   
            if (Route.Count == 0) return;

            float dx = Route[0].X - xPos;
            float dy = Route[0].Y - yPos;

            float dv = Length(new PointF(xPos, yPos), new PointF((float)Route[0].X, (float)Route[0].Y));
            dv /= speed;
            xPos += (dx / dv);
            yPos += (dy / dv);

            if (isInDistrict(new PointF(xPos, yPos), Route[0]))
            {
                Route.RemoveAt(0);
                tf = false;
            }
        }
    }
}
