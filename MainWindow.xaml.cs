using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Drawing.Chart.Style;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using LicenseContext = OfficeOpenXml.LicenseContext;

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
            SetCamera();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

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
            Viewport.DataContext                = pr.integrator;

            border_UpperPlatform.DataContext    = pr.integrator;
            border_LowerPlatform.DataContext    = pr.integrator;
            border_Actuators.DataContext        = pr.actuatorsystem;
            border_NumberOfPoints.DataContext   = pr;
            border_PausePosition.DataContext    = pr.integrator;
            border_CenterOfRotation.DataContext = pr.integrator;
            txtbx_OffsetCoR.DataContext         = pr.integrator;

        }
        private void SetCamera()
        {
            Viewport.Camera.LookDirection = new Vector3D(-1000, 2000, -1000);
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

            //pr.integrator.Offset_Park = Properties.Settings.Default.Offset_Park;
            pr.integrator.Offset_Pause = Properties.Settings.Default.Offset_Pause;
            pr.integrator.Offset_CoR = Properties.Settings.Default.Offset_CoR;

            pr.numberOfPointsOnList = Properties.Settings.Default.NumberOfPoints;
        }
        private void SaveSettings()
        {
            Properties.Settings.Default.Dist_Lower_A = pr.integrator.Dist_A_Lower;
            Properties.Settings.Default.Dist_Lower_B = pr.integrator.Dist_B_Lower;
            Properties.Settings.Default.Dist_Upper_A = pr.integrator.Dist_A_Upper;
            Properties.Settings.Default.Dist_Upper_B = pr.integrator.Dist_B_Upper;

            Properties.Settings.Default.Stroke = pr.actuatorsystem.Stroke;
            Properties.Settings.Default.Min_Length = pr.actuatorsystem.MinLength;

            //Properties.Settings.Default.Offset_Park = pr.integrator.Offset_Park;
            Properties.Settings.Default.Offset_Pause = pr.integrator.Offset_Pause;
            Properties.Settings.Default.Offset_CoR = pr.integrator.Offset_CoR;

            Properties.Settings.Default.NumberOfPoints = pr.numberOfPointsOnList;

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
                SetButtonBusy();
                bw.RunWorkerAsync();
            }
        }

        void DoEnvelopeCalculations()
        {
            Create_XLSX(DOF.surge, DOF.heave);
            Create_XLSX(DOF.sway, DOF.surge);
            Create_XLSX(DOF.sway, DOF.heave);
            Create_XLSX(DOF.roll, DOF.pitch);
        }
        void EndWork()
        {
            SetButtonReady();
        }

        private void SetButtonBusy()
        {
            btn_calculate.Foreground = Brushes.LightCoral;
            btn_calculate.Content = "Busy...";
            btn_calculate.IsEnabled = false;
        }
        private void SetButtonReady()
        {
            Color color = (Color)ColorConverter.ConvertFromString("#FF000000");
            btn_calculate.Foreground = new SolidColorBrush(color);
            btn_calculate.Content = "Calculate Envelope";
            btn_calculate.IsEnabled = true;
        }
        
        void Create_CSV(List<Point> list, string name)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($";{name}");

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
        void Create_XLSX(DOF x_dof, DOF y_dof)
        {
            List<Point> PointList = pr.Explore(x_dof, y_dof);

            string sheetname = y_dof + " over " + x_dof;
            string filename = sheetname;

            FileInfo fileinfo = new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + 
                @"\Saved Games\Rig-O-Meter\" + filename + ".xlsx");
            
            if (!Directory.Exists(fileinfo.DirectoryName))  Directory.CreateDirectory(fileinfo.DirectoryName);
            if (fileinfo.Exists)                            fileinfo.Delete();                                      //Start blank

            

            using (var package = new ExcelPackage(fileinfo))
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add(sheetname);

                for (int i = 0; i < PointList.Count; i++)
                {
                    int row = i + 1;        //To start ar row 2
                    ws.Cells[row, 1].Value = PointList[i].X;
                    ws.Cells[row, 2].Value = PointList[i].Y;
                }

                var scatterChart = ws.Drawings.AddScatterChart("X-Y-Scatterplot", eScatterChartType.XYScatter);

                scatterChart.SetPosition(0, 0, 2, 0);
                scatterChart.SetSize(1020, 1000);

                scatterChart.Title.Overlay = true;
                scatterChart.Title.Text = sheetname;

                if (x_dof == DOF.surge || x_dof == DOF.heave || x_dof == DOF.sway )     //Is X translations?
                {
                    scatterChart.XAxis.MaxValue = 800;
                    scatterChart.XAxis.MinValue = -800;
                }
                else
                {
                    scatterChart.XAxis.MaxValue = 50;
                    scatterChart.XAxis.MinValue = -50;
                }

                if (y_dof == DOF.surge || y_dof == DOF.heave || y_dof == DOF.sway)     //Is Y translations?
                {
                    scatterChart.YAxis.MaxValue = 800;
                    scatterChart.YAxis.MinValue = -800;
                }
                else
                {
                    scatterChart.YAxis.MaxValue = 50;
                    scatterChart.YAxis.MinValue = -50;
                }

                scatterChart.XAxis.AddGridlines(true, false);
                scatterChart.YAxis.AddGridlines(true, false);

                var x_Values = ExcelRange.GetAddress(2, 1, PointList.Count, 1);
                var y_Values = ExcelRange.GetAddress(2, 2, PointList.Count, 2);
                
                ExcelScatterChartSerie EnvelopePlot = scatterChart.Series.Add(x_Values, y_Values);
                scatterChart.StyleManager.SetChartStyle(ePresetChartStyle.ScatterChartStyle9);
                EnvelopePlot.Marker.Size = 2;

                package.Save();
            }

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


