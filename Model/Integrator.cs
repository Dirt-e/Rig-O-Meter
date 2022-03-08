using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using static Utility;

namespace Rig_O_Meter
{
    public class Integrator : MyObject
    {
        public DOF_Data Input = new DOF_Data();

        #region Geometry
        float _dist_a_upper = 1136;
        public float Dist_A_Upper
        {
            get { return _dist_a_upper; }
            set
            {
                _dist_a_upper = value;
                OnPropertyChanged(nameof(Dist_A_Upper));
                UpdateViewModel();
            }
        }

        float _dist_b_upper = 178;
        public float Dist_B_Upper
        {
            get { return _dist_b_upper; }
            set
            {
                _dist_b_upper = value;
                OnPropertyChanged(nameof(Dist_B_Upper));
                UpdateViewModel();
            }
        }

        float _dist_a_lower = 110;
        public float Dist_A_Lower
        {
            get { return _dist_a_lower; }
            set
            {
                _dist_a_lower = value;
                OnPropertyChanged(nameof(Dist_A_Lower));
                UpdateViewModel();
            }
        }

        float _dist_b_lower = 1391;
        public float Dist_B_Lower
        {
            get { return _dist_b_lower; }
            set
            {
                _dist_b_lower = value;
                OnPropertyChanged(nameof(Dist_B_Lower));
                UpdateViewModel();
            }
        }

        float _offset_park = 500;
        public float Offset_Park
        {
            get { return _offset_park; }
            set
            {
                _offset_park = value;
                OnPropertyChanged(nameof(Offset_Park));
                UpdateViewModel();
            }
        }

        float _offset_pause = 999;
        public float Offset_Pause
        {
            get { return _offset_pause; }
            set
            {
                _offset_pause = value;
                OnPropertyChanged(nameof(Offset_Pause));
                UpdateViewModel();
            }
        }

        float _offset_cor = 299;
        public float Offset_CoR
        {
            get { return _offset_cor; }
            set
            {
                _offset_cor = value;
                OnPropertyChanged(nameof(Offset_CoR));
                UpdateViewModel();
            }
        }
        #endregion

        #region MyTransforms
        public MyTransform World;
        public MyTransform Plat_Fix_Base;
        public MyTransform Plat_Fix_Pause;
        public MyTransform Plat_CoR;
        public MyTransform Plat_LFC;
        public MyTransform Plat_HFC;
        public MyTransform Plat_Motion;
        public ConnectingPoints UpperPoints;
        public ConnectingPoints LowerPoints;
        public MyTransform Plat_Fix_Park;
        #endregion

        //ViewModel:
        #region SPHERES (BASE)
        Point3D _lower1;
        public Point3D Lower1
        {
            get { return _lower1; }
            set { _lower1 = value; OnPropertyChanged("Lower1"); }
        }
        Point3D _lower2;
        public Point3D Lower2
        {
            get { return _lower2; }
            set { _lower2 = value; OnPropertyChanged("Lower2"); }
        }
        Point3D _lower3;
        public Point3D Lower3
        {
            get { return _lower3; }
            set { _lower3 = value; OnPropertyChanged("Lower3"); }
        }
        Point3D _lower4;
        public Point3D Lower4
        {
            get { return _lower4; }
            set { _lower4 = value; OnPropertyChanged("Lower4"); }
        }
        Point3D _lower5;
        public Point3D Lower5
        {
            get { return _lower5; }
            set { _lower5 = value; OnPropertyChanged("Lower5"); }
        }
        Point3D _lower6;
        public Point3D Lower6
        {
            get { return _lower6; }
            set { _lower6 = value; OnPropertyChanged("Lower6"); }
        }
        #endregion

