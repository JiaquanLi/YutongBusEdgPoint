using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using HalconDotNet;
using WindowsFormsApp2;

namespace WindowsFormsApp2
{

    public struct XYZ
    {
        public int x;
        public double y;
        public double z;
    }

    public struct Point2Point
    {
       public  Point pointStart;
       public Point pointEnd;
    }

    public  struct Str_Ht
    {
        public HTuple htRow;
        public HTuple htCol;

    }

    public enum SideType
    {
        LEFT,
        TOP
    }
  
    public partial class Form1 : Form
    {
        public const int offsetImgageXLft = 500;
        public const int offsetImgageYLft = 500;

        public const int offsetImgageXTop = 500;
        public const int offsetImgageYTop = -200;

        public  int offsetImgageX ;
        public  int offsetImgageY ;

        public static List<Point2Point> obj_MotorP2P = new List<Point2Point>();

        public static List<Point> obj_MotorP = new List<Point>();

        public static List<XYZ> obj_xyz = new List<XYZ>();

        public static SideType mSizdType;

        private  List<XYZ> mSendMotorPoint;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string file = "";
            if (comboBox1.Text.ToUpper() == "TOP")
            {
                mSizdType = SideType.TOP;
                file = "车顶.txt";

                offsetImgageX = offsetImgageXTop;
                offsetImgageY = offsetImgageYTop;
            }
            else if(comboBox1.Text.ToUpper() == "LEFT")
            {
                mSizdType = SideType.LEFT;
                file = "侧边.txt";

                offsetImgageX = offsetImgageXLft;
                offsetImgageY =  offsetImgageYLft;
            }
            else
            {
                MessageBox.Show("选择面");
                return;
            }


