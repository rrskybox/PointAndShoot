using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheSky64Lib;

namespace PointAndShoot
{
    public partial class FormPAS : Form
    {

        const double searchAreaDeg = 10;
        const string Version = "PointAndShoot V1.0";
        const double InitialGuiderExposure = 5; //Initial exposure level for guider camera images.
        const double OptGuiderADU = 20000; //Target ADU for guide star in guider camera images.

        public FormPAS()
        {
            InitializeComponent();
            StartButton.BackColor = Color.LightGreen;
            CloseButton.BackColor = Color.LightGreen;
            PickButton.Enabled = false;
            OptimizeButton.Enabled = false;
            CalibrateButton.Enabled = false;
        }

        //PointAndShoot Application for automated guider star selection, and optional calibration
        /*
         * This application locates a suitable calibration star for guider calibration,
         *   slews the mount to frame the star in the guider FOV,
         *   adjusts the guide camera exposure for the targer,
         *   runs a calibration.
         *   
         */

        public FOVMiracles guiderFOV;
        public double imagePA;
        public double optExposure = InitialGuiderExposure;

        /// <summary>
        /// Writes a line to the text box, similare to Console.Write
        /// </summary>
        /// <param name="logLline">String to be appended in text box</param>
        private void WriteLog(string logLine)
        {
            this.LogTextBox.AppendText(logLine + "\r\n");
            Show();
            System.Windows.Forms.Application.DoEvents();
            return;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            StartButton.BackColor = Color.LightSalmon;
            Show();
            System.Windows.Forms.Application.DoEvents();
            //plate solve current location to prime target star search and to acquire image camera position angle
            WriteLog("Plate Solving for current position and position angle");
            imagePA = PlateSolve();
            WriteLog("Position Angle: " + imagePA.ToString("0.000"));
            //Create an FOV object for the guider from the "My equipment.txt" Field of View Indicators file
            WriteLog("Parsing My equipment.txt file for FOVI definitions");
            guiderFOV = new FOVMiracles();
            WriteLog("Active Guider found: " + guiderFOV.FOVName);
            //Set the chart size for something pleasant around the FOVI's
            guiderFOV.SetStarChartSize();
            StartButton.BackColor = Color.LightGreen;
            PickButton.Enabled = true;
            PickButton.BackColor = Color.LightGreen;
            WriteLog("FOV Position Angle found");
        }

        private void PickButton_Click(object sender, EventArgs e)
        {
            PickButton.BackColor = Color.LightSalmon;
            StartButton.BackColor = Color.LightSalmon;
            //Pick up the target star location as selected in TSX Star Chart
            MessageBox.Show("Select target star");
            WriteLog("Picking up coordinates of selected target star");
            sky6ObjectInformation tsxoi = new sky6ObjectInformation();
            StarProspect foundStar = new StarProspect();
            tsxoi.Index = 0;
            tsxoi.Property(Sk6ObjectInformationProperty.sk6ObjInfoProp_NAME1);
            foundStar.StarName = (tsxoi.ObjInfoPropOut);
            tsxoi.Property(Sk6ObjectInformationProperty.sk6ObjInfoProp_RA_2000);
            foundStar.StarRA = (tsxoi.ObjInfoPropOut);
            tsxoi.Property(Sk6ObjectInformationProperty.sk6ObjInfoProp_DEC_2000);
            foundStar.StarDec = (tsxoi.ObjInfoPropOut);
            tsxoi.Property(Sk6ObjectInformationProperty.sk6ObjInfoProp_MAG);
            foundStar.StarMag = (tsxoi.ObjInfoPropOut);

            WriteLog("Target star found: " + foundStar.StarName);
            //Closed Loop Slew (following standard slew -- see notes) to the target guide star
            WriteLog("Centering imaging camera on target star");
            bool slewDone1 = SlewToStar(foundStar.StarName, foundStar.StarRA, foundStar.StarDec);
            if (slewDone1) WriteLog("Target star centered");
            else WriteLog("There was a problem centering the target star");
            //Calculate a pointing position that would put the target star in the guider FOV
            WriteLog("Calculating offset for centering star in guider FOV");
            StarProspect tgtPosition = guiderFOV.OffsetCenter(foundStar, imagePA);
            WriteLog("Offset calculated for pointing at " + tgtPosition.StarRA.ToString("0.000") + " , " + tgtPosition.StarDec.ToString("0.000"));

            //Closed Loop Slew (following standard slew -- see notes) to that offset position
            WriteLog("Centering target star in guider FOV");
            bool slewDone = SlewToPosition(tgtPosition.StarRA, tgtPosition.StarDec);
            if (slewDone1) WriteLog("Target star centered in guider FOV");
            else WriteLog("Could not recenter the target star");
            //plate solve current location -- not necessary but it sets up the star chart nicely for viewing
            //  note that we are not in such a hurry that we can't mess around a bit
            WriteLog("Checking offset position with a plate solve");
            imagePA = PlateSolve();
            //Reset the chart size for something pleasant around the FOVI's
            guiderFOV.SetStarChartSize();
            //center the star chart on the pointing location ==  once again, for esthetic purposes
            WriteLog("Recentering chart");
            sky6StarChart tsxsc = new sky6StarChart
            {
                RightAscension = tgtPosition.StarRA,
                Declination = tgtPosition.StarDec
            };
            WriteLog("Target star centered in Guider FOV");
            PickButton.BackColor = Color.LightGreen;
            StartButton.BackColor = Color.LightGreen;
            OptimizeButton.Enabled = true;
            CalibrateButton.Enabled = true;
            OptimizeButton.BackColor = Color.LightGreen;
            CalibrateButton.BackColor = Color.LightGreen;

            //all done
            return;
        }

