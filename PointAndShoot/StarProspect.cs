using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TheSky64Lib;

namespace PointAndShoot
{
  
    public class StarProspect
    {
        //Basically a structure for encapsulating a star database entry
        public StarProspect()
        {
            //Nothing really to do here upon instantiation
        }
        public string StarName { get; set; }
        public double StarRA { get; set; }
        public double StarDec { get; set; }
        public double StarMag { get; set; }
    }

}
