using OpenCvSharp;
using Ratel.Vision.Mura;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatelMura
{
    internal static class FilterHelpers
    {
        //public static Mat MakeMat(int h, int w, IEnumerable<MuraUnitDefect> defects, int nx, int xy)
        //{
        //    var mat = new Mat(h, w, MatType.CV_8UC1, 0);
        //    foreach (var d in defects)
        //    {
        //        for (int y = 0; y < xy; y++)
        //        {
        //            var yy = d.Rect.Y + y;
        //            if (yy >= h)
        //                continue;
        //            for (int x = 0; x < nx; x++)
        //            {
        //                var xx = d.Rect.X + x;
        //                if (xx >= w)
        //                    continue;
        //                mat.At<byte>(yy, xx) = 255;
        //            }
        //        }
        //    }
        //    return mat;
        //}

        public static MuraConfig MakeHoriConfig(int cx, int cy, double blackFactor, double whiteFactor, double angle = 0,
            double dpitch = 1, double minLevelPercent = 1.0, int diffCount = 2)
        {
            var conf = new MuraConfig
            {
                Use = true,
                SizeX = cx,
                SizeY = cy,
                BlackFactor = blackFactor,
                WhiteFactor = whiteFactor,
                Angle = angle,
                MergeGroup = "Hori",
                DiffCount = diffCount,
            };

            if (dpitch >= 4)
                dpitch = (int)dpitch;
            conf.DefectPoints.Add(new SurConfig(0, 0));
            conf.SurPoints.Add(new SurConfig(0, -dpitch, CompareOption.Count, minLevelPercent));
            conf.SurPoints.Add(new SurConfig(0, dpitch, CompareOption.Count, minLevelPercent));
            return conf;
        }

        public static MuraConfig MakeCir4Config(int cx, int cy, double blackFactor, double whiteFactor, double angle = 0,
            double dpitch = 1, double minLevelPercent = 1.0, int diffCount = 2)
        {
            var conf = new MuraConfig
            {
                Use = true,
                SizeX = cx,
                SizeY = cy,
                BlackFactor = blackFactor,
                WhiteFactor = whiteFactor,
                Angle = angle,
                MergeGroup = "Cir",
                DiffCount = diffCount,
            };
            if (dpitch >= conf.StepX)
                dpitch = dpitch / conf.StepX;
            conf.DefectPoints.Add(new SurConfig(0, 0));
            conf.SurPoints.Add(new SurConfig(0, -dpitch, CompareOption.Count, minLevelPercent));
            conf.SurPoints.Add(new SurConfig(0, dpitch, CompareOption.Count, minLevelPercent));
            conf.SurPoints.Add(new SurConfig(-dpitch, 0, CompareOption.Count, minLevelPercent));
            conf.SurPoints.Add(new SurConfig(dpitch, 0, CompareOption.Count, minLevelPercent));
            return conf;
        }

        public static MuraConfig MakeCir8Config(int sizeX, int sizeY, double blackFactor, double whiteFactor, double angle = 0,
            double dpitch = 1, double minLevelPercent = 1.0, int diffCount = 2)
        {
            var conf = new MuraConfig
            {
                Use = true,
                SizeX = sizeX,
                SizeY = sizeY,
                BlackFactor = blackFactor,
                WhiteFactor = whiteFactor,
                Angle = angle,
                MergeGroup = "Cir",
                DiffCount = diffCount,
            };
            if (dpitch >= 4)
                dpitch = (int)dpitch;
            conf.DefectPoints.Add(new SurConfig(0, 0));
            conf.SurPoints.Add(new SurConfig(0, -dpitch, CompareOption.Count, minLevelPercent));
            conf.SurPoints.Add(new SurConfig(0, dpitch, CompareOption.Count, minLevelPercent));
            conf.SurPoints.Add(new SurConfig(-dpitch, 0, CompareOption.Count, minLevelPercent));
            conf.SurPoints.Add(new SurConfig(dpitch, 0, CompareOption.Count, minLevelPercent));
            conf.SurPoints.Add(new SurConfig(-dpitch, -dpitch, CompareOption.NoCount));
            conf.SurPoints.Add(new SurConfig(-dpitch, dpitch, CompareOption.NoCount));
            conf.SurPoints.Add(new SurConfig(dpitch, -dpitch, CompareOption.NoCount));
            conf.SurPoints.Add(new SurConfig(dpitch, dpitch, CompareOption.NoCount));
            return conf;
        }

        public static MuraConfig MakeVertConfig(int cx, int cy, double blackFactor, double whiteFactor, double angle = 0,
            double dpitch = 1, double minLevelPercent = 1.0, int diffCount = 2)
        {
            var conf = new MuraConfig
            {
                Use = true,
                SizeX = cx,
                SizeY = cy,
                BlackFactor = blackFactor,
                WhiteFactor = whiteFactor,
                Angle = angle,
                MergeGroup = "Verti",
                DiffCount = diffCount,
            };
            if (dpitch >= 4)
                dpitch = (int)dpitch;
            conf.DefectPoints.Add(new SurConfig(0, 0));
            conf.SurPoints.Add(new SurConfig(-dpitch, 0, CompareOption.Count, minLevelPercent));
            conf.SurPoints.Add(new SurConfig(dpitch, 0, CompareOption.Count, minLevelPercent));
            return conf;
        }
    }
}