        /// <summary>
        /// Optimize the exposure to produce an ADU of about 10K
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OptimizeButton_Click(object sender, EventArgs e)
        {
            //Take a guider image and adjust the exposure to an optimal level
            OptimizeButton.BackColor = Color.LightSalmon;
            WriteLog("Adjusting guider exposure to achieve ADU of " + OptGuiderADU.ToString());
            optExposure = OptimizeExposure(InitialGuiderExposure, OptGuiderADU);
            WriteLog("Best guider exposure determined to be " + optExposure.ToString("0.00") + " secs");
            OptimizeButton.BackColor = Color.LightGreen;
            return;
        }

        /// <summary>
        /// Calibrate DirectGuide for the guider
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CalibrateButton_Click(object sender, EventArgs e)
        {
            //Calibrate the guider
            CalibrateButton.BackColor = Color.LightSalmon;
            WriteLog("Starting guider calibration");
            string calDone = CalibrateGuideCam(optExposure, false); //No AO
            WriteLog("DirectGuide calibration complete");
            if (AOCheckBox.Checked)
            {
                calDone = CalibrateGuideCam(optExposure, true); //AO
                WriteLog("AO calibration complete");
            }
            CalibrateButton.BackColor = Color.LightGreen;
            return;
        }

        private double PlateSolve()
        {
            //runs an image link on the current location to get PA data
            //assume camera, mount etc are connected and properly configured.  
            ccdsoftCamera tsxcc = new ccdsoftCamera
            {
                Autoguider = 0,
                Frame = ccdsoftImageFrame.cdLight,
                ExposureTime = 10,
                Delay = 0,
                Asynchronous = 0,
                AutoSaveOn = 1,
                Subframe = 0
            };

            try { tsxcc.TakeImage(); }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
                return 0;
            }

            ccdsoftImage tsxi = new ccdsoftImage();
            ImageLink tsxil = new ImageLink
            {
                pathToFITS = tsxcc.LastImageFileName,
                scale = 1.70
            };
            try { tsxil.execute(); }
            catch (Exception ex)
            {
                return 0;
            }
            ImageLinkResults tsxir = new ImageLinkResults();
            double iPA = tsxir.imagePositionAngle;

            //Check for image link success, return 0 if not.
            if (tsxir.succeeded == 1)
            { return iPA; }
            else
            { return 0; }

        }


        static bool SlewToStar(string starName, double starRA, double starDec)
        {
            //Moves the mount to center the calibration star in the guider FOV
            //Async slew to target (letting dome catch up), then CLS to align (does not coordinate with dome)
            //
            //First, convert RA and Dec to topocentric (Epoch now) as that is what the slew expects
            sky6Utils tsxu = new sky6Utils();
            tsxu.Precess2000ToNow(starRA, starDec);
            starRA = tsxu.dOut0;
            starDec = tsxu.dOut1;
            sky6RASCOMTele tsxm = new sky6RASCOMTele
            {
                Asynchronous = 0
            };
            try
            {
                tsxm.SlewToRaDec(starRA, starDec, starName);
            }
            catch (Exception ex)
            { return false; }
            return true;
        }