            int countAdd = 0;  //Y方向的间隔 补齐参数 老的是10
            if(System.IO.File.Exists(file) == false)
            {
                MessageBox.Show("File :" + file + " not exist");
                return;
            }
            FileStream fs = new FileStream(file, FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            obj_MotorP.Clear();
            obj_MotorP2P.Clear();

            string str;
            XYZ tempXyz;
            long count = 0;
            obj_xyz.Clear();

            int[][] buffer;
            while ((str = sr.ReadLine()) != null)
            {
                string[] temp = str.Split(' ');

                if (temp.Length != 3) continue;
                tempXyz.x = (int)double.Parse(temp[0]) + offsetImgageX;
                tempXyz.y = double.Parse(temp[1]) - offsetImgageY;
                tempXyz.z = double.Parse(temp[2]);
                if (obj_xyz.Count == 0)
                {
                    obj_xyz.Add(tempXyz);
                }
                else
                {
                    if (tempXyz.x != obj_xyz[obj_xyz.Count - 1].x)
                        obj_xyz.Add(tempXyz);
                }

            }

            Bitmap bt = new Bitmap(1080, 3000);
            for (int w = 0; w < bt.Width; w++)
            {
                for(int h = 0; h < bt.Height; h++)
                {
                    bt.SetPixel(w, h, System.Drawing.Color.FromArgb(0, 0, 0));
                }
            }
            for(int i = 0; i < obj_xyz.Count; i++)
            {
                //bt.SetPixel(obj_xyz[i].x, (int)obj_xyz[i].y, System.Drawing.Color.FromArgb(255, 255, 255));
                if (i > 1)
                {
                    int offset = obj_xyz[i].x - obj_xyz[i - 1].x;//补齐 x 方向的缺点
                    if (offset > 1 && offset < 10 && obj_xyz[i].y == obj_xyz[i - 1].y)
                    {
                        for (int index = 0; index < offset; index++)
                        {
                            bt.SetPixel(obj_xyz[i].x - index, (int)obj_xyz[i].y, System.Drawing.Color.FromArgb(255, 255, 255));
                            for (int indexAdd = 0; indexAdd < countAdd; indexAdd++)
                            {
                                bt.SetPixel(obj_xyz[i].x - index, (int)obj_xyz[i].y + indexAdd, System.Drawing.Color.FromArgb(255, 255, 255));
                            }
                        }

                    }
                    else
                    {
                        bt.SetPixel(obj_xyz[i].x, (int)obj_xyz[i].y, System.Drawing.Color.FromArgb(255, 255, 255));
                        for (int indexAdd = 0; indexAdd < countAdd; indexAdd++) //补齐Y方向
                        {
                            bt.SetPixel(obj_xyz[i].x, (int)obj_xyz[i].y + indexAdd, System.Drawing.Color.FromArgb(255, 255, 255));
                        }
                    }
                }
                else
                {
                    bt.SetPixel(obj_xyz[i].x, (int)obj_xyz[i].y, System.Drawing.Color.FromArgb(255, 255, 255));
                    for (int index = 0; index < 10; index++)// 补齐y 方向的间隔
                    {
                        bt.SetPixel(obj_xyz[i].x, (int)obj_xyz[i].y + index, System.Drawing.Color.FromArgb(255, 255, 255));
                    }
                }

            }
            sr.Close();
            fs.Close();
            bt.Save("Conv2.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            if (pictureBox1.Image != null) pictureBox1.Image.Dispose();
            pictureBox1.Image = bt;


            List<Str_Ht> lsHT = new List<Str_Ht>();
            HDevelopExport hd = new HDevelopExport();
            hd.RunHalcon( ref lsHT,mSizdType);

            GetNewImage(lsHT);

        }

        void GetNewImage(List<Str_Ht> lsHT)
        {

            FileStream fs = new FileStream("save.txt", FileMode.OpenOrCreate);
           
            StreamWriter sr = new StreamWriter(fs);
            Bitmap btGet = new Bitmap(1080, 3000);
            for (int w = 0; w < btGet.Width; w++)
            {
                for (int h = 0; h < btGet.Height; h++)
                {
                    btGet.SetPixel(w, h, System.Drawing.Color.FromArgb(0, 0, 0));
                }
            }

            for(int i = 0; i < lsHT.Count; i++)
            {
                for(int x = 0; x < lsHT[i].htCol.Length; x++)
                {

                        double sxx = lsHT[i].htCol[x];
                        double yxx = lsHT[i].htRow[x];
                        btGet.SetPixel((int)sxx,(int)yxx, System.Drawing.Color.FromArgb(255, 255, 255));

                        string strtemp = ((int)sxx).ToString() + " " + yxx.ToString();
                        sr.WriteLine(strtemp);

                }
            }

            sr.Close();
            fs.Close();
            btGet.Save("edg.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            if (pictureBox2.Image != null) pictureBox2.Image.Dispose();
            pictureBox2.Image = btGet;

            GetPoints(btGet, mSizdType);


        }

        private void GetPoints(Bitmap bt,SideType sizdType)
        {
            int iStep = 30;
            List<Point> lstPoint = new List<Point>();
            int count = 0;
            for(int heightY = 0; heightY < bt.Height; heightY += iStep )
            {
            NEXT1:
                lstPoint.Clear();

                if (sizdType == SideType.TOP)
                {
                    for (int widthX = 0; widthX < bt.Width; widthX++)
                    {
                        Color cl = bt.GetPixel(widthX, heightY);
                        if (cl.R > 0)
                        {
                            lstPoint.Add(new Point(widthX, heightY));
                        }

                    }
                    if (lstPoint.Count == 2 && lstPoint[1].X - lstPoint[0].X > 140)
                    {
                        //获取2个点的 起始和结尾的 坐标
                        Point pStart, pEnd;
                        Point pTempStart, pTempEnd;

                        pTempStart = new Point();
                        pTempEnd = new Point();

                        pStart = lstPoint[0];
                        pEnd = lstPoint[1];
                        //2个点的 再寻找8个点 补充车顶弧度坐标
                        double addCuntPointTo2 = 8.0;

                        // 计算8个点的间隔像素  step
                        double tst = (double)(lstPoint[1].X - lstPoint[0].X) / (addCuntPointTo2 - 1);
                        int xStepForEightPoint = (int)tst;//(lstPoint[1].X - lstPoint[0].X) / (int)addCuntPointTo2;//算出8个点间隔的像素点

                        for(int iXCount = 0; iXCount < 4; iXCount++)
                        {
                            Point2Point p2p;

                            pTempStart.X = pStart.X + xStepForEightPoint * 2*iXCount;
                            pTempStart.Y = pStart.Y;


                            pTempEnd.X = pTempStart.X + xStepForEightPoint;
                            pTempEnd.Y = pStart.Y;

                            p2p.pointStart = pTempStart;
                            if (iXCount == 3) //将结尾的 点用 pEnd 矫正
                            {
                                pTempEnd.X = pEnd.X;
                            }

                            p2p.pointEnd = pTempEnd;
                            obj_MotorP2P.Add(p2p);

                            //pTempStart.X = pTempEnd.X + xStepForEightPoint;// //
                        }

                       

                    }
                    else if (lstPoint.Count == 4 && (lstPoint[1].X - lstPoint[0].X > 30 && lstPoint[1].X - lstPoint[0].X < 90)
                        && (lstPoint[3].X - lstPoint[2].X > 30 && lstPoint[3].X - lstPoint[2].X < 90))
                    {
                        Point2Point p2p1, p2p2;
                        p2p1.pointStart = lstPoint[0];
                        p2p1.pointEnd = lstPoint[1];
                        obj_MotorP2P.Add(p2p1);

                        p2p2.pointStart = lstPoint[2];
                        p2p2.pointEnd = lstPoint[3];
                        obj_MotorP2P.Add(p2p2);
                    }
                    else if (lstPoint.Count == 8)
                    {
                        Point2Point p2p1, p2p2, p2p3, p2p4, p2p5, p2p6, p2p7, p2p8;
                        p2p1.pointStart = lstPoint[0];
                        p2p1.pointEnd = lstPoint[1];
                        obj_MotorP2P.Add(p2p1);

                        p2p2.pointStart = lstPoint[2];
                        p2p2.pointEnd = lstPoint[3];
                        obj_MotorP2P.Add(p2p2);

                        p2p3.pointStart = lstPoint[4];
                        p2p3.pointEnd = lstPoint[5];
                        obj_MotorP2P.Add(p2p3);

                        p2p4.pointStart = lstPoint[6];
                        p2p4.pointEnd = lstPoint[7];
                        obj_MotorP2P.Add(p2p4);
                    }
                    else
                    {
                        heightY += 1;
                        if (heightY < bt.Height)
                            goto NEXT1;
                    }
                }
                else if(sizdType == SideType.LEFT)
                {
                    for (int widthX = 0; widthX < bt.Width; widthX++)
                    {
                        Color cl = bt.GetPixel(widthX, heightY);
                        if (cl.R > 0)
                        {
                            lstPoint.Add(new Point(widthX, heightY));
                        }

                    }

                    if (lstPoint.Count < 20 && lstPoint.Count >2)
                    {
                        for(int i = 0; i < lstPoint.Count; i ++)
                        {
                            obj_MotorP.Add(lstPoint[i]);
                        }
                    }
                    else
                    {
                        heightY += 1;
                        if (heightY < bt.Height)
                            goto NEXT1;
                    }
                }


            }

            Bitmap btPointMotor = new Bitmap(1080, 3000);
            for (int w = 0; w < btPointMotor.Width; w++)
            {
                for (int h = 0; h < btPointMotor.Height; h++)
                {
                    btPointMotor.SetPixel(w, h, System.Drawing.Color.FromArgb(0, 0, 0));
                }
            }


            string strWriteS = "";
            string strWriteE = "";
            FileStream fs = new FileStream("motorPoint.txt", FileMode.OpenOrCreate);
            StreamWriter sr = new StreamWriter(fs);

            if(sizdType == SideType.LEFT)
            {
                for (int i = 0; i < obj_MotorP.Count; i++)
                {
                    for (int xLen = -3; xLen < 3; xLen++)
                    {
                        for (int yLen = -3; yLen < 3; yLen++)
                        {
                            btPointMotor.SetPixel(obj_MotorP[i].X + xLen, obj_MotorP[i].Y + yLen, System.Drawing.Color.FromArgb(255, 255, 255));

                            for (int iCnt = 0; iCnt < obj_xyz.Count; iCnt++)
                            {
                                if (obj_xyz[iCnt].x == (obj_MotorP[i].X + xLen) && (int)(obj_xyz[iCnt].y) == obj_MotorP[i].Y + yLen)
                                {
                                    strWriteS = String.Format("{0} {1} {2}", obj_xyz[iCnt].x - offsetImgageX, (int)obj_xyz[iCnt].y + offsetImgageY, obj_xyz[iCnt].z);

                                }
                            }
                        }


                    }

                    if (strWriteS == "" )
                    {
                        MessageBox.Show("Miss one");
                        continue;
                    }
                    sr.WriteLine(strWriteS);
                }

            }
            else
            {

                for (int i = 0; i < obj_MotorP2P.Count; i++)
                {
                    for (int xLen = -3; xLen < 3; xLen++)
                    {
                        for (int yLen = -3; yLen < 3; yLen++)
                        {
                            btPointMotor.SetPixel(obj_MotorP2P[i].pointStart.X + xLen, obj_MotorP2P[i].pointStart.Y + yLen, System.Drawing.Color.FromArgb(255, 255, 255));
                            btPointMotor.SetPixel(obj_MotorP2P[i].pointEnd.X + xLen, obj_MotorP2P[i].pointEnd.Y + yLen, System.Drawing.Color.FromArgb(255, 255, 255));


                            for (int iCnt = 0; iCnt < obj_xyz.Count; iCnt++)
                            {
                                if (obj_xyz[iCnt].x == (obj_MotorP2P[i].pointStart.X + xLen) && (int)(obj_xyz[iCnt].y) == obj_MotorP2P[i].pointStart.Y + yLen)
                                {
                                    strWriteS = String.Format("{0} {1} {2}", obj_xyz[iCnt].x - offsetImgageX, (int)obj_xyz[iCnt].y + offsetImgageY, obj_xyz[iCnt].z);

                                    //sr.WriteLine(strWrite);
                                }
                            }

                            for (int iCnt = 0; iCnt < obj_xyz.Count; iCnt++)
                            {
                                if (obj_xyz[iCnt].x == (obj_MotorP2P[i].pointEnd.X + xLen) && (int)(obj_xyz[iCnt].y) == obj_MotorP2P[i].pointEnd.Y + yLen)
                                {
                                    strWriteE = String.Format("{0} {1} {2}", obj_xyz[iCnt].x - offsetImgageX, (int)obj_xyz[iCnt].y + offsetImgageY, obj_xyz[iCnt].z);

                                    //sr.WriteLine(strWrite);
                                }
                            }


                        }
                    }

                    if (strWriteS == "" || strWriteE == "")
                    {
                        MessageBox.Show("Miss one");
                    }
                    sr.WriteLine(strWriteS);
                    sr.WriteLine(strWriteE);

                }
            }
            
            sr.Close();
            fs.Close();
            btPointMotor.Save("motor.BMP", System.Drawing.Imaging.ImageFormat.Bmp);

            if (pictureBox3.Image != null) pictureBox3.Image.Dispose();
            pictureBox3.Image = btPointMotor;

        }
    }
}
