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
        static void Main(string[] args)
        {
            //Affin.csにaffineパラメータを格納する関数が存在する。
            //txtに記録されたaffineパラメータを読み取り、そのパラメータを使用して、指定した前プレートでのtrackの位置(stage座標)を現プレートに変換する。

            Affine pl_to_pl = new Affine();

            string path = "C:\\Users\\GTR\\Affinepara.txt";

            System.IO.StreamReader file = new System.IO.StreamReader(path);
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

            //最後に乾板の回転等を考慮して座標を変換して終了。
        }
    }
}