        static bool SlewToPosition(double starRA, double starDec)
        {
            //Moves the mount to center the calibration star in the guider FOV
            //Async slew to target (letting dome catch up), then CLS to align (does not coordinate with dome)
            //
            sky6RASCOMTele tsxm = new sky6RASCOMTele
            {
                Asynchronous = 0
            };

            tsxm.SlewToRaDec(starRA, starDec, "Target Position");

            sky6StarChart tsxsc = new sky6StarChart();
            string RADecname = starRA.ToString() + "," + starDec.ToString();
            ClosedLoopSlew tsxcls = new ClosedLoopSlew();
            tsxsc.Find(RADecname);
            try
            { tsxcls.exec(); }
            catch (Exception ex)
            { return false; }
            return true;
        }

        static double OptimizeExposure(double exposure, double ADU)
        {
            //Adjusts the exposure level to the target ADu
            return (GuideControl.OptimizeExposure(exposure, ADU));
        }

        static bool SetSubFrame(double xCenter, double yCenter, double edgePixels)
        {
            //Sizes and centers the subframe of the guider imaging
            return true;
        }

        static string CalibrateGuideCam(double guiderExposure, bool AOEnabled)
        {
            //Launches the guider calibration
            return GuideControl.Calibrate(guiderExposure, AOEnabled);
        }

        private void AOCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            return;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
            return;
        }
    }


    /// <summary>
    /// Class GuideCamFOV 
    /// 
    /// Used to encapsulate all the FOV data and functions for guide camera FOV
    /// 
    /// </summary>
    public class GuideCamFOV
    {
        //Instantiation picks up the guider FOV size, center, offset and PA
        //Open "my equipment.txt" file in TSX.  Translate to XML version
        //  for ease of parsing contents

        //Guide camera FOVI name could be "Autoguider" but maybe not
        public const string GuiderName = "Autoguider";
        //Guide camera FOVI should be second element (zero based) -- this may be a must, don't know yet
        public const int GuiderElementNumber = 1;

        //Gonna pick up an instance of the FOV data
        private FOVX gFOV;

        public GuideCamFOV()
        {
            //Open and populate the FOV database from the TSX my equipment.txt file
            gFOV = new FOVX();
            //Get the FOV name found for the first active entry (should only be one)
            Name = gFOV.GetActiveFOVHeaderEntry(FOVX.Description1FieldXName);
            //Get the second FOV element name for this entry -- should be the guide camera
            string aGuiderName = gFOV.GetActiveFOVElementEntry(GuiderElementNumber, FOVX.ElementDescriptionFieldXName);
            //Populate the position fields for the FOV
            PA = Convert.ToDouble(gFOV.GetActiveFOVHeaderEntry(FOVX.PositionAngleFieldXName));
            CenterX = Convert.ToDouble(gFOV.GetActiveFOVElementEntry(GuiderElementNumber, FOVX.CenterOffsetXFieldXName));
            CenterY = Convert.ToDouble(gFOV.GetActiveFOVElementEntry(GuiderElementNumber, FOVX.CenterOffsetYFieldXName));
            PixelSizeX = Convert.ToDouble(gFOV.GetActiveFOVElementEntry(GuiderElementNumber, FOVX.PixelsXFieldXName));
            PixelSizeY = Convert.ToDouble(gFOV.GetActiveFOVElementEntry(GuiderElementNumber, FOVX.PixelsYFieldXName));
            ArcMinSizeX = Convert.ToDouble(gFOV.GetActiveFOVElementEntry(GuiderElementNumber, FOVX.SizeXFieldXName));
            ArcMinSizeY = Convert.ToDouble(gFOV.GetActiveFOVElementEntry(GuiderElementNumber, FOVX.SizeYFieldXName));
            return;
        }

        //Create automatic properties to hold FOV data in the class instance
        public string Name { get; set; }
        public double PA { get; set; }
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double PixelSizeX { get; set; }
        public double PixelSizeY { get; set; }
        public double ArcMinSizeX { get; set; }
        public double ArcMinSizeY { get; set; }
    }
}



