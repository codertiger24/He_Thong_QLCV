﻿
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QLCVan
{
    public partial class LoaiCV : System.Web.UI.Page
    {
        InfoDataContext db;
        String maQuyenYeuCau = "Q006";
        protected void Page_Load(object sender, EventArgs e)
        {
            db = new InfoDataContext(
                System.Configuration.ConfigurationManager.ConnectionStrings["QuanLyCongVanConnectionString1"].ConnectionString
            );

            if (Session["TenDN"] == null)
            {
                Response.Redirect("Dangnhap.aspx");
            }
            //if (!PermissionHelper.HasPermission(maQuyenYeuCau))
            //{
            //    Response.Write("<script>alert('Bạn không có quyền truy cập trang này!'); window.history.back();</script>");
            //    Response.End();
            //}
            if (!IsPostBack)
            {
                ViewState["SearchMaLoai"] = "";
                ViewState["SearchTenLoai"] = "";
                load_LoaiCV();
            }
        }

        private void load_LoaiCV()
        {
            string maLoaiSearch = ViewState["SearchMaLoai"].ToString();
            string tenLoaiSearch = ViewState["SearchTenLoai"].ToString();

            IQueryable<tblLoaiCV> query = db.tblLoaiCVs;

            if (!string.IsNullOrEmpty(maLoaiSearch))
            {
                query = query.Where(p => p.MaLoaiCV.ToString().Contains(maLoaiSearch));
            }

            if (!string.IsNullOrEmpty(tenLoaiSearch))
            {
                query = query.Where(p => p.TenLoaiCV.Contains(tenLoaiSearch));
            }

            var data = query.OrderBy(p => p.MaLoaiCV).ToList();

            grvLoaiCV.DataSource = data;
            grvLoaiCV.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            ViewState["SearchMaLoai"] = txtSearchMaLoai.Text.Trim();
            ViewState["SearchTenLoai"] = txtSearchTenLoai.Text.Trim();
            grvLoaiCV.PageIndex = 0;
            load_LoaiCV();
        }

        protected void grvLoaiCV_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grvLoaiCV.PageIndex = e.NewPageIndex;
            load_LoaiCV();
        }

        protected void grvLoaiCV_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandArgument == null) return;

            int maLoaiCV = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "ShowEditPopup")
            {
                hfEditID.Value = maLoaiCV.ToString();
                var loaiCV = db.tblLoaiCVs.SingleOrDefault(p => p.MaLoaiCV == maLoaiCV);
                if (loaiCV != null)
                {
                    txtEditMaLoai.Text = loaiCV.MaLoaiCV.ToString();
                    txtEditTenLoaiCV.Text = loaiCV.TenLoaiCV;

                    // ĐÃ SỬA: Đọc giá trị PheDuyet từ DB và gán vào RadioButton SỬA
                    if (loaiCV.PheDuyet == "1")
                    {
                        rbEditPheDuyetCo.Checked = true;
                        rbEditPheDuyetKhong.Checked = false;
                    }
                    else
                    {
                        rbEditPheDuyetCo.Checked = false;
                        rbEditPheDuyetKhong.Checked = true;
                    }

                    mpeEdit.Show();
                }
            }
            else if (e.CommandName == "ShowDeletePopup")
            {
                hfDeleteID.Value = maLoaiCV.ToString();
                mpeDelete.Show();
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hfEditID.Value))
            {
                int maLoaiCV = Convert.ToInt32(hfEditID.Value);
                var loaiCV = db.tblLoaiCVs.SingleOrDefault(p => p.MaLoaiCV == maLoaiCV);
                if (loaiCV != null)
                {
                    loaiCV.TenLoaiCV = txtEditTenLoaiCV.Text.Trim();

                    // ĐÃ SỬA: CẬP NHẬT GIÁ TRỊ PHÊ DUYỆT KHI BẤM NÚT SỬA
                    loaiCV.PheDuyet = rbEditPheDuyetCo.Checked ? "1" : "0";

                    db.SubmitChanges();
                    load_LoaiCV();
                    mpeEdit.Hide();
                    hfEditID.Value = "";
                }
            }
        }

        protected void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hfDeleteID.Value))
            {
                int maLoaiCV = Convert.ToInt32(hfDeleteID.Value);
                var loaiCV = db.tblLoaiCVs.SingleOrDefault(p => p.MaLoaiCV == maLoaiCV);
                if (loaiCV != null)
                {
                    db.tblLoaiCVs.DeleteOnSubmit(loaiCV);
                    db.SubmitChanges();
                    load_LoaiCV();
                    mpeDelete.Hide();
                    hfDeleteID.Value = "";
                }
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string maStr = txtMaLoaiCV.Text.Trim();
            string ten = txtTenLoaiCV.Text.Trim();

            if (!string.IsNullOrWhiteSpace(maStr) && !string.IsNullOrWhiteSpace(ten))
            {
                int maLoai;
                if (int.TryParse(maStr, out maLoai))
                {
                    try
                    {
                        var checkTrung = db.tblLoaiCVs.Any(p => p.MaLoaiCV == maLoai);
                        if (!checkTrung)
                        {
                            // Logic Thêm mới: Lấy giá trị từ RadioButton. "1" = Có, "0" = Không
                            string pheDuyetValue = rbPheDuyetCo.Checked ? "1" : "0";

                            tblLoaiCV pr = new tblLoaiCV
                            {
                                MaLoaiCV = maLoai,
                                TenLoaiCV = ten,
                                PheDuyet = pheDuyetValue // GÁN GIÁ TRỊ PHÊ DUYỆT
                            };

                            db.tblLoaiCVs.InsertOnSubmit(pr);
                            db.SubmitChanges();
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ok", "alert('Đã thêm thành công'); closeAddModal();", true);
                            txtMaLoaiCV.Text = "";
                            txtTenLoaiCV.Text = "";
                            load_LoaiCV();
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "trung", "alert('Mã loại công văn đã tồn tại!');", true);
                            mpeAdd.Show();
                        }
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "err", $"alert('Lỗi khi thêm: {ex.Message}');", true);
                        mpeAdd.Show();
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "loi", "alert('Mã loại phải là số!');", true);
                    mpeAdd.Show();
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "rong", "alert('Vui lòng nhập đầy đủ');", true);
                mpeAdd.Show();
            }
        }
    }
}