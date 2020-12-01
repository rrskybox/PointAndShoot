using System;
using System.Collections.Generic;
using TheSkyXLib;

namespace PointAndShoot
{
    class GuideControl
    {
        //Optimize Exposure determines the best exposure time for the target star
        public static double OptimizeExposure(double initialExposure, double tgtADU)
        {
            //Get the best exposure time based on the target ADU
            //
            //Subrountine loops up to 4 times, taking an image, and calulating a new exposure that comes closest to meeting ADU goal
            //
            //Take an image with current exposure set from last guider exposure
            //If the max ADu is 100 or less, then there was no star at all.  Double the exposure upto MaxGuiderExposure
            //If the returned exposure is 64000 or more, then the star was probably saturated.  Halve the exposure down to no less than the minguider exposure.
            //If not within 20% of targetADU, then recalculate and rerun
            //If within 20% then recalculate and done, then update the exposure settings and return the exposure
            //

            const double maxExposure = 10;
            const double minExposure = 0.01;

            //Only take at max 4 shots at getting a good exposure, otherwise return the max or min
            for (int i = 0; i < 4; i++)
            {
                //Start at the initial exposure.  This is a minimum.
                //Take a subframe image
                //Get maximum pixels ADU
                //Will not support AO guider for now
                //
                double maxPixel = GuiderStarADUSex(initialExposure, false);  //Uses SexTractor engine
                //Check through too low, too high and just right
                if (maxPixel < 500)  //way too low
                {
                    initialExposure = LimitMaxMin((initialExposure * 2), maxExposure, minExposure);
                }
                else if (maxPixel > 60000.0)  //too close to saturation
                {
                    initialExposure = LimitMaxMin((initialExposure / 2), maxExposure, minExposure);
                }
                else if (!(CloseEnough(maxPixel, tgtADU, 20.0)))  //if not quite close enought recalculate exposure try again
                {
                    if (maxPixel > tgtADU)
                    {
                        initialExposure = LimitMaxMin(((tgtADU / maxPixel) * initialExposure), maxExposure, minExposure);
                    }
                    else
                    {
                        initialExposure = LimitMaxMin(((tgtADU / maxPixel) * initialExposure), maxExposure, minExposure);
                    }
                }
                else
                {
                    initialExposure = LimitMaxMin(((tgtADU / maxPixel) * initialExposure), maxExposure, minExposure);
                    break;
                }
            }
            return initialExposure;
        }

        //Runs the guider through the calibration routine
        public static string Calibrate(double calExposure, bool AOEnabled)
        {
            //Run autocalibration routine
            ccdsoftCamera tsxgc = new ccdsoftCamera
            {
                Autoguider = 1,
                AutoSaveOn = 0,
                Frame = ccdsoftImageFrame.cdLight,
                Delay = 2.0,
                AutoguiderExposureTime = calExposure,
                ExposureTime = calExposure
            };
            if (AOEnabled)
            {
                try
                { tsxgc.Calibrate(1); }
                catch (Exception ex)
                {
                    return (ex.Message);
                }
            }
            else
            {
                try
                { tsxgc.Calibrate(0); }
                catch (Exception ex)
                {
                    return (ex.Message);
                }
            }
            return "Calibration Complete";
        }

        //SexTractor-based method for getting the ADU of the brightest star in an image
        public static double GuiderStarADUSex(double exposure, bool AOEnabled)
        {
            //Determines the ADU for the X/Y centroid of the maximum FWHM star in a subframe
            //
            //Take a full frame image on the guider using TSX guide star coordinates and trackbox size 
            ccdsoftCamera tsxg = new ccdsoftCamera
            {
                Autoguider = 1,
                Frame = ccdsoftImageFrame.cdLight,
                Delay = 0,
                Asynchronous = 0,
                AutoSaveOn = 1,
                AutoguiderExposureTime = exposure,
                ExposureTime = exposure
            };
            int tstat = tsxg.TakeImage();
            if (tstat != 0) { return 0; }

            //Next step is to generate the collection of stars in the frame

            SexTractor sEx = new SexTractor();
            sEx.SourceExtractGuider();

            List<double> FWHMlist = sEx.GetSourceExtractionList(SexTractor.SourceExtractionType.sexFWHM);
            List<double> CenterX = sEx.GetSourceExtractionList(SexTractor.SourceExtractionType.sexX);
            List<double> CenterY = sEx.GetSourceExtractionList(SexTractor.SourceExtractionType.sexY);
            int iMax = sEx.GetListLargest(FWHMlist);

            double maxStarADU = 0;
            try
            {
                maxStarADU = sEx.GetPixelADU((int)CenterX[iMax], (int)CenterY[iMax]);
            }
            catch (Exception ex)
            {
                maxStarADU = 0;
            }
            tstat = tsxg.TakeImage();
            if (maxStarADU == 0)
            { maxStarADU = tsxg.MaximumPixel; }
            return maxStarADU;
        }

        //Brackets a value between a max value and a min value
        public static double LimitMaxMin(double inVal, double maxLimit, double minLimit)
        {
            //Returns InVal, limited by max and min
            if (inVal > maxLimit)
            { return maxLimit; }
            if (inVal < minLimit)
            { return minLimit; }
            return inVal;
        }

        //Tests whether a value is within a specified percentage of another value
        public static bool CloseEnough(double testval, double targetval, double percentnear)
        {
            //Cute little method for determining if a value is withing a certain percentatge of
            // another value.
            //testval is the value under consideration
            //targetval is the value to be tested against
            //npercentnear is how close (in percent of target val, i.e. x100) the two need to be within to test true
            // otherwise returns false

            if (Math.Abs(targetval - testval) <= Math.Abs((targetval * percentnear / 100)))
            { return true; }
            else
            { return false; }
        }

    }
}
