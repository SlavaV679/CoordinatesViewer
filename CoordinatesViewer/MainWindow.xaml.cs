using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading;
using System.Net.Http;

namespace CoordinatesViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CoordinatesClient coordinatesClient;

        bool IsWriteInDb = false;

        private static ManualResetEvent _runner = new ManualResetEvent(false);

        int counter;

        public TimeSpan Timeout { get; private set; }

        public MainWindow()
        {
            coordinatesClient = GetClient();

            InitializeComponent();

            this.Loaded += ControlLoaded;
        }

        public CoordinatesClient GetClient()
        {
            var httpClient = new HttpClient();

            httpClient.BaseAddress = new Uri("http://localhost:17788/");

            return new CoordinatesClient(httpClient);
        }

        private async void Grid_Initialized(object sender, EventArgs e)
        {     
            try
            {
                Task TrackCoordinatesTask = Task.Run(() => TrackCoordinates());

                foreach (var point in await coordinatesClient.GetAllCoordinates())
                {
                    counter++;
                    listBoxData.Items.Add(point);
                }
                lblCounter.Content = counter;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString());
            }
        }

        private void BtnStartStop_Click(object sender, RoutedEventArgs e)
        {
            if (IsWriteInDb == false)
            {
                IsWriteInDb = true;
                _runner.Set();
            }
            else
            {
                IsWriteInDb = false;
                _runner.Reset();
            }
        }              

        private async void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RadioBtnUnchek();

            await RefreshListBoxData();
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await coordinatesClient.DeleteCoordinates();

                listBoxData.Items.Clear();
                counter = 0;
                lblCounter.Content = counter;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString());
            }
        }

        private async void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            await InsertCoordinatesMouseClick("left");
        }

        private async void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            await InsertCoordinatesMouseClick("right");
        }

        private async void RadioBtnMove_Checked(object sender, RoutedEventArgs e)
        {
            await GetCoordinates("move");
        }

        private async void RadioBtnLeft_Checked(object sender, RoutedEventArgs e)
        {
            await GetCoordinates("left");
        }

        private async void RadioBtnRight_Checked(object sender, RoutedEventArgs e)
        {
            await GetCoordinates("right");
        }

        private async void btnView_Click(object sender, RoutedEventArgs e)
        {
            listBoxData.Items.Clear();
            counter = 0;
            RadioBtnUnchek();

            DateTime datePickerFrom = new DateTime(1900, 01, 01, 12, 00, 00);
            DateTime datePickerTo = new DateTime(2999, 01, 01, 12, 00, 00);

            if (DatePickerFrom.SelectedDate != null)
            { datePickerFrom = DatePickerFrom.SelectedDate.Value; }

            if (DatePickerTo.SelectedDate != null)
            { datePickerTo = DatePickerTo.SelectedDate.Value; }

            foreach (var point in await coordinatesClient.GetCoordinates(datePickerFrom, datePickerTo))
            {
                listBoxData.Items.Add(point);
                counter++;
            }
            lblCounter.Content = counter;
        }

        #region helperMethods

        private Task TrackCoordinates()
        {
            int x = 0, xOld = 0, y = 0, yOld = 0;
            while (true)
            {
                _runner.WaitOne();

                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        var windowPosition = Mouse.GetPosition(this);
                        var screenPosition = PointToScreen(windowPosition);

                        x = Convert.ToInt32(screenPosition.X);
                        y = Convert.ToInt32(screenPosition.Y);

                        lblX.Content = x;
                        lblY.Content = y;

                        if ((Math.Abs(xOld - x) > 9 || Math.Abs(yOld - y) > 9)
                             && x != 0 && y != 0)
                        {
                            lblXDb.Content = x;
                            xOld = x;
                            lblYDb.Content = y;
                            yOld = y;
                            
                            coordinatesClient.InsertCoordinates(x.ToString(), y.ToString(), "move").ConfigureAwait(false);
                            
                            lblCounter.Content = ++counter;
                        }
                    }
                    catch (InvalidOperationException)
                    {

                    }
                });
            }
        }

        private async Task RefreshListBoxData()
        {
            listBoxData.Items.Clear();
            counter = 0;

            foreach (var point in await coordinatesClient.GetAllCoordinates())
            {
                listBoxData.Items.Add(point);
                lblCounter.Content = ++counter;
            }
        }

        private async Task GetCoordinates(string eventMouse)
        {
            listBoxData.Items.Clear();
            counter = 0;

            foreach (var point in await coordinatesClient.GetCoordinates(eventMouse))
            {
                listBoxData.Items.Add(point);
                lblCounter.Content = ++counter;
            }
        }

        private async Task InsertCoordinatesMouseClick(string button)
        {
            if (IsWriteInDb)
            {
                await coordinatesClient.InsertCoordinates(lblXDb.Content.ToString(), lblYDb.Content.ToString(), button);
                lblCounter.Content = ++counter;
            }
        }

        private void RadioBtnUnchek()
        {
            RadioBtnMove.IsChecked = false;
            RadioBtnLeft.IsChecked = false;
            RadioBtnRight.IsChecked = false;
        }

        public void ControlLoaded(object sender, EventArgs e)
        {
            Console.WriteLine(this.PointToScreen(new Point(0, 0)));
        }

        #endregion
    }
}
