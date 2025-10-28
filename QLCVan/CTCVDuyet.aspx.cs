using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;

namespace QLCVan
{
    public partial class CTCVDuyet : System.Web.UI.Page
    {
        InfoDataContext db = new InfoDataContext();
        private List<string> lstAtt = new List<string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TenDN"] == null)
            {
                Response.Redirect("Dangnhap.aspx");
                return;
            }

            string trangThaiHienThi = "";
            string maNguoiDung = Session["MaNguoiDung"].ToString();
            string maCongVan = Request.QueryString["id"];
            if (String.IsNullOrEmpty(maCongVan)) return;

            tblNoiDungCV tk = db.tblNoiDungCVs.SingleOrDefault(n => n.MaCV == maCongVan);
            tblGuiNhan gn = db.tblGuiNhans.SingleOrDefault(g => g.MaCV == maCongVan);
            if (tk != null)
            {
                txtTieuDe.Text = tk.TieuDeCV;
                txtSoCV.Text = tk.SoCV;
                txtTenloaiCV.Text = db.tblLoaiCVs
                                      .Where(lcv => lcv.MaLoaiCV == tk.MaLoaiCV)
                                      .Select(lcv => lcv.TenLoaiCV)
                                      .SingleOrDefault();
                txtCQBH.Text = tk.CoQuanBanHanh;
                //TextDonvinhan.Text = tk.NoiNhan; // nếu có
                TextNguoiky.Text = tk.NguoiKy;
                TextNguoiduyet.Text = tk.NguoiDuyet;
                txtNgayBH.Text = tk.NgayBanHanh.HasValue ? tk.NgayBanHanh.Value.ToString("dd-MM-yyyy") : "";
                txtNgaygui.Text = tk.NgayGui.HasValue ? tk.NgayGui.Value.ToString("dd-MM-yyyy") : "";
                // <--- FIX: dùng .Text thay vì .InnerText
                txtaTrichyeu.Text = tk.TrichYeuND ?? "";
                rptfilecv.DataSource = db.tblFileDinhKems.Where(t => t.MaCV == tk.MaCV);
                rptfilecv.DataBind();

                if (gn.MaNguoiNhan == maNguoiDung)
                {
                    trangThaiHienThi = gn.TrangThaiNhan;
                }
                if (tk.MaNguoiGui == maNguoiDung)
                {
                    trangThaiHienThi = tk.TrangThai;
                    BtnDuyet.Visible = false;
                    BtnKhongDuyet.Visible = false;
                    BtnChinhSua.Visible = true;
                }


                lblTrangThai.Text = trangThaiHienThi;
                //if (trangThaiHienThi == "Chờ duyệt")
                //{
                //    lblTrangThai.Text = trangThaiHienThi;
                //    lblTrangThai.Style["background-color"] = "#F9B200";
                //}
                //if (trangThaiHienThi == "Đang trình")
                //{
                //    lblTrangThai.Text = trangThaiHienThi;
                //    lblTrangThai.Style["background-color"] = "#DC3545";
                //}

            }

            // lay và xu ly file
            lstAtt.Clear();
            var v = db.tblFileDinhKems.Where(f => f.MaCV.Equals(maCongVan)).Select(n => n.TenFile).ToList();
            if (v != null && v.Count > 0)
            {
                foreach (var file in v)
                {
                    lstAtt.Add(file);
                }
            }

            // FIX: .Text thay vì .InnerText
            Session["macv"] = txtTieuDe.Text + "|" + txtaTrichyeu.Text + "|";
        }

        protected void btnQuayLai(object sender, EventArgs e)
        {
            //    string maCongVan = Request.QueryString["id"];
            //    if (string.IsNullOrEmpty(maCongVan)) return;

            //    tblNoiDungCV cv1 = db.tblNoiDungCVs.SingleOrDefault(t => t.MaCV == maCongVan);
            //    if (cv1 != null)
            //    {
            //        cv1.TrangThai = "Đã duyệt";
            //        db.SubmitChanges();
            //    }
            Response.Redirect($"Trangchu.aspx");
        }

        protected void btnKhongDuyet(object sender, EventArgs e)
        {
            //    string maCongVan = Request.QueryString["id"];
            //    if (string.IsNullOrEmpty(maCongVan)) return;

            //    tblNoiDungCV cv1 = db.tblNoiDungCVs.SingleOrDefault(t => t.MaCV == maCongVan);
            //    if (cv1 != null)
            //    {
            //        cv1.TrangThai = "Đã duyệt";
            //        db.SubmitChanges();
            //    }
            //Response.Redirect($"Trangchu.aspx");
        }

        protected void btnDuyet(object sender, EventArgs e)
        {
            //    string maCongVan = Request.QueryString["id"];
            //    if (string.IsNullOrEmpty(maCongVan)) return;

            //    tblNoiDungCV cv1 = db.tblNoiDungCVs.SingleOrDefault(t => t.MaCV == maCongVan);
            //    if (cv1 != null)
            //    {
            //        cv1.TrangThai = "Đã duyệt";
            //        db.SubmitChanges();
            //    }
            //Response.Redirect($"Trangchu.aspx");
        }

        protected void btnChinhSua(object sender, EventArgs e)
        {
            //    string maCongVan = Request.QueryString["id"];
            //    if (string.IsNullOrEmpty(maCongVan)) return;

            //    tblNoiDungCV cv1 = db.tblNoiDungCVs.SingleOrDefault(t => t.MaCV == maCongVan);
            //    if (cv1 != null)
            //    {
            //        cv1.TrangThai = "Đã duyệt";
            //        db.SubmitChanges();
            //    }
            //Response.Redirect($"Trangchu.aspx");
        }
    }
}

