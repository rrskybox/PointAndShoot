using AstroMath;
using System;
using TheSky64Lib;

namespace PointAndShoot
{
    public class FOVMiracles
    {
        private GuideCamFOV gfov = null;
        private double gIsolation;
        private StarProspect gFOVCenter = new StarProspect();

        public FOVMiracles()
        {
            //Parses the guider FOV,
            // calculates its hold out area
            // calculates its 
            //Get the guider FOV data
            gfov = new GuideCamFOV();
            //Calculate the width of the FOV as the longest dimension of the FOV in arcmins and double it
            if (gfov.ArcMinSizeX > gfov.ArcMinSizeY)
            { gIsolation = gfov.ArcMinSizeX * 2; }
            else
            { gIsolation = gfov.ArcMinSizeY * 2; }

            return;
        }

        public string FOVName => gfov.Name;

        public double FOVRA => gFOVCenter.StarRA;

        public double FOVDec => gFOVCenter.StarDec;

        public double FOVIsolation => gIsolation;

        public StarProspect OffsetCenter(StarProspect tgtStar, double imagePA)
        {
            /*Calculates an offset position that places the input position in the center 
               * of the guider FOV.  
               * 
               * Note that the bearing for moving off the target star is opposite for different
               * sides of the pier.  If the scope is peering over the meridian, then this wont work
               * well.
               */

            //Set the star chart FOV 4 times the y distance (arcmin) center of the ccd
            //  fov to the center of the guider fov -- not really needed but helps viewing during processing
            SetStarChartSize();
            // Convert target star RA and Dec to radians and place in RA/Dec structure
            double tgtRA = Transform.HoursToRadians(tgtStar.StarRA);
            double tgtDec = Transform.DegreesToRadians(tgtStar.StarDec);
            Celestial.RADec tgtStarPosition = new Celestial.RADec(tgtRA, tgtDec);
            // Compute the rotation (radians) associated with any guider FOV offset from the image PA
            double gRotationR = -Math.Atan2(gfov.CenterX, gfov.CenterY);
            // Increment the image PA by the guider offset
            double bearingR = gRotationR + Transform.DegreesToRadians(imagePA);
            // Reverse bearing if the mount is pointing east
            //if (!pierEast) { bearingR = -bearingR; }
            // Compute the distance beween the center of the Image FOV and the center of the Guider FOV
            double distanceR = Transform.DegreesToRadians((SumOfSquares(gfov.CenterX, gfov.CenterY)) / 60.0);
            // Calculate an offset position such that the guider FOV will have the target star in its center
            //Celestial.RADec endPosition = Celestial.Travel(tgtStarPosition, -gVectorR, gRotationR);
            Celestial.RADec endPosition = Celestial.ComputePositionFromBearingAndRange(tgtStarPosition, bearingR, -distanceR);

            // Convert this position back to hours and degrees (RA and Dec) and return the result
            //  as a DBQStar structure
            StarProspect endStar = new StarProspect()
            {
                StarRA = Transform.RadiansToHours(endPosition.RA),
                StarDec = Transform.RadiansToDegrees(endPosition.Dec)
            };
            return endStar;
        }

        public void SetStarChartSize()
        {
            // Changes the StarChart width to four times the center-to-center distance
            //  between image FOV and guider FOV for esthetic purposes
            sky6StarChart tsxsc = new sky6StarChart();
            double widthViaCenters = (SumOfSquares(gfov.CenterX, gfov.CenterY) * 4) / 60.0;
            double widthViaSize = (SumOfSquares(gfov.ArcMinSizeX, gfov.ArcMinSizeY) * 4) / 60.0;
            if (widthViaCenters < widthViaSize)
            { tsxsc.FieldOfView = widthViaSize; }
            else
            { tsxsc.FieldOfView = widthViaCenters; }
            return;
        }

        private double SumOfSquares(double xVal, double yVal)
        {
            // Computes length of hypotenuse of right triangle in xVal and yVal
            return Math.Sqrt((xVal * xVal) + (yVal * yVal));
        }

    }
}
