//
// File generated by HDevelop for HALCON/.NET (C#) Version 18.05.0.1
//
//  This file is intended to be used with the HDevelopTemplate or
//  HDevelopTemplateWPF projects located under %HALCONEXAMPLES%\c#

using System;
using HalconDotNet;

public partial class HDevelopExport
{
    public HTuple hv_ExpDefaultWinHandle;

    private const int countNumberTop = 20;
    private const int countNumberLeft = 45;

    private int countNumber = 0;

    // Main procedure 
    private void action(ref System.Collections.Generic.List<BusEdgPoint.Str_Ht> lst, BusEdgPoint.SideType sideType)
    {
        string strFile;

        int []length = new int[150];
        // Local iconic variables 

        HObject ho_Image, ho_Edges, ho_UnionContours, ho_RegionDilation;
        HObject ho_ObjectSelected = null;

        // Local control variables 

        HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
        HTuple hv_Row = new HTuple(), hv_Col = new HTuple(), hv_Number = new HTuple();
        HTuple hv_Index = new HTuple();
        // Initialize local and output iconic variables 
        HOperatorSet.GenEmptyObj(out ho_Image);
        HOperatorSet.GenEmptyObj(out ho_Edges);
        HOperatorSet.GenEmptyObj(out ho_UnionContours);
        HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
        HOperatorSet.GenEmptyObj(out ho_RegionDilation);
        ho_Image.Dispose();
        HOperatorSet.ReadImage(out ho_Image, "Conv2.bmp");
        ho_RegionDilation.Dispose();
        HOperatorSet.DilationCircle(ho_Image, out ho_RegionDilation, 3.5);

        HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);

        // dev_update_window(...); only in hdevelop
        //dev_close_window(...);
        //dev_open_window(...);
        //HOperatorSet.DispObj(ho_Image, hv_ExpDefaultWinHandle);
        //提取轮廓
        ho_Edges.Dispose();
        HOperatorSet.EdgesSubPix(ho_Image, out ho_Edges, "lanser2", 0.5, 40, 90);
        //length_xld (Edges, Length)
        //select_shape_xld (Edges, SelectedXLD, 'area', 'and', 0, 9999999)
        ho_UnionContours.Dispose();
        HOperatorSet.UnionAdjacentContoursXld(ho_Edges, out ho_UnionContours, 100, 1,
            "attr_keep");
        //lines_facet (Image, Lines, 5, 3, 8, 'light')
        //get_contour_xld (SelectedXLD, Row, Col)


        //edges_sub_pix (Image104305Aa5460af6802b71, Edges, 'canny', 1, 20, 40)
        HOperatorSet.CountObj(ho_Edges, out hv_Number);

        HTuple end_val20 = hv_Number;
        HTuple step_val20 = 1;
        for (hv_Index = 1; hv_Index.Continue(end_val20, step_val20); hv_Index = hv_Index.TupleAdd(step_val20))
        {
            ho_ObjectSelected.Dispose();
            HOperatorSet.SelectObj(ho_Edges, out ho_ObjectSelected, hv_Index);

            HOperatorSet.GetContourXld(ho_ObjectSelected, out hv_Row, out hv_Col);
            length[hv_Index - 1] = hv_Row.Length;

            switch(sideType)
            {
                case BusEdgPoint.SideType.LEFT:
                    countNumber = countNumberLeft;
                    break;
                case BusEdgPoint.SideType.TOP:
                    countNumber = countNumberTop;
                    break;
                default:
                    return;
            }
            if(hv_Row.Length > countNumber)
            {

                BusEdgPoint.Str_Ht ht = new BusEdgPoint.Str_Ht();
                ht.htCol = hv_Col;
                ht.htRow = hv_Row;
                lst.Add(ht);
            }
          
        }

        ho_Image.Dispose();
        ho_Edges.Dispose();
        ho_UnionContours.Dispose();
        ho_ObjectSelected.Dispose();



    }

    public void InitHalcon()
    {
        // Default settings used in HDevelop 
        HOperatorSet.SetSystem("width", 512);
        HOperatorSet.SetSystem("height", 512);
    }

    public void RunHalcon(ref System.Collections.Generic.List<BusEdgPoint.Str_Ht> lst, BusEdgPoint.SideType side)
    {
        //hv_ExpDefaultWinHandle = Window;
        action(ref lst, side);
    }

}