        #region SPHERES (UPPER)
        Point3D _Upper1;
        public Point3D Upper1
        {
            get { return _Upper1; }
            set { _Upper1 = value; OnPropertyChanged("Upper1"); }
        }
        Point3D _Upper2;
        public Point3D Upper2
        {
            get { return _Upper2; }
            set { _Upper2 = value; OnPropertyChanged("Upper2"); }
        }
        Point3D _Upper3;
        public Point3D Upper3
        {
            get { return _Upper3; }
            set { _Upper3 = value; OnPropertyChanged("Upper3"); }
        }
        Point3D _Upper4;
        public Point3D Upper4
        {
            get { return _Upper4; }
            set { _Upper4 = value; OnPropertyChanged("Upper4"); }
        }
        Point3D _Upper5;
        public Point3D Upper5
        {
            get { return _Upper5; }
            set { _Upper5 = value; OnPropertyChanged("Upper5"); }
        }
        Point3D _Upper6;
        public Point3D Upper6
        {
            get { return _Upper6; }
            set { _Upper6 = value; OnPropertyChanged("Upper6"); }
        }
        #endregion

        #region Actuator Colors
        //These actuator colors are being updated via a Dispatcher callback.
        ActuatorStatus _act1_status = ActuatorStatus.TooLong;
        public ActuatorStatus Act1_Status
        {
            get { return _act1_status; }
            set { _act1_status = value; OnPropertyChanged(nameof(Act1_Status)); }
        }
        ActuatorStatus _act2_status = ActuatorStatus.TooLong;
        public ActuatorStatus Act2_Status
        {
            get { return _act2_status; }
            set { _act2_status = value; OnPropertyChanged(nameof(Act2_Status)); }
        }
        ActuatorStatus _act3_status = ActuatorStatus.TooLong;
        public ActuatorStatus Act3_Status
        {
            get { return _act3_status; }
            set { _act3_status = value; OnPropertyChanged(nameof(Act3_Status)); }
        }
        ActuatorStatus _act4_status = ActuatorStatus.TooLong;
        public ActuatorStatus Act4_Status
        {
            get { return _act4_status; }
            set { _act4_status = value; OnPropertyChanged(nameof(Act4_Status)); }
        }
        ActuatorStatus _act5_status = ActuatorStatus.TooLong;
        public ActuatorStatus Act5_Status
        {
            get { return _act5_status; }
            set { _act5_status = value; OnPropertyChanged(nameof(Act5_Status)); }
        }
        ActuatorStatus _act6_status = ActuatorStatus.TooLong;
        public ActuatorStatus Act6_Status
        {
            get { return _act6_status; }
            set { _act6_status = value; OnPropertyChanged(nameof(Act6_Status)); }
        }
        #endregion

        //Constructor
        public Integrator()
        {
            World = new MyTransform();
            Plat_Fix_Base = new MyTransform();  //(0% extension position)
            Plat_Fix_Pause = new MyTransform(); //(50% extension position)
            Plat_CoR = new MyTransform();
            Plat_LFC = new MyTransform();
            Plat_HFC = new MyTransform();
            Plat_Motion = new MyTransform();
            Plat_Fix_Park = new MyTransform();
            LowerPoints = new ConnectingPoints();
            UpperPoints = new ConnectingPoints();

            EstablishHierarchy();
        }

        private void EstablishHierarchy()
        {
            World.IsParentOf(Plat_Fix_Base);
            Plat_Fix_Base.IsParentOf(Plat_Fix_Park);
            Plat_Fix_Base.IsParentOf(Plat_Fix_Pause);
            Plat_Fix_Pause.IsParentOf(Plat_CoR);
            Plat_CoR.IsParentOf(Plat_LFC);
            Plat_LFC.IsParentOf(Plat_HFC);
            Plat_HFC.IsParentOf(Plat_Motion);
            Plat_Motion.IsParentOf(UpperPoints);
            Plat_Fix_Base.IsParentOf(LowerPoints);
        }

        public void Update()
        {
            Integrate_Platforms();
        }
        public void Update(DOF_Data data)
        {
            Input = new DOF_Data(data);
            Integrate_Platforms();
        }

