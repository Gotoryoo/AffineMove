﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using System.IO;

namespace AffineMove
{
    class Affine_GTR
    {
        public struct gridmark
        {
            public double x;
            public double y;
            //public bool xflag;
        }

        static List<gridmark> GridMarks(string txtname)
        {
            List<gridmark> gridmarks = new List<gridmark>();
            string line;

            System.IO.StreamReader file = new System.IO.StreamReader(txtname);
            while ((line = file.ReadLine()) != null)
            {
                gridmark mark;
                string line4 = line.Replace("    ", "  ");
                string line2 = line4.Replace("   ", " ");
                string line3 = line2.Replace("  ", " ");
                string[] data = line3.Split(' ');

                if (data[0] == "#")
                {
                    continue;
                }

                double x = double.Parse(data[1]);
                double y = double.Parse(data[2]);

                mark.x = x;
                mark.y = y;

                gridmarks.Add(mark);
            }
            file.Close();

            return gridmarks;
        }

        static Vector2 getTheNearest(List<gridmark> GridMarks, double x, double y)
        {
            Vector2 near = new Vector2();
            double nearx = new double();
            double neary = new double();

            double mindist = 999999.9;

            for (int i = 0; i < GridMarks.Count(); i++)
            {
                double dx = x - GridMarks[i].x;
                double dy = y - GridMarks[i].y;
                double dr = Math.Sqrt(dx * dx + dy * dy);

                if (dr < mindist)
                {
                    mindist = dr;
                    nearx = GridMarks[i].x;
                    neary = GridMarks[i].y;
                }
            }

            near.X = nearx;
            near.Y = neary;

            return near;
        }



        static void Main(string[] args)
        {
            string path = "C:\\Users\\GTR\\Documents\\";

            List<gridmark> negaGrid = GridMarks(path + "Grid_E07_20170105.gorg");

            //Affin.csにaffineパラメータを格納する関数が存在する。
            //txtに記録されたaffineパラメータを読み取り、そのパラメータを使用して、指定した前プレートでのtrackの位置(stage座標)を現プレートに変換する。

            Affine pl_to_pl = new Affine();



            System.IO.StreamReader file = new System.IO.StreamReader(path + "Affinepara.txt");
            string line;
            while ((line = file.ReadLine()) != null)
            {
                string[] data = line.Split(' ');
                double A = double.Parse(data[0]);
                double B = double.Parse(data[1]);
                double C = double.Parse(data[2]);
                double D = double.Parse(data[3]);
                double P = double.Parse(data[4]);
                double Q = double.Parse(data[5]);

                pl_to_pl.A = A;
                pl_to_pl.B = B;
                pl_to_pl.C = C;
                pl_to_pl.D = D;
                pl_to_pl.P = P;
                pl_to_pl.Q = Q;
            }
            file.Close();
            //affine変換のパラメータが格納された。

            Vector2 track = new Vector2();//track座標の仮置き ここをtxtファイル読み込みで行うのか、scan0的なファイルから読み取るのかはまだ分からないので、とりあえずこのように置く。

            Vector2 track_now = pl_to_pl.Trance(track);//これでtrack_nowには座標変換されたtrackのstageが格納される。

            //最後に乾板の回転等を考慮して座標を変換して終了。
            string path2 = "C:\\Users\\GTR\\Documents\\mag_theta.txt";

            double mag;
            double theta;

            System.IO.StreamReader file2 = new System.IO.StreamReader(path2);
            string line2;
            while ((line2 = file2.ReadLine()) != null)
            {
                string[] data = line2.Split(' ');
                mag = double.Parse(data[0]);
                theta = double.Parse(data[1]);
            }
            file2.Close();
            //今後は乾板の傾き等も考慮する必要がある？と思うので、一応乾板の傾きを読み取る。
            //必要になるのならばこの傾きで、座標変換した座標を計算し直す。


        }
    }
}
