using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QLCVan
{
    public partial class NhapNDCV : System.Web.UI.Page
    {
        InfoDataContext db = new InfoDataContext();
        protected void Page_Load(object sender, EventArgs e)
        {

            if ((Session["TenDN"] == null))
            {
                Response.Redirect("Dangnhap.aspx");
            }

            /// check quyền bằng mã nhá , nếu user ko có mã quyền thì ko vào dc
       /*     if (!PermissionHelper.HasPermission("Q002"))
            {
                Response.Write("<script>alert('Bạn không có quyền truy cập trang này!'); window.history.back();</script>");
                Response.End();
            }
*/
            if (!Page.IsPostBack)
            {
                LoadLoaiCV();
                LoadDonViNhan();
                pnlNguoiDuyet.Visible = false;

                // BỎ HOÀN TOÀN ĐOẠN NÀY (VÌ SAI):
                //var cboLoaiCongvans = from d in db.tblLoaiCVs
                //                      where d.MaLoaiCV.ToString() != null
                //                      select d.TenLoaiCV;
                //ddlLoaiCV.DataSource = cboLoaiCongvans;
                //ddlLoaiCV.DataBind();

                // ✅ Dùng đoạn đúng 1 lần duy nhất:
                ddlLoaiCV.DataSource = db.tblLoaiCVs.ToList();
                ddlLoaiCV.DataTextField = "TenLoaiCV";
                ddlLoaiCV.DataValueField = "MaLoaiCV";
                ddlLoaiCV.DataBind();
                ddlLoaiCV.Items.Insert(0, new ListItem("-- Chọn loại công văn --", ""));

                // ✅ Phần GridView giữ nguyên
                var gv1 = from g in db.tblNoiDungCVs
                          join h in db.tblLoaiCVs on g.MaLoaiCV equals h.MaLoaiCV
                          orderby g.NgayGui descending
                          select new
                          {
                              g.MaCV,
                              g.SoCV,
                              h.TenLoaiCV,
                              g.NgayGui,
                              g.TieuDeCV,
                              g.CoQuanBanHanh,
                              g.GhiChu,
                              g.NgayBanHanh,
                              g.NguoiKy,
                              g.NoiNhan,
                              TrichYeuND = g.TrichYeuND.Substring(0, 250) + " ... ",
                              g.TrangThai
                          };

                gvnhapcnden.DataSource = gv1;
                gvnhapcnden.DataBind();

                // Placeholder cho date
                txtngaybanhanh.Attributes["placeholder"] = "dd/mm/yyyy";
                txtngaygui.Attributes["placeholder"] = "dd/mm/yyyy";



                if (Request.QueryString["macv"] != null)
                {
                    tblNoiDungCV cv1 = db.tblNoiDungCVs.SingleOrDefault(t => t.MaCV.ToString() == (Request.QueryString["macv"].ToString()));

                    if (cv1 != null)
                    {
                        txttieude.Text = cv1.TieuDeCV;
                        // Sửa tên biến để khớp với giao diện mới
                        txtngaybanhanh.Text = cv1.NgayGui.ToString();
                        txtngaygui.Text = cv1.NgayBanHanh.ToString();
                        txtcqbh.Text = cv1.CoQuanBanHanh;
                        txtsocv.Text = cv1.SoCV;
                        txttrichyeu.Text = cv1.TrichYeuND;
                        // Nút "Thêm" đã được hiển thị, không cần ẩn đi
                        // btnthem.Visible = false;
                        // Nút "Lưu" đã được thay bằng "Quay lại"
                        // btnsua.Visible = true;
                        ListBox1.DataTextField = "TenFile";
                        ListBox1.DataSource = cv1.tblFileDinhKems;
                        ListBox1.DataBind();
                        RadioButtonList1.SelectedIndex = (int)cv1.GuiHayNhan;

                        // Cập nhật các trường mới
                        txtNguoiKy.Text = cv1.NguoiKy;
                        txtGhiChu.Text = cv1.GhiChu;
                        // Các trường khác tương tự...
                    }
                }

            }
        }
        private void LoadDonViNhan()
        {
            string connStr = ConfigurationManager.ConnectionStrings["QuanLyCongVanConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT MaDonVi, TenDonVi FROM tblDonVi ORDER BY TenDonVi";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlDonViNhan.DataSource = reader;
                    ddlDonViNhan.DataTextField = "MaDonVi";   // Cột hiển thị
                    ddlDonViNhan.DataValueField = "MaDonVi";   // Cột lưu giá trị
                    ddlDonViNhan.DataBind();
                }
            }

            ddlDonViNhan.Items.Insert(0, new ListItem("-- Chọn đơn vị nhận --", ""));
        }
        private void LoadLoaiCV()
        {
            string connStr = ConfigurationManager.ConnectionStrings["QuanLyCongVanConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT MaLoaiCV, TenLoaiCV, PheDuyet FROM tblLoaiCV ORDER BY TenLoaiCV";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlLoaiCV.DataSource = reader;
                    //ddlLoaiCV.DataTextField = "TenLoaiCV";   // Hiển thị
                    //ddlLoaiCV.DataValueField = "MaLoaiCV";   // Lưu mã loại
                    ddlLoaiCV.DataTextField = "MaLoaiCV";
                    ddlLoaiCV.DataValueField = "MaLoaiCV";
                    ddlLoaiCV.DataBind();
                }
            }

            ddlLoaiCV.Items.Insert(0, new ListItem("-- Chọn loại công văn --", ""));
        }

        protected void ddlLoaiCV_SelectedIndexChanged(object sender, EventArgs e)
        {
            string connStr = ConfigurationManager.ConnectionStrings["QuanLyCongVanConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT PheDuyet FROM tblLoaiCV WHERE MaLoaiCV = @id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = int.Parse(ddlLoaiCV.SelectedValue);
                    conn.Open();

                    object pheDuyet = cmd.ExecuteScalar();

                    if (pheDuyet != null)
                    {
                        string val = pheDuyet.ToString().Trim().ToUpper();

                        // Chấp nhận 1 / Y / CÓ
                        if (val == "1" || val == "Y" || val == "CÓ")
                        {
                            pnlNguoiDuyet.Visible = true;
                        }
                        else
                        {
                            pnlNguoiDuyet.Visible = false;
                            txtNguoiDuyet.Text = "";
                        }
                    }
                    else
                    {
                        pnlNguoiDuyet.Visible = false;
                        txtNguoiDuyet.Text = "";
                    }
                }
            }
        }


        protected void lnk_Sua_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (LinkButton)sender;
            string maCV = lnk.CommandArgument;
            Response.Redirect("~/SuaCongVan.aspx?macv=" + maCV);
        }


        protected void lnk_Xoa_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (sender) as LinkButton;
            string str = lnk.CommandArgument;

            foreach (tblFileDinhKem item in db.tblFileDinhKems.Where(f => f.MaCV == str))
            {
                db.tblFileDinhKems.DeleteOnSubmit(item);
            }
            tblNoiDungCV cv = db.tblNoiDungCVs.SingleOrDefault(t => t.MaCV == str);
            if (cv != null)
            {
                db.tblNoiDungCVs.DeleteOnSubmit(cv);
                db.SubmitChanges();
                Page_Load(sender, e);

                Response.Redirect("~/NhapNDCV.aspx");

            }
        }

        protected void btnthem_Click(object sender, EventArgs e)
        {
            string MaCongVan = Guid.NewGuid().ToString();

            tblNoiDungCV cv1 = new tblNoiDungCV();
            cv1.MaCV = MaCongVan.ToString();
            cv1.SoCV = txtsocv.Text;
            DateTime ngayguicv = DateTime.ParseExact(txtngaybanhanh.Text.ToString(), "dd/MM/yyyy", null);
            cv1.NgayGui = ngayguicv;
            cv1.TieuDeCV = txttieude.Text;
            cv1.MaLoaiCV = int.Parse(ddlLoaiCV.SelectedValue.ToString());
            cv1.CoQuanBanHanh = txtcqbh.Text;
            cv1.TrichYeuND = txttrichyeu.Text;
            cv1.NguoiKy = txtNguoiKy.Text;
            cv1.MaNguoiGui = Session["MaNguoiDung"].ToString();
            // ❌ Không gán trực tiếp vì LINQ chưa nhận trường này
            // cv1.NguoiDuyet = txtNguoiDuyet.Text.Trim();

            cv1.BaoMat = RadioButtonList1.SelectedValue == "Có" ? "1" : "0";
            cv1.GhiChu = txtGhiChu.Text;
            DateTime ngaybancv = DateTime.ParseExact(txtngaygui.Text.ToString(), "dd/MM/yyyy", null);
            cv1.NgayBanHanh = ngaybancv;
            cv1.TrangThai = "Đã gửi";

            db.tblNoiDungCVs.InsertOnSubmit(cv1);
            db.SubmitChanges();

            // ✅ Sau khi Insert xong thì UPDATE thủ công phần Người Duyệt
            //if (!string.IsNullOrEmpty(txtNguoiDuyet.Text))
            //{
            //    string connStr = ConfigurationManager.ConnectionStrings["QuanLyCongVanConnectionString"].ConnectionString;
            //    using (SqlConnection conn = new SqlConnection(connStr))
            //    {
            //        string sql = "UPDATE tblNoiDungCV SET NguoiDuyet = @NguoiDuyet WHERE MaCV = @MaCV";
            //        using (SqlCommand cmd = new SqlCommand(sql, conn))
            //        {
            //            cmd.Parameters.AddWithValue("@NguoiDuyet", txtNguoiDuyet.Text.Trim());
            //            cmd.Parameters.AddWithValue("@MaCV", cv1.MaCV);

            //            conn.Open();
            //            cmd.ExecuteNonQuery();
            //        }
            //    }
            //}

            // ✅ Các file đính kèm giữ nguyên
            if (ListBox1.Items.Count != 0)
            {
                foreach (ListItem item in ListBox1.Items)
                {
                    tblFileDinhKem fcv = new tblFileDinhKem();
                    fcv.MaCV = cv1.MaCV;
                    fcv.FileID = Guid.NewGuid().ToString();
                    fcv.Size = Convert.ToInt32(item.Value);
                    fcv.DateUpload = DateTime.Now.ToShortDateString();
                    fcv.TenFile = item.Text;
                    fcv.Url = "~/Upload/" + item.Text;

                    db.tblFileDinhKems.InsertOnSubmit(fcv);
                    db.SubmitChanges();
                }
            }

            //Thêm danh sách vào đơn vị nhận
            string maDonViNhan = ddlDonViNhan.SelectedValue; // dropdown đơn vị nhận
            if (!string.IsNullOrEmpty(maDonViNhan))
            {
                // Lấy danh sách người dùng thuộc đơn vị nhận
                var nguoiNhanList = db.tblNguoiDungs
                    .Where(x => x.MaDonVi == maDonViNhan && x.MaNguoiDung.ToString() != Session["MaNguoiDung"].ToString())
                    .Select(x => x.MaNguoiDung)
                    .ToList();
                //foreach (var maNguoiNhan in nguoiNhanList)
                //{
                //    tblGuiNhan guiNhan = new tblGuiNhan
                //    {
                //        MaCV = MaCongVan,
                //        MaNguoiDung = Session["MaNguoiDung"].ToString(),
                //        MaNguoiNhan = maNguoiNhan,
                //        TrangThaiNhan = "Chưa đọc",
                //    };
                //    db.tblGuiNhans.InsertOnSubmit(guiNhan);
                //}
                //db.SubmitChanges();
                foreach (var maNguoiNhan in nguoiNhanList)
                {
                    db.tblGuiNhans.InsertOnSubmit(new tblGuiNhan
                    {
                        MaCV = cv1.MaCV,
                        MaNguoiDung = Session["MaNguoiDung"].ToString(),
                        MaNguoiNhan = maNguoiNhan,
                        TrangThaiNhan = "Chưa đọc"
                    });
                }
                db.SubmitChanges();
            }

            Response.Redirect("NhapNDCV.aspx");
        }



        public string kttrangthai(object obj)
        {
            bool trangthai = bool.Parse(obj.ToString());
            if (trangthai)
            {
                return "Đã duyệt";
            }
            else
            {
                return "Chưa duyệt";
            }
        }

        protected void btnUp_Click(object sender, EventArgs e)
        {
            string UploadFolder = Server.MapPath("/Upload/");
            if (FileUpload1.HasFile)
                try
                {
                    string filename = FileUpload1.PostedFile.FileName;
                    string FileNameOnServer = UploadFolder + filename;
                    FileUpload1.SaveAs(FileNameOnServer);

                    ListItem item = new ListItem(filename, Convert.ToString(FileUpload1.PostedFile.ContentLength));
                    ListBox1.Items.Add(item);

                }
                catch (Exception ex)
                {
                    lblloi.Text = "Lỗi: " + ex.Message.ToString();
                }
            else
            {
                /* lblchuachonfile.Text = "Bạn chưa chọn file!";*/
            }
        }
        void RemoveFile(int index)
        {
            if (index >= ListBox1.Items.Count) return;
            string filename = ListBox1.Items[index].Value;
            System.IO.File.Delete(filename);
            ListBox1.Items.RemoveAt(index);
        }
        protected void btnRemove_Click(object sender, EventArgs e)
        {

            RemoveFile(ListBox1.SelectedIndex);
        }

        protected void btnReAll_Click(object sender, EventArgs e)
        {
            while (ListBox1.Items.Count > 0) RemoveFile(0);
        }

        protected void btnsua_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["macv"] != null)
            {
                tblNoiDungCV cv1 = db.tblNoiDungCVs.SingleOrDefault(t => t.MaCV.ToString() == (Request.QueryString["macv"].ToString()));
                if (cv1 != null)
                {
                    // ✅ XỬ LÝ NGÀY KIỂU dd/MM/yyyy AN TOÀN
                    DateTime ngayGui, ngayBanHanh;
                    if (!DateTime.TryParseExact(
                        txtngaybanhanh.Text.Trim(),
                        "dd/MM/yyyy",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None,
                        out ngayBanHanh))
                    {
                        // Nếu lỗi format ngày thì không lưu
                        return;
                    }

                    if (!DateTime.TryParseExact(
                        txtngaygui.Text.Trim(),
                        "dd/MM/yyyy",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None,
                        out ngayGui))
                    {
                        return;
                    }

                    // ✅ GÁN GIÁ TRỊ AN TOÀN
                    cv1.SoCV = txtsocv.Text;
                    cv1.NgayGui = ngayGui;
                    cv1.TieuDeCV = txttieude.Text;
                    cv1.MaLoaiCV = int.Parse(ddlLoaiCV.SelectedValue.ToString());
                    cv1.CoQuanBanHanh = txtcqbh.Text;
                    cv1.TrichYeuND = txttrichyeu.Text;
                    cv1.NgayBanHanh = ngayBanHanh;

                    if (RadioButtonList1.SelectedIndex == 0)
                    {
                        cv1.GuiHayNhan = 1;
                    }
                    else
                        cv1.GuiHayNhan = 0;

                    // ✅ XỬ LÝ FILE AN TOÀN
                    for (int i = 0; i < ListBox1.Items.Count; i++)
                    {
                        var files = db.tblFileDinhKems.Where(t => t.MaCV.ToString() == (Request.QueryString["macv"].ToString()));
                        bool check = true;
                        foreach (var item in files)
                        {
                            if (ListBox1.Items[i].Text == item.TenFile)
                            {
                                check = false;
                                break;
                            }
                        }

                        if (check &&
                            !string.IsNullOrEmpty(ListBox1.Items[i].Value) &&
                            !string.IsNullOrEmpty(ListBox1.Items[i].Text))
                        {
                            tblFileDinhKem file = new tblFileDinhKem();
                            file.FileID = Guid.NewGuid().ToString();
                            file.Url = ListBox1.Items[i].Value.ToString();
                            file.TenFile = (ListBox1.Items[i].Text);
                            file.DateUpload = DateTime.Now.ToShortDateString();
                            file.MaCV = cv1.MaCV;
                            db.tblFileDinhKems.InsertOnSubmit(file);
                        }
                    }

                    // ✅ CHẶN LỖI Ở SubmitChanges
                    try
                    {
                        db.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        Response.Write(ex.Message);
                        return;
                    }

                    Response.Redirect("NhapNDCV.aspx");
                    // btnthem.Visible = true;
                    // btnsua.Visible = false;
                }
            }
        }


        protected void btnlammoi_Click(object sender, EventArgs e)
        {
            txtcqbh.Text = "";
            txtngaygui.Text = "";
            txtngaybanhanh.Text = "";
            txtsocv.Text = "";
            txttieude.Text = "";
            txttrichyeu.Text = "";
            txtNguoiKy.Text = "";
            txtNguoiDuyet.Text = "";
            txtGhiChu.Text = "";

        }

        protected void gvnhapcnden_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            var gv1 = from g in db.tblNoiDungCVs
                      from h in db.tblLoaiCVs
                      where g.MaLoaiCV == h.MaLoaiCV
                      orderby g.NgayGui descending
                      select new
                      {
                          g.MaCV,
                          g.SoCV,
                          h.TenLoaiCV,
                          g.NgayGui,
                          g.TieuDeCV,
                          g.CoQuanBanHanh,
                          g.NgayBanHanh,
                          g.NguoiKy,
                          TrichYeuND = g.TrichYeuND.Substring(0, 200) + ". . ."
                      };
            gvnhapcnden.PageIndex = e.NewPageIndex;
            gvnhapcnden.DataSource = gv1;
            gvnhapcnden.DataBind();

        }

        protected void gvnhapcnden_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        protected void cboLoaiCongvan_ItemInserting(object sender, AjaxControlToolkit.ComboBoxItemInsertEventArgs e)
        {
            string congvanmoi = e.Item.Value;
            tblLoaiCV pr = new tblLoaiCV();
            pr.TenLoaiCV = congvanmoi;
            db.tblLoaiCVs.InsertOnSubmit(pr);
            db.SubmitChanges();
            pr = db.tblLoaiCVs.SingleOrDefault(p => p.TenLoaiCV == congvanmoi);
            e.Item.Value = pr.MaLoaiCV.ToString();
        }

        protected void cboNguoiKy_ItemInserted(object sender, AjaxControlToolkit.ComboBoxItemInsertEventArgs e)
        {
            string nguoikimoi = e.Item.Value;
            tblNoiDungCV pr = new tblNoiDungCV();
            pr.NguoiKy = nguoikimoi;
            db.tblNoiDungCVs.InsertOnSubmit(pr);
            db.SubmitChanges();
            pr = db.tblNoiDungCVs.SingleOrDefault(p => p.NguoiKy == nguoikimoi);
            e.Item.Value = pr.MaLoaiCV.ToString();
        }
    }
}