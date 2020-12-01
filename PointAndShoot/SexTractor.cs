using System;
using System.Collections.Generic;
using TheSkyXLib;

namespace PointAndShoot
{
    class SexTractor
    {
        // Added enumeration of inventory index because TSX doesn't
        public enum SourceExtractionType
        {
            sexX,
            sexY,
            sexMagnitude,
            sexClass,
            sexFWHM,
            sexMajorAxis,
            sexMinorAxis,
            sexTheta,
            sexEllipticity
        }

        public ccdsoftImage timg = null;

        public SexTractor()
        {
            //ccdsoftCamera tsxa = new ccdsoftCamera();
            timg = new ccdsoftImage();
            return;
        }

        public void Close()
        {
            return;
        }

        public int SourceExtractGuider()
        {
            int aStat = timg.AttachToActiveAutoguider();
            int iStat = timg.ShowInventory();
            return iStat;
        }

        //*** Converts an array of generic "objects" to an array of doubles
        private double[] ConvertDoubleArray(object[] oIn)
        {
            double[] dOut = new double[oIn.Length];
            for (int i = 0; i < oIn.Length; i++)
            {
                dOut[i] = Convert.ToDouble(oIn[i]);
            }
            return dOut;
        }

        //*** Converts an array of generic "objects" to a list of doubles
        private List<double> ConvertDoubleList(object[] oIn)
        {
            List<double> dOut = new List<double>();
            for (int i = 0; i < oIn.Length; i++)
            {
                dOut.Add(Convert.ToDouble(oIn[i]));
            }
            return dOut;
        }

        //*** returns the index of the largest value found in a list
        public int GetListLargest(List<double> iArray)
        {
            int idx = 0;
            for (int i = 0; i < iArray.Count; i++)
            {
                if (iArray[i] > iArray[idx]) { idx = i; }
            }
            return idx;
        }

        //*** Get the ADU value at pixel X,Y
        public double GetPixelADU(int xPix, int yPix)
        {
            //get the array height and width
            int aHeight = timg.HeightInPixels;
            int aWidth = timg.WidthInPixels;
            //need to some out of bounds checking here someday
            double[] aRow = timg.scanLine(yPix);
            double aVal = aRow[xPix];
            return aVal;
        }

        public double[] GetSourceExtractionArray(SourceExtractionType dataIndex)
        {
            {
                object[] iA = timg.InventoryArray((int)dataIndex);
                double[] sexArray = ConvertDoubleArray(iA);
                return sexArray;
            }

        }

        public List<double> GetSourceExtractionList(SourceExtractionType dataIndex)
        {
            {
                object[] iA = timg.InventoryArray((int)dataIndex);
                List<double> sexArray = ConvertDoubleList(iA);
                return sexArray;
            }

        }

        public int WidthInPixels => timg.WidthInPixels;

        public int HeightInPixels => timg.HeightInPixels;

    }
}