        private void Integrate_Platforms()
        {
            UpperPoints.Dist_A = Dist_A_Upper;
            UpperPoints.Dist_B = Dist_B_Upper;

            LowerPoints.Dist_A = Dist_A_Lower;
            LowerPoints.Dist_B = Dist_B_Lower;

            Plat_Fix_Park.SetTranslation(0,
                                            0,
                                            Offset_Park);

            Plat_Fix_Pause.SetTranslation(0,
                                            0,
                                            Offset_Pause);

            Plat_CoR.SetTranslation(0,
                                            0,
                                            Offset_CoR);

            Plat_LFC.SetOrientation(0,
                                            RAD_from_DEG(Input.LFC_Pitch),
                                            -RAD_from_DEG(Input.LFC_Roll));     //Negative sign, because a positive accel (right) shall tilt the platform towards negative roll (left)

            Plat_HFC.SetTranslation(Input.HFC_Sway,
                                            Input.HFC_Surge,
                                            Input.HFC_Heave);
            Plat_HFC.SetOrientation(RAD_from_DEG(Input.HFC_Yaw),
                                            RAD_from_DEG(Input.HFC_Pitch),
                                            RAD_from_DEG(Input.HFC_Roll));

            Plat_Motion.SetTranslation(0,
                                            0,
                                            -Offset_CoR);
        }

        private void UpdateViewModel()
        {   
            Integrate_Platforms();
            
            //Lower Points:
            var low1 = LowerPoints.P1.GetWorldTransform();
            var low2 = LowerPoints.P2.GetWorldTransform();
            var low3 = LowerPoints.P3.GetWorldTransform();
            var low4 = LowerPoints.P4.GetWorldTransform();
            var low5 = LowerPoints.P5.GetWorldTransform();
            var low6 = LowerPoints.P6.GetWorldTransform();

            Lower1 = new Point3D(low1.Value.OffsetX, low1.Value.OffsetY, low1.Value.OffsetZ);
            Lower2 = new Point3D(low2.Value.OffsetX, low2.Value.OffsetY, low2.Value.OffsetZ);
            Lower3 = new Point3D(low3.Value.OffsetX, low3.Value.OffsetY, low3.Value.OffsetZ);
            Lower4 = new Point3D(low4.Value.OffsetX, low4.Value.OffsetY, low4.Value.OffsetZ);
            Lower5 = new Point3D(low5.Value.OffsetX, low5.Value.OffsetY, low5.Value.OffsetZ);
            Lower6 = new Point3D(low6.Value.OffsetX, low6.Value.OffsetY, low6.Value.OffsetZ);

            //Upper Points:
            var upp1 = UpperPoints.P1.GetWorldTransform();
            var upp2 = UpperPoints.P2.GetWorldTransform();
            var upp3 = UpperPoints.P3.GetWorldTransform();
            var upp4 = UpperPoints.P4.GetWorldTransform();
            var upp5 = UpperPoints.P5.GetWorldTransform();
            var upp6 = UpperPoints.P6.GetWorldTransform();

            Upper1 = new Point3D(upp1.Value.OffsetX, upp1.Value.OffsetY, upp1.Value.OffsetZ);
            Upper2 = new Point3D(upp2.Value.OffsetX, upp2.Value.OffsetY, upp2.Value.OffsetZ);
            Upper3 = new Point3D(upp3.Value.OffsetX, upp3.Value.OffsetY, upp3.Value.OffsetZ);
            Upper4 = new Point3D(upp4.Value.OffsetX, upp4.Value.OffsetY, upp4.Value.OffsetZ);
            Upper5 = new Point3D(upp5.Value.OffsetX, upp5.Value.OffsetY, upp5.Value.OffsetZ);
            Upper6 = new Point3D(upp6.Value.OffsetX, upp6.Value.OffsetY, upp6.Value.OffsetZ);
        }
    }
}
