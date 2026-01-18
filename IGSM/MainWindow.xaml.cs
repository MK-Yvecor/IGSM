using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts.Charts;
using System.Threading;
using System.Windows.Threading;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel;
using LiveCharts.Wpf;
using LiveCharts.Definitions.Series;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Globalization;

namespace IGSM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DispatcherTimer timer;
        UART uart;

        static BasicTelemetry telemetry;
        static EX_01_Data eX_01_Data;
        static IMUTelemetry IMUTelemetry;


        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            MainPage.Visibility = Visibility.Visible;
            IGS_Page.Visibility = Visibility.Hidden;
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            timer.Start();
            telemetry = new BasicTelemetry();
            GetSerialPortList();
        }


        private void Timer_Tick(object? sender, EventArgs e)
        {
            var now = DateTime.Now;
            var interval = now.AddSeconds(1).AddMilliseconds(-now.Millisecond);
            timer.Interval = interval - now;
            UpdateTime();
        }

        private void UpdateTime()
        {
            Current_Time_TB.Text = "CT: " + DateTime.Now.ToString("HH:mm:ss");
            UTC_Time_TB.Text = "UTC: " + DateTime.UtcNow.ToString("HH:mm:ss");
        }



        private void GetSerialPortList()
        {
            ComPorts_CB.Items.Clear();
            string []ComPortList = SerialPort.GetPortNames();

            foreach(string i in ComPortList)
            {
                ComPorts_CB.Items.Add(i);
            }
        }


        private void MPRB_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Visibility = Visibility.Visible;
            IGS_Page.Visibility = Visibility.Hidden;
        }

        private void IGSPRB_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Visibility = Visibility.Hidden;
            IGS_Page.Visibility = Visibility.Visible;
        }

        private void ComConnect_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (ComPorts_CB.Text.Contains("COM"))
            {
                uart = new UART(ComPorts_CB.Text);
                uart.DataReceived += Uart_DataReceived;
                uart.Connect();
            }
            else
            {
                return;
            }
        }

        private void Uart_DataReceived(string prefixbuffer, string databuffer)
        {
            Dispatcher.BeginInvoke(() =>
            {
                    DataType.TryGetValue(prefixbuffer, out Action<double> setter);
                    setter(double.Parse(databuffer, NumberStyles.Float, CultureInfo.InvariantCulture));

                    Altitude_TB.Text = $"{telemetry.Altitude}" + "m";
                    Temperature_TB.Text = $"{telemetry.Tempereature}" + "°C";
                    Pressure_TB.Text = $"{telemetry.Pressure}" + "hPa";
                    Voltage_TB.Text = $"{telemetry.Voltage}" + "V";
                    ER_TB.Text = $"{eX_01_Data.RChannel}" + "V";
                    EG_TB.Text = $"{eX_01_Data.GChannel}" + "V";
                    EB_TB.Text = $"{eX_01_Data.BChannel}" + "V";
                    GX_TB.Text = $"{IMUTelemetry.GyroX}" + "°/s";
                    GY_TB.Text = $"{IMUTelemetry.GyroY}" + "°/s";
                    GZ_TB.Text = $"{IMUTelemetry.GyroZ}" + "°/s";
                    AX_TB.Text = $"{IMUTelemetry.AccelX}" + "m/s^2";
                    AY_TB.Text = $"{IMUTelemetry.AccelY}" + "m/s^2";
                    AZ_TB.Text = $"{IMUTelemetry.AccelZ}" + "m/s^2";

            });
        }

        private void ComRefresh_Btn_Click(object sender, RoutedEventArgs e)
        {
            GetSerialPortList();
        }

        private void ComDisConnect_Btn_Click(object sender, RoutedEventArgs e)
        {
            
        }

        static Dictionary<string, Action<double>> DataType = new Dictionary<string, Action<double>>
        {
            ["AL"] = value => telemetry.Altitude = value,
            ["TE"] = value => telemetry.Tempereature = value,
            ["PR"] = value => telemetry.Pressure = value,
            ["VO"] = value => telemetry.Voltage = value,
            ["ER"] = value => eX_01_Data.RChannel = value,
            ["EG"] = value => eX_01_Data.GChannel = value,
            ["EB"] = value => eX_01_Data.BChannel = value,
            ["GX"] = value => IMUTelemetry.GyroX = value,
            ["GY"] = value => IMUTelemetry.GyroY = value,
            ["GZ"] = value => IMUTelemetry.GyroZ = value,
            ["AX"] = value => IMUTelemetry.AccelX = value,
            ["AY"] = value => IMUTelemetry.AccelY = value,
            ["AZ"] = value => IMUTelemetry.AccelZ = value,
        };
    }
}