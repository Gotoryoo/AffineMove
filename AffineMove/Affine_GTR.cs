using System;
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

        static List<Vector2> Position(string txt)
        {
            List<Vector2> posi = new List<Vector2>();

            System.IO.StreamReader file = new System.IO.StreamReader(txt);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                string[] data = line.Split(' ');
                double x = double.Parse(data[0]);
                double y = double.Parse(data[1]);

                posi.Add(new Vector2(x, y));

            }
            file.Close();

            return posi;
        }

        static void Main(string[] args)
        {
            string path = "C:\\Users\\GTR\\Documents\\";

            Vector2 track = new Vector2();//track座標の仮置き ここをtxtファイル読み込みで行うのか、scan0的なファイルから読み取るのかはまだ分からないので、とりあえずこのように置く。
            //Vector2 track2 = new Vector2();

            System.IO.StreamReader file2 = new System.IO.StreamReader(path + "track.txt");
            string line2;
            while ((line2 = file2.ReadLine()) != null)
            {
                string[] data = line2.Split(' ');
                double x = double.Parse(data[0]);
                double y = double.Parse(data[1]);
                track = new Vector2(x, y);
            }
            file2.Close();

            //track2 = track;

            List<gridmark> negaGrid = GridMarks(path + "Grid_E07_20170105.gorg");//gridのネガ座標

            List<Vector2> pl2Grid_Stage = Position(path + "pl2_gridstage.txt");

            List<Vector2> negaGrid_select_pl2 = new List<Vector2>();
            for(int i = 0; i< pl2Grid_Stage.Count(); i++)
            {
                Vector2 nearGrid = getTheNearest(negaGrid, pl2Grid_Stage[i].X, pl2Grid_Stage[i].Y);
                negaGrid_select_pl2.Add(nearGrid);
            }

            Affine pl2_Stage_to_Grid = Affine.CreateAffineBy(pl2Grid_Stage, negaGrid_select_pl2);//pl2のstage_to_Gridのaffineパラメータになる。

            Vector2 track_grid_pl2 = pl2_Stage_to_Grid.Trance(track);//trackをpl2のstage座標からネガ座標に変換した。

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

            Vector2 track_grid_pl3 = pl_to_pl.Trance(track_grid_pl2);//これでtrack_nowには座標変換されたtrackのstageが格納される。
            //Vector2 track_grid_pl3 = pl_to_pl.Trance(track2);//これでtrack_nowには座標変換されたtrackのstageが格納される。


            List<Vector2> pl3Grid_Stage = Position(path + "pl3_gridstage.txt");

            List<Vector2> negaGrid_select_pl3 = new List<Vector2>();
            for (int i = 0; i < pl3Grid_Stage.Count(); i++)
            {
                Vector2 nearGrid = getTheNearest(negaGrid, pl3Grid_Stage[i].X, pl3Grid_Stage[i].Y);
                negaGrid_select_pl3.Add(nearGrid);
            }

            Affine pl3_Grid_to_Stage = Affine.CreateAffineBy(negaGrid_select_pl3, pl3Grid_Stage);//pl3のGrid_to_stageのaffineパラメータになる。

            Vector2 track_stage_pl3 = pl3_Grid_to_Stage.Trance(track_grid_pl3);//trackをpl3のネガ座標からstage座標に変換した。

            string txtfileName_f_up_sw = path + string.Format("pl3_track_stage.txt");
            StreamWriter twriter_f_up_sw = File.CreateText(txtfileName_f_up_sw);
            for (int i = 0; i < 1; i++)
            {
                twriter_f_up_sw.WriteLine("{0} {1}", track_stage_pl3.X, track_stage_pl3.Y);
            }
            twriter_f_up_sw.Close();


            int aaaaa = 100;
            
        }
    }
}
