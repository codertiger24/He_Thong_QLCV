using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QLCVan
{
    public partial class SuaCongVan : Page
    {
        private readonly InfoDataContext db = new InfoDataContext();

        #region Lifecycle

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TenDN"] == null)
            {
                Response.Redirect("Gioithieu.aspx");
                return;
            }
            /*  if (Session["QuyenHan"] != null && Session["QuyenHan"].ToString().Trim() == "User")
            {
                ClientScript.RegisterStartupScript(GetType(), "noauth",
                    "alert('Bạn không có quyền truy cập trang này !'); location.href='Trangchu.aspx';", true);
                return;
            }*/
            if (!IsPostBack)
            {
                string key = GetCvKey();
                if (string.IsNullOrEmpty(key))
                {
                    Response.Redirect("Trangchu.aspx");
                    return;
                }

                LoadCongVan(key);
                LoadFileDinhKem(key);
                BindDonViNhanReadonly(key); // dùng đúng biến key
            }
        }
        

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (ViewState["__NOI_NHAN_LOCK__"] != null)
                lblDonViNhan.Text = ViewState["__NOI_NHAN_LOCK__"].ToString();
        }

        #endregion

        #region UI Events

        protected void btnCapNhat_Click(object sender, EventArgs e)
        {
            lblloi.Text = string.Empty;

            string key = GetCvKey();
            if (string.IsNullOrEmpty(key)) return;

            var cv = db.tblNoiDungCVs.SingleOrDefault(x => x.MaCV == key);
            if (cv == null) return;

            // Cập nhật các trường cho phép sửa (KHÔNG đụng cột Đơn vị nhận)
            cv.TieuDeCV = SafeText(txttieude.Text);
            cv.SoCV = SafeText(txtsocv.Text);
            cv.CoQuanBanHanh = SafeText(txtcqbh.Text);
            cv.TrichYeuND = SafeText(txttrichyeu.Text);
            cv.GhiChu = SafeText(txtGhiChu.Text);
            cv.NguoiKy = SafeText(txtNguoiKy.Text);

            DateTime d;
            cv.NgayBanHanh = TryParseDdMMyyyy(SafeText(txtngaybanhanh.Text), out d) ? d : (DateTime?)null;
            cv.NgayGui = TryParseDdMMyyyy(SafeText(txtngaygui.Text), out d) ? d : (DateTime?)null;

            if (RadioButtonList1?.SelectedItem != null)
                cv.GuiHayNhan = (RadioButtonList1.SelectedValue == "Có") ? 1 : 0;

            // Cập nhật lại đơn vị nhận
            string selectedDonViNhan = lblDonViNhan.Text; // Giả sử bạn đã lấy danh sách đơn vị nhận ở trên
            if (!string.IsNullOrEmpty(selectedDonViNhan))
            {
                // Lưu thông tin đơn vị nhận vào cơ sở dữ liệu
                // (Bạn có thể thêm logic xử lý nếu muốn cập nhật lại danh sách đơn vị nhận vào bảng liên kết)
            }

            try
            {
                db.SubmitChanges();
                LoadCongVan(key);
                Response.Redirect("Trangchu.aspx");
            }
            catch (Exception ex)
            {
                lblloi.Text = "Lỗi lưu: " + ex.Message;
            }
        }


        protected void btnQuayLai_Click(object sender, EventArgs e)
        {
            Response.Redirect("Trangchu.aspx");
        }

        protected void btnUp_Click(object sender, EventArgs e)
        {
            lblloi.Text = string.Empty;
            string key = GetCvKey();
            if (string.IsNullOrEmpty(key)) return;

            if (!FileUpload1.HasFile)
            {
                lblloi.Text = "Vui lòng chọn tệp.";
                return;
            }

            try
            {
                string uploadFolder = Server.MapPath("~/Upload/");
                if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);

                string originalName = Path.GetFileName(FileUpload1.FileName);
                string serverFile = Path.Combine(uploadFolder, originalName);
                serverFile = EnsureUniquePath(serverFile);

                FileUpload1.SaveAs(serverFile);

                var file = new tblFileDinhKem
                {
                    FileID = Guid.NewGuid().ToString(),
                    TenFile = Path.GetFileName(serverFile),
                    Url = "~/Upload/" + Path.GetFileName(serverFile),
                    DateUpload = DateTime.Now.ToString("dd/MM/yyyy"),
                    MaCV = key
                };

                db.tblFileDinhKems.InsertOnSubmit(file);
                db.SubmitChanges();

                LoadFileDinhKem(key);
            }
            catch (Exception ex)
            {
                lblloi.Text = "Lỗi upload: " + ex.Message;
            }
        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            lblloi.Text = string.Empty;
            string key = GetCvKey();
            if (string.IsNullOrEmpty(key)) return;
            if (ListBox1?.SelectedItem == null) return;

            string fileName = ListBox1.SelectedItem.Text;

            try
            {
                var record = db.tblFileDinhKems.FirstOrDefault(x => x.MaCV == key && x.TenFile == fileName);
                if (record != null)
                {
                    db.tblFileDinhKems.DeleteOnSubmit(record);
                    db.SubmitChanges();
                }

                string physical = Server.MapPath("~/Upload/" + fileName);
                if (File.Exists(physical)) File.Delete(physical);

                LoadFileDinhKem(key);
            }
            catch (Exception ex)
            {
                lblloi.Text = "Lỗi xóa tệp: " + ex.Message;
            }
        }

        #endregion

        #region Data Loading

        private void LoadCongVan(string key)
        {
            var cv = db.tblNoiDungCVs.SingleOrDefault(x => x.MaCV == key);
            if (cv == null)
            {
                Response.Redirect("Trangchu.aspx");
                return;
            }

            // Đặt dữ liệu vào các textbox
            txttieude.Text = NullSafe(cv.TieuDeCV);
            txtsocv.Text = NullSafe(cv.SoCV);
            txtcqbh.Text = NullSafe(cv.CoQuanBanHanh);
            txttrichyeu.Text = NullSafe(cv.TrichYeuND);
            txtGhiChu.Text = NullSafe(cv.GhiChu);
            txtNguoiKy.Text = NullSafe(cv.NguoiKy);

            txtngaybanhanh.Text = cv.NgayBanHanh.HasValue ? cv.NgayBanHanh.Value.ToString("dd/MM/yyyy") : string.Empty;
            txtngaygui.Text = cv.NgayGui.HasValue ? cv.NgayGui.Value.ToString("dd/MM/yyyy") : string.Empty;

            // Lấy dữ liệu Loại công văn
            lblLoaiCVValue.Text = ResolveLoaiCongVan(cv);
            if (string.IsNullOrWhiteSpace(lblLoaiCVValue.Text)) lblLoaiCVValue.Text = "(Chưa có)";

            // Lấy thông tin người duyệt
            lblNguoiDuyetValue.Text = ResolveNguoiDuyet(cv);
            if (string.IsNullOrWhiteSpace(lblNguoiDuyetValue.Text)) lblNguoiDuyetValue.Text = "(Chưa có)";

            // Gọi hàm lấy đơn vị nhận
            BindDonViNhanReadonly(key);
        }


        private void LoadFileDinhKem(string key)
        {
            var files = db.tblFileDinhKems
                          .Where(x => x.MaCV == key)
                          .OrderByDescending(x => x.DateUpload)
                          .Select(x => x.TenFile)
                          .ToList();

            ListBox1.Items.Clear();
            foreach (var name in files)
                ListBox1.Items.Add(new ListItem(name, name));
        }

        #endregion

        #region Resolvers

        private string ResolveLoaiCongVan(tblNoiDungCV cv)
        {
            try
            {
                if (cv.MaLoaiCV == null) return string.Empty;
                int ma = Convert.ToInt32(cv.MaLoaiCV);
                var loai = db.tblLoaiCVs.SingleOrDefault(l => l.MaLoaiCV == ma);
                return loai != null ? NullSafe(loai.TenLoaiCV) : string.Empty;
            }
            catch { return string.Empty; }
        }

        private string ResolveNguoiDuyet(tblNoiDungCV cv)
        {
            string s = NullSafe(cv.NguoiDuyet);
            if (!string.IsNullOrWhiteSpace(s)) return s;
            return string.Empty;
        }

        #endregion

        #region 

        private void BindDonViNhanReadonly(string maCv)
        {
            lblDonViNhan.Text = string.Empty;  // Đảm bảo trước khi gán mới, xóa nội dung cũ

            string connStr = System.Configuration.ConfigurationManager
                .ConnectionStrings["QuanLyCongVanConnectionString"].ConnectionString;

            var tenDonVis = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string noiNhanRaw = null;

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();

                // 1) Lấy giá trị NoiNhan (có thể là chuỗi mã hoặc tên; có thể NULL)
                using (var cmd = new SqlCommand("SELECT NoiNhan FROM tblNoiDungCV WHERE MaCV = @MaCV", conn))
                {
                    cmd.Parameters.AddWithValue("@MaCV", maCv);
                    var o = cmd.ExecuteScalar();
                    if (o != null && o != DBNull.Value)
                        noiNhanRaw = o.ToString().Trim();
                }

                // 2) Nếu có NoiNhan → tách và chuẩn hóa từng token (mã/tên)
                if (!string.IsNullOrWhiteSpace(noiNhanRaw))
                {
                    var tokens = noiNhanRaw.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                           .Select(s => s.Trim())
                                           .Where(s => s.Length > 0)
                                           .ToList();

                    foreach (var tk in tokens)
                    {
                        // Thử map tk như MÃ đơn vị trước, nếu không có thì coi như TÊN đơn vị
                        using (var cmd = new SqlCommand(
                            "SELECT TenDonVi FROM tblDonVi WHERE MaDonVi = @v OR TenDonVi = @v", conn))
                        {
                            cmd.Parameters.AddWithValue("@v", tk);
                            var o = cmd.ExecuteScalar();
                            if (o != null && o != DBNull.Value)
                                tenDonVis.Add(o.ToString());
                            else
                                tenDonVis.Add(tk); // không map được mã → dùng tk như tên
                        }
                    }
                }
                else
                {
                    // 3) NoiNhan đang NULL → suy ra từ luồng gửi nhận thực tế
                    using (var cmd = new SqlCommand(@"
                SELECT DISTINCT dv.TenDonVi
                FROM   tblGuiNhan gn
                JOIN   tblNguoiDung nd ON nd.MaNguoiDung = gn.MaNguoiNhan
                JOIN   tblDonVi     dv ON dv.MaDonVi     = nd.MaDonVi
                WHERE  gn.MaCV = @MaCV", conn))
                    {
                        cmd.Parameters.AddWithValue("@MaCV", maCv);
                        using (var rd = cmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                var ten = rd["TenDonVi"] == DBNull.Value ? "" : rd["TenDonVi"].ToString().Trim();
                                if (!string.IsNullOrEmpty(ten)) tenDonVis.Add(ten);
                            }
                        }
                    }
                }
            }

            // 4) Render ra Label với danh sách đơn vị nhận
            if (tenDonVis.Count == 0)
            {
                lblDonViNhan.Text = "(Chưa có đơn vị nhận)";
                return;
            }

            // Chuyển danh sách thành chuỗi và hiển thị trong Label
            lblDonViNhan.Text = string.Join(", ", tenDonVis.OrderBy(s => s));
        }


        #endregion


        #region Helpers

        private static bool TryParseDdMMyyyy(string input, out DateTime date)
        {
            return DateTime.TryParseExact(input, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }

        private static string SafeText(string s) => s == null ? string.Empty : s.Trim();
        private static string NullSafe(string s) => s ?? string.Empty;

        private static string GetCvKey()
        {
            string key = SafeGetQS("id");
            if (!string.IsNullOrEmpty(key)) return key;
            key = SafeGetQS("macv");
            if (!string.IsNullOrEmpty(key)) return key;
            key = SafeGetQS("MaCV");
            return key;
        }


        private static string SafeGetQS(string name)
        {
            var ctx = System.Web.HttpContext.Current;
            if (ctx == null) return string.Empty;
            var val = ctx.Request.QueryString[name];
            return string.IsNullOrEmpty(val) ? string.Empty : val.Trim();
        }

        private static string EnsureUniquePath(string path)
        {
            if (!File.Exists(path)) return path;
            string dir = Path.GetDirectoryName(path);
            string name = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);
            int i = 1;
            string candidate = Path.Combine(dir, $"{name} ({i}){ext}");
            while (File.Exists(candidate))
            {
                i++;
                candidate = Path.Combine(dir, $"{name} ({i}){ext}");
            }
            return candidate;
        }

        #endregion
    }
}
