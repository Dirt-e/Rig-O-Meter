using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Rig_O_Meter
{
    public partial class MainWindow : Window
    {
        PhantomRig pr = new PhantomRig();
        BackgroundWorker bw = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            SetDataContexts();

            Title = "Rig-O-Meter " + getRunningVersion().ToString();

            bw.DoWork += (object sender, DoWorkEventArgs e) =>
            {
                DoEnvelopeCalculations();
            };
            bw.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
            {
                EndWork();
            };
        }

        private void SetDataContexts()
        {
            border_UpperPlatform.DataContext = pr.integrator;
            border_LowerPlatform.DataContext = pr.integrator;
            border_Actuators.DataContext = pr.actuatorsystem;
            //border_Park_Position.DataContext = pr.integrator;
            border_PausePosition.DataContext = pr.integrator;
            border_CenterOfRotation.DataContext = pr.integrator;

            txtbx_OffsetCoR.DataContext = pr.integrator;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSettings();
        }


        private void LoadSettings()
        {
            pr.integrator.Dist_A_Lower = Properties.Settings.Default.Dist_Lower_A;
            pr.integrator.Dist_B_Lower = Properties.Settings.Default.Dist_Lower_B;
            pr.integrator.Dist_A_Upper = Properties.Settings.Default.Dist_Upper_A;
            pr.integrator.Dist_B_Upper = Properties.Settings.Default.Dist_Upper_B;

            pr.actuatorsystem.Stroke = Properties.Settings.Default.Stroke;
            pr.actuatorsystem.MinLength = Properties.Settings.Default.Min_Length;

            pr.integrator.Offset_Park = Properties.Settings.Default.Offset_Park;
            pr.integrator.Offset_Pause = Properties.Settings.Default.Offset_Pause;
            pr.integrator.Offset_CoR = Properties.Settings.Default.Offset_CoR;
        }
        private void SaveSettings()
        {
            Properties.Settings.Default.Dist_Lower_A = pr.integrator.Dist_A_Lower;
            Properties.Settings.Default.Dist_Lower_B = pr.integrator.Dist_B_Lower;
            Properties.Settings.Default.Dist_Upper_A = pr.integrator.Dist_A_Upper;
            Properties.Settings.Default.Dist_Upper_B = pr.integrator.Dist_B_Upper;

            Properties.Settings.Default.Stroke = pr.actuatorsystem.Stroke;
            Properties.Settings.Default.Min_Length = pr.actuatorsystem.MinLength;

            Properties.Settings.Default.Offset_Park = pr.integrator.Offset_Park;
            Properties.Settings.Default.Offset_Pause = pr.integrator.Offset_Pause;
            Properties.Settings.Default.Offset_CoR = pr.integrator.Offset_CoR;

            Properties.Settings.Default.Save();
        }

        private void CalibPark_Click(object sender, RoutedEventArgs e)
        {
            pr.calibrate();
        }
        private void CalibPause_Click(object sender, RoutedEventArgs e)
        {
            pr.calibrate();
        }

        private void CalculateEnvelope_Click(object sender, RoutedEventArgs e)
        {
            if (!bw.IsBusy)
            {
                StartWork();
            }
        }

        void StartWork()
        {
            btn_calculate.Foreground = Brushes.LightCoral;
            btn_calculate.Content = "Busy...";
            btn_calculate.IsEnabled = false;
            bw.RunWorkerAsync();
        }
        void DoEnvelopeCalculations()
        {
            List<Point> Heave_Surge = pr.Explore(DOF.heave, DOF.surge);
            List<Point> Heave_Sway = pr.Explore(DOF.heave, DOF.sway);
            List<Point> Surge_Sway = pr.Explore(DOF.surge, DOF.sway);
            List<Point> Pitch_Roll = pr.Explore(DOF.pitch, DOF.roll);

            PrintList(Heave_Surge, nameof(Heave_Surge));
            PrintList(Heave_Sway, nameof(Heave_Sway));
            PrintList(Surge_Sway, nameof(Surge_Sway));
            PrintList(Pitch_Roll, nameof(Pitch_Roll));
        }
        void EndWork()
        {
            Color color = (Color)ColorConverter.ConvertFromString("#FF000000");
            btn_calculate.Foreground = new SolidColorBrush(color);
            btn_calculate.Content = "Calculate Envelope";
            btn_calculate.IsEnabled = true;
        }

        void PrintList(List<Point> list, string name)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Point p in list)
            {
                sb.AppendLine(p.ToString());
            }

            string dir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) +
                                    "\\Saved Games\\Rig-O-Meter\\";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            string filepath = Path.Combine(dir, name) + ".csv";

            File.WriteAllText(filepath, sb.ToString());
        }
        Version getRunningVersion()
        {
            try
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch (Exception)
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }
    }
}
