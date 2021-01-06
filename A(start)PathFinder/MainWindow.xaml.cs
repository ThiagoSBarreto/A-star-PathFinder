using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace A_start_PathFinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Node> Nodes = new List<Node>();
        public List<Border> Borders = new List<Border>();

        public List<Node> ClosedList = new List<Node>();
        public List<Node> OpenList = new List<Node>();
        public List<Node> PathList = new List<Node>();

        public Node StartNode;
        public Node FinalNode;

        public bool startSelect = true;
        public bool finalSelect = true;

        public List<int> FValues = new List<int>();

        public bool done = false;

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        public void Initialize()
        {
            Random r = new Random(DateTime.Now.Millisecond);
            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    Border b = new Border();
                    b.BorderThickness = new Thickness(1);
                    b.BorderBrush = Brushes.Black;
                    b.CornerRadius = new CornerRadius(5);
                    b.Background = Brushes.White;

                    b.MouseRightButtonUp += SetObstable;
                    b.MouseLeftButtonUp += SetStartPoint;

                    Grid.SetRow(b, x);
                    Grid.SetColumn(b, y);

                    Node n = new Node();
                    n.f = 1;
                    n.x = x;
                    n.y = y;
                    n.impassable = false;

                    //n.c = r.Next(0, 100);

                    Label l = new Label();
                    l.Content = "C:" + n.c;
                    l.VerticalAlignment = VerticalAlignment.Center;
                    l.HorizontalAlignment = HorizontalAlignment.Center;

                    b.Child = l;

                    //if ((r.Next(0, 10) + r.Next(0, 100)) %2 == 0)
                    //{
                    //    b.Background = Brushes.Black;
                    //    n.c = int.MaxValue;
                    //}

                    Borders.Add(b);
                    Nodes.Add(n);

                    Board.Children.Add(b);
                }
            }
        }

        public void SetStartPoint(object sender, RoutedEventArgs e)
        {
            if (startSelect)
            {
                startSelect = false;
                int index = Borders.IndexOf((sender as Border));
                Nodes[index].sNode = true;
                StartNode = Nodes[index];
                Borders[index].Background = Brushes.Green;
                Nodes[index].impassable = false;
            }
            else if (finalSelect)
            {
                finalSelect = false;
                int index = Borders.IndexOf((sender as Border));
                Nodes[index].fNode = true;
                FinalNode = Nodes[index];
                Borders[index].Background = Brushes.Red;
                Nodes[index].impassable = false;
                StartNode.f = 0;
                OpenList.Add(StartNode);

                DateTime antes = DateTime.Now;
                Calcular();
                DateTime agora = DateTime.Now;


                if(done == true)
                {
                    Reset();
                    return;
                }

                Node curNode = FinalNode.p;
                while(curNode != StartNode)
                {
                    PathList.Add(curNode);
                    curNode = curNode.p;
                }
                PathList.Reverse();
                foreach(Node node in PathList)
                {
                    Borders[Nodes.IndexOf(node)].Background = Brushes.Blue;
                }

                MessageBox.Show(string.Format("Took {0}ms to Find the Path", (agora - antes).TotalMilliseconds));
                Reset();
            }
        }

        public void Reset()
        {
            Nodes.Clear();
            Borders.Clear();
            OpenList.Clear();
            ClosedList.Clear();
            PathList.Clear();
            StartNode = null;
            startSelect = true;
            FinalNode = null;
            finalSelect = true;
            done = false;
            FValues.Clear();

            Board.Children.Clear();

            Initialize();
        }

        public void SetObstable(object sender, RoutedEventArgs e)
        {
            int index = Borders.IndexOf((sender as Border));
            if (Nodes[index].Equals(StartNode))
            {
                Borders[index].Background = Brushes.Transparent;
                Nodes[index].sNode = false;
                startSelect = true;
            }
            else if (Nodes[index].Equals(FinalNode))
            {
                Borders[index].Background = Brushes.Transparent;
                Nodes[index].fNode = false;
                finalSelect = true;
            }
            else if (Nodes[index].impassable == false)
            {
                Borders[index].Background = Brushes.Black;
                Nodes[index].impassable = true;
                ClosedList.Add(Nodes[index]);
            }
        }

        public void Calcular()
        {
            while (OpenList.Count > 0)
            {
                //await Task.Delay(100);
                Node currentNode = null;
                while (currentNode == null)
                {
                    OpenList = OpenList.OrderBy(n => n.f).ToList();
                    currentNode = OpenList[0];
                    OpenList.Remove(currentNode);
                    ClosedList.Add(currentNode);
                    if (currentNode != null)
                    {

                        if (currentNode.fNode)
                        {
                            return;
                        }

                        currentNode.children.Add(Nodes.FirstOrDefault(n => n.x == currentNode.x - 1 && n.y == currentNode.y && !ClosedList.Contains(n)));
                        currentNode.children.Add(Nodes.FirstOrDefault(n => n.x == currentNode.x && n.y == currentNode.y - 1 && !ClosedList.Contains(n)));
                        currentNode.children.Add(Nodes.FirstOrDefault(n => n.x == currentNode.x + 1 && n.y == currentNode.y && !ClosedList.Contains(n)));
                        currentNode.children.Add(Nodes.FirstOrDefault(n => n.x == currentNode.x && n.y == currentNode.y + 1 && !ClosedList.Contains(n)));

                        foreach (Node c in currentNode.children)
                        { 
                            if (!OpenList.Contains(c))
                            {
                                if (c != null && !c.impassable)
                                {
                                    if (c.x < StartNode.x)
                                        c.g = StartNode.x - c.x;
                                    else
                                        c.g = c.x - StartNode.x;
                                    if (c.y < StartNode.y)
                                        c.g += StartNode.y - c.y;
                                    else
                                        c.g += c.y - StartNode.y;

                                    if (c.x < currentNode.x)
                                        c.g2 = currentNode.x - c.x;
                                    else
                                        c.g2 = c.x - currentNode.x;
                                    if (c.y < currentNode.y)
                                        c.g2 += currentNode.y - c.y;
                                    else
                                        c.g2 += c.y - currentNode.y;

                                    if (c.x < FinalNode.x)
                                        c.h = FinalNode.x - c.x;
                                    else
                                        c.h = c.x - FinalNode.x;
                                    if (c.y < FinalNode.y)
                                        c.h += FinalNode.y - c.y;
                                    else
                                        c.h += c.y - FinalNode.y;

                                    c.f = c.g + c.h;
                                    c.f = c.f + c.c;

                                    Label l = new Label();
                                    l.Content = "F:" + c.f;
                                    l.VerticalAlignment = VerticalAlignment.Center;
                                    l.HorizontalAlignment = HorizontalAlignment.Center;
                                    if (c != FinalNode)
                                    {
                                        Borders[Nodes.IndexOf(c)].Child = l;
                                        Borders[Nodes.IndexOf(c)].Background = Brushes.LightBlue;
                                    }
                                    c.p = currentNode;

                                    OpenList.Add(c);
                                }
                            }
                        }
                    }
                }
            }
            if (OpenList.Count == 0)
            {
                MessageBox.Show("No Path available!");
                done = true;
            }
        }
    }
}
