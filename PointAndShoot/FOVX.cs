using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace PointAndShoot
{
    public class FOVX
    {
        const int headerLength = 11;
        const int elementLength = 8;

        public const string ActiveFieldXName = "Active";
        public const string ReferenceFrameFieldXName = "ReferenceFrame";
        public const string Description1FieldXName = "Description1";
        public const string PositionAngleFieldXName = "PositionAngle";
        public const string OffsetXFieldXName = "OffsetX";
        public const string OffsetYFieldXName = "OffsetY";
        public const string ScaleFieldXName = "Scale";
        public const string EnabledFieldXName = "Enabled";
        public const string Description2FieldXName = "Description2";
        public const string UnitsFieldXName = "Units";

        public const string ShapeFieldXName = "Shape";
        public const string ElementDescriptionFieldXName = "ElementDescription";
        public const string SizeXFieldXName = "SizeX";
        public const string SizeYFieldXName = "SizeY";
        public const string PixelsXFieldXName = "PixelsX";
        public const string PixelsYFieldXName = "PixelsY";
        public const string CenterOffsetXFieldXName = "CenterOffsetX";
        public const string CenterOffsetYFieldXName = "CenterOffsetY";
        public const string LinkedFieldXName = "Linked";
        public const string ReversedFieldXName = "Reversed";

        public const string FOVElementNumberXName = "FOVElementNumber";

        public const string FOVIndicatorXName = "FOVIndicator";
        public const string FOVElementXName = "FOVElement";

        public string fovdir;
        public string fovfile;
        public string fovXfile;
        public XElement xFovList;

        public FOVX()
        {

            fovdir = "C:\\Users\\" + System.Environment.UserName + "\\Documents\\Software Bisque\\TheSky Professional Edition 64\\Field of View Indicators";
            fovfile = fovdir + "\\My Equipment.txt";
            fovXfile = fovdir + "\\My Equipment.xml";
            System.IO.TextReader fovDataFile = System.IO.File.OpenText(fovfile);
            //create xml object
            xFovList = new XElement("FieldOfViewIndicators");

            string fovline = fovDataFile.ReadLine();
            //skip past all field definition lines for now, maybe forever
            while (fovline != null)
            {
                if ((fovline.Contains("[F]")) && (fovline[0] != ';'))
                {
                    XElement xfovI = new XElement(FOVIndicatorXName);
                    string[] splitline;
                    int fElementCount;

                    splitline = fovline.Split('|');
                    fElementCount = (splitline.Length - headerLength) / elementLength;
                    xfovI.Add(new XElement(ActiveFieldXName, splitline[1]));
                    xfovI.Add(new XElement(ReferenceFrameFieldXName, splitline[2]));
                    xfovI.Add(new XElement(Description1FieldXName, splitline[3]));
                    xfovI.Add(new XElement(PositionAngleFieldXName, splitline[4]));
                    xfovI.Add(new XElement(OffsetXFieldXName, splitline[5]));
                    xfovI.Add(new XElement(OffsetYFieldXName, splitline[6]));
                    xfovI.Add(new XElement(ScaleFieldXName, splitline[7]));
                    xfovI.Add(new XElement(EnabledFieldXName, splitline[8]));
                    xfovI.Add(new XElement(Description2FieldXName, splitline[9]));
                    xfovI.Add(new XElement(UnitsFieldXName, splitline[10]));

                    for (int elm = 0; elm < fElementCount; elm++)
                    {
                        int splitIndx = elementLength * elm + headerLength;
                        XElement xelm = new XElement(FOVElementXName);
                        xelm.Add(new XElement(ShapeFieldXName, splitline[splitIndx + 0]));
                        xelm.Add(new XElement(ElementDescriptionFieldXName, splitline[splitIndx + 1]));
                        xelm.Add(new XElement(SizeXFieldXName, splitline[splitIndx + 2]));
                        xelm.Add(new XElement(SizeYFieldXName, splitline[splitIndx + 3]));
                        xelm.Add(new XElement(PixelsXFieldXName, splitline[splitIndx + 4]));
                        xelm.Add(new XElement(PixelsYFieldXName, splitline[splitIndx + 5]));
                        xelm.Add(new XElement(CenterOffsetXFieldXName, splitline[splitIndx + 6]));
                        xelm.Add(new XElement(CenterOffsetYFieldXName, splitline[splitIndx + 7]));
                        //xelm.Add(new XElement(Field_19, splitline(splitIndx + 8)));
                        //xelm.Add(new XElement(Field_20, splitline(splitIndx + 9)));
                        xelm.Add(new XElement(FOVElementNumberXName, elm.ToString()));
                        xfovI.Add(xelm);
                    }
                    xFovList.Add(xfovI);
                }
                fovline = fovDataFile.ReadLine();
            }
            xFovList.Save(fovXfile);
            fovDataFile.Close();
            return;
        }

        public string GetActiveFOVHeaderEntry(string headerEntryName)
        {
            //Get contents of the element named "headingEntryName" in first active FOV record
            //if no entry, then return null
            foreach (XElement fovEntry in xFovList.Elements(FOVIndicatorXName))
            {
                if (fovEntry.Element(ActiveFieldXName).Value == "1")
                {
                    XElement hdrElement = fovEntry.Element(headerEntryName);
                    return (hdrElement.Value);
                }
            }
            return null;
        }

        public string GetActiveFOVElementEntry(int fovIndicatorElementNumber, string fovIndicatorElementComponent)
        {
            //Get content of an specific element based on element number, of first active fov
            foreach (XElement xfovEntry in xFovList.Elements(FOVIndicatorXName))
            {
                if (Convert.ToInt16(xfovEntry.Element(ActiveFieldXName).Value) == 1)
                {
                    foreach (XElement xfovelm in xfovEntry.Elements(FOVElementXName))
                    {
                        int testx = Convert.ToInt16(xfovelm.Element(FOVElementNumberXName).Value);
                        if (Convert.ToInt16(xfovelm.Element(FOVElementNumberXName).Value) == fovIndicatorElementNumber)
                        {
                            return (xfovelm.Element(fovIndicatorElementComponent).Value);
                        }
                    }
                }
            }
            return (null);
        }

        public string GetActiveFOVElementEntry(string fovIndicatorElementName, string fovIndicatorElementComponent)
        {
            //Get content of an specific element based on element name, of first active fov
            //
            //Look through the fovi entries
            foreach (XElement xfovEntry in xFovList.Elements(FOVIndicatorXName))
            {
                //Find the first active entry
                if (Convert.ToInt16(xfovEntry.Element(ActiveFieldXName).Value) == 1)
                {
                    //Look through all the fov elements in this active entry
                    foreach (XElement xfovelm in xfovEntry.Elements(FOVElementXName))
                    {
                        //find the first fov element that matches the fovIndicatorElementName
                        if (xfovelm.Element(ElementDescriptionFieldXName).Value == fovIndicatorElementName)
                        {
                            //if it matches, then return the contents
                            return (xfovelm.Element(fovIndicatorElementComponent).Value);
                        }
                    }
                }
            }
            return (null);
        }
    }
}

