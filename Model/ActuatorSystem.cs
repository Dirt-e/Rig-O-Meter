using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rig_O_Meter
{
    public class ActuatorSystem : MyObject
    {
        Actuator A1 = new Actuator();
        Actuator A2 = new Actuator();
        Actuator A3 = new Actuator();
        Actuator A4 = new Actuator();
        Actuator A5 = new Actuator();
        Actuator A6 = new Actuator();

        IK_Module IK_Module;

        //ViewModel:
        float _minlength = 970;
        public float MinLength
        {
            get { return _minlength; }
            set
            {
                if (value < 0) _minlength = 0;
                else _minlength = value;

                A1.MinLength = value;
                A2.MinLength = value;
                A3.MinLength = value;
                A4.MinLength = value;
                A5.MinLength = value;
                A6.MinLength = value;

                OnPropertyChanged("MinLength");
            }
        }
        float _stroke = 520;
        public float Stroke
        {
            get { return _stroke; }
            set
            {
                if (value > 0) _stroke = value;
                else _stroke = 1;

                A1.Stroke = value;
                A2.Stroke = value;
                A3.Stroke = value;
                A4.Stroke = value;
                A5.Stroke = value;
                A6.Stroke = value;

                OnPropertyChanged(nameof(Stroke));
            }
        }
        //Non-UI:
        public List<float> UtilisationList
        {
            get
            {
                List<float> UtilList = new List<float>
                {
                    A1.Utilisation,
                    A2.Utilisation,
                    A3.Utilisation,
                    A4.Utilisation,
                    A5.Utilisation,
                    A6.Utilisation
                };

                return UtilList;
            }
        }
        public bool IsPrettymuchLevel
        {
            get
            {
                //deternmine std. deviation...
                double StdDev = Utility.getStandardDeviation(UtilisationList);

                throw new NotImplementedException();
                //return StdDev < 0.1f;
            }
        }
        public bool AllInLimits
        {
            get
            {
                return (A1.InLimits &&
                        A2.InLimits &&
                        A3.InLimits &&
                        A4.InLimits &&
                        A5.InLimits &&
                        A6.InLimits);
            }
        }
        public bool Is_AllActuatorsFullyRetracted
        {
            get
            {
                return A1.Utilisation == 0 &&
                        A2.Utilisation == 0 &&
                        A3.Utilisation == 0 &&
                        A4.Utilisation == 0 &&
                        A5.Utilisation == 0 &&
                        A6.Utilisation == 0;
            }
        }

        public ActuatorSystem(IK_Module ikm)
        {
            IK_Module = ikm;

            A1 = new Actuator();
            A2 = new Actuator();
            A3 = new Actuator();
            A4 = new Actuator();
            A5 = new Actuator();
            A6 = new Actuator();

            A1.MinLength = MinLength;
            A2.MinLength = MinLength;
            A3.MinLength = MinLength;
            A4.MinLength = MinLength;
            A5.MinLength = MinLength;
            A6.MinLength = MinLength;

            A1.Stroke = Stroke;
            A2.Stroke = Stroke;
            A3.Stroke = Stroke;
            A4.Stroke = Stroke;
            A5.Stroke = Stroke;
            A6.Stroke = Stroke;
        }

        public void Update()
        {
            A1.CurrentLength = IK_Module.Lengths[0];
            A2.CurrentLength = IK_Module.Lengths[1];
            A3.CurrentLength = IK_Module.Lengths[2];
            A4.CurrentLength = IK_Module.Lengths[3];
            A5.CurrentLength = IK_Module.Lengths[4];
            A6.CurrentLength = IK_Module.Lengths[5];
        }
    }
}
