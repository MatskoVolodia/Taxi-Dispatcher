using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// You may have some troubles with resources. 
/// It's easy to fix all this problems. You just have to send all resources to
/// directory with .exe file (/bin folder)
/// HAVE FUN
/// </summary>

namespace Taxi
{
    public partial class Taxi : Form
    {
        #region variables
        Dictionary<string, PictureBox> pBCars = new Dictionary<string, PictureBox>();
        Dictionary<string, PictureBox> routeMarksPB = new Dictionary<string, PictureBox>();
        Bitmap DrawArea = new Bitmap(Environment.CurrentDirectory.ToString() + @"\map.png");
        Bitmap avalCars = new Bitmap(Environment.CurrentDirectory.ToString() + @"\availableCars.png");
        Bitmap taxiOnTour = new Bitmap(Environment.CurrentDirectory.ToString() + @"\TaxiOnTour.png");
        Bitmap routeStart = new Bitmap(Environment.CurrentDirectory.ToString() + @"\routeStart.png");
        Bitmap routeFinish = new Bitmap(Environment.CurrentDirectory.ToString() + @"\routeFinish.png");
        Tuple<Point, string> tempStartPoint = null; // for saving start point of taxi route until prog gets an finish one
        Field myField;
        bool startFinish = true; bool tempBool = true;
        #endregion

        public Taxi()
        {
            InitializeComponent();
            MainTimer = new Timer(); MainTimer.Interval = 1; MainTimer.Enabled = true;
            BackgroundImage = DrawArea;
            mainArea.BackColor = Color.Transparent;
            myField = new Field(); 
            foreach (var item in myField.Fleet.Values)
            {
                MainTimer.Tick += new EventHandler((sender, e) => item.DoMove(sender, e, tempBool));
            }
        }

        private void CreatePictureBoxForTaxi(Car item)
        {          
            pBCars.Add(item.Name, new PictureBox());
            Controls.Add(pBCars[item.Name]);
            pBCars[item.Name].BackColor = Color.Transparent;
            pBCars[item.Name].Size = new Size(taxiOnTour.Width, taxiOnTour.Height);
            pBCars[item.Name].Location = new Point((int)(item.xPos - taxiOnTour.Width / 2), (int)(item.yPos - taxiOnTour.Height));
            pBCars[item.Name].Image = taxiOnTour;         
            mainArea.SendToBack();
            MainTimer.Tick += new EventHandler((sender, e) => DoMove(sender, e, item.Name));
        }
        private void CreatePictureBoxForRouteMarks(string carID, bool tf, int locX, int locY)
        {
            string stringID = myField.Fleet[carID].Name+locX.ToString()+locY.ToString()+tf.ToString();
            routeMarksPB.Add(stringID, new PictureBox());
            Controls.Add(routeMarksPB[stringID]);
            routeMarksPB[stringID].BackColor = Color.Transparent;
            Bitmap temp;
            temp = tf ? routeFinish : routeStart; 
            routeMarksPB[stringID].Size = new Size(temp.Width, temp.Height);
            routeMarksPB[stringID].Location = new Point((int)(locX - temp.Width / 2), 
                (int)(locY - temp.Height / 2));
            routeMarksPB[stringID].Image = temp;
            mainArea.SendToBack();
        }
        private void mainArea_Click(object sender, EventArgs e)
        {
            // if TRUE then we create a new route
            startFinish = !startFinish;
            var mouseEventArgs = e as MouseEventArgs;
            Point newPnt = myField.FindTheNearestPoint(mouseEventArgs.Location);
            if (startFinish)
            {
                CreatePictureBoxForRouteMarks(myField.Fleet[tempStartPoint.Item2].Name, startFinish, newPnt.X, newPnt.Y);
                if (myField.Fleet[tempStartPoint.Item2].Route.Count == 0)
                    myField.Fleet[tempStartPoint.Item2].AddRoute(myField.ArrayOfPathPoints(new Point((int)myField.Fleet[tempStartPoint.Item2].xPos, (int)myField.Fleet[tempStartPoint.Item2].yPos), tempStartPoint.Item1));
                else
                    myField.Fleet[tempStartPoint.Item2].AddRoute(myField.ArrayOfPathPoints(new Point(
                        (int)myField.Fleet[tempStartPoint.Item2].Route[myField.Fleet[tempStartPoint.Item2].Route.Count - 1].X,
                        (int)myField.Fleet[tempStartPoint.Item2].Route[myField.Fleet[tempStartPoint.Item2].Route.Count - 1].Y), 
                        tempStartPoint.Item1));
                myField.Fleet[tempStartPoint.Item2].AddRoute(myField.ArrayOfPathPoints(tempStartPoint.Item1, newPnt));
                tempStartPoint = null;
                return;
            }
            string carI = myField.FindACar(newPnt);
            if (!pBCars.ContainsKey(myField.Fleet[carI].Name)) CreatePictureBoxForTaxi(myField.Fleet[carI]);
            CreatePictureBoxForRouteMarks(carI, startFinish, newPnt.X, newPnt.Y);
            tempStartPoint = new Tuple<Point,string>(newPnt, carI);
            
        }
        internal void DoMove(object sender, EventArgs e, string pbID) // per 1 ms
        {
            // there we synchronize moves of car object with car picturebox
            // coordinates in object are float for accuracy so we should cast them to int for FORM 
            Car car = myField.FindByName(pbID);
            pBCars[pbID].Location = new Point((int)(car.xPos - taxiOnTour.Width / 2), (int)(car.yPos - taxiOnTour.Height));
            // deleting and pictureboxes that containes start and finish of taxi's route when our car go throught them
            Point nearest = myField.FindTheNearestPoint(new Point((int)car.xPos, (int)car.yPos));
            if (car.isInDistrict(new PointF(car.xPos, car.yPos), nearest))
            {
                if (routeMarksPB.ContainsKey(car.Name + nearest.X.ToString() + nearest.Y.ToString() + "True"))
                {
                    routeMarksPB[car.Name + nearest.X.ToString() + nearest.Y.ToString() + "True"].Hide();
                    routeMarksPB.Remove(car.Name + nearest.X.ToString() + nearest.Y.ToString() + "True");
                }
                if (routeMarksPB.ContainsKey(car.Name + nearest.X.ToString() + nearest.Y.ToString() + "False"))
                {
                    routeMarksPB[car.Name + nearest.X.ToString() + nearest.Y.ToString() + "False"].Hide();
                    routeMarksPB.Remove(car.Name + nearest.X.ToString() + nearest.Y.ToString() + "False");
                }
            }
        }
    }
}
