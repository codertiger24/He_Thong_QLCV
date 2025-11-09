<%@ Page Title="" Language="C#" MasterPageFile="~/QLCV.Master" AutoEventWireup="true"
    CodeBehind="SuaCongVan.aspx.cs" Inherits="QLCVan.SuaCongVan" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        :root {
            --ink:#0f172a; --red:#c00000; --blue:#0d6efd; --line:#e5e7eb; --h:44px; --br:8px;
            --bd:#dcdcdc; --bg:#f9f9f9; --bgf:#fff; --bdf:#0b57d0; --shadowf:0 0 0 2px rgba(11,87,208,.18);
            --px:14px; --py:11px; --fs:14px;
        }
        .content-header{background:transparent;padding:0;border-bottom:none;margin:0 auto 6px}
        .content-header-title{text-transform:uppercase;font-weight:700;font-size:20px;color:#444;margin:0 0 6px}
        .welcome-bar{background:#c00;color:#fff;border-radius:4px;padding:8px 0;margin:0 auto 26px;font-weight:bold;text-align:center;display:flex;align-items:center;justify-content:center;height:15px;overflow:hidden}
        .welcome-bar marquee{font-size:16px;font-weight:bold;color:#fff}
        .form-container{margin:40px auto;padding:30px;max-width:900px;border-radius:12px;background:#fff;box-shadow:0 4px 12px rgba(0,0,0,.08)}
        .form-title{text-align:center;font-weight:700;font-size:24px;margin-bottom:25px;color:#222}
        .form-grid{display:grid;grid-template-columns:repeat(2,1fr);gap:20px 30px}
        .form-field{display:flex;align-items:center;gap:15px}
        .form-field-full-width{grid-column:1/-1}
        .form-field-full-width .form-field-inner{display:flex;align-items:center;width:100%;gap:15px}
        .form-label{font-weight:600;color:#555;flex-shrink:0;width:120px;text-align:right}
        .form-input-control{flex-grow:1;display:flex;flex-direction:column;width:100%}
        .form-input,.form-select,.form-textarea,.form-static,.form-listbox,.form-radio,.file-upload-custom{
            width:100%;border:1px solid var(--bd);border-radius:var(--br);background:var(--bg);
            transition:border-color .2s,box-shadow .2s,background .2s;box-sizing:border-box;font-size:var(--fs);line-height:1.4
        }
        .form-input,.form-select,.form-static,.file-upload-custom,.form-radio{height:var(--h);display:flex;align-items:center;padding:0 var(--px)}
        .form-input{padding:var(--py) var(--px)}
        .form-textarea{min-height:108px;padding:var(--py) var(--px);resize:vertical}
        .form-listbox{height:116px;padding:8px 10px;background:var(--bg)}
        .form-input:focus,.form-select:focus,.form-textarea:focus,.form-listbox:focus{outline:none;border-color:var(--bdf);box-shadow:var(--shadowf);background:var(--bgf)}
        .form-static{color:#111827;border-style:dashed}
        .form-radio table{border-collapse:collapse}
        .form-radio td{padding-right:18px;white-space:nowrap}
        .form-radio input[type="radio"]{margin-right:6px}
        .file-upload-row,.file-list-row{display:flex;align-items:flex-start;width:100%;gap:15px}
        .file-upload-main-container{display:flex;align-items:center;gap:10px}
        .file-upload-custom{padding:0;overflow:hidden}
        .file-upload-label{background:#e8e8e8;color:#202124;border-right:1px solid var(--bd);padding:0 12px;
            height:calc(var(--h)-2px);display:flex;align-items:center;font-weight:600;border-radius:8px 0 0 8px}
        .file-upload-custom input[type="file"]{opacity:0;position:absolute;width:1px;height:1px;pointer-events:none}
        .file-upload-main-container .btn.btn-primary{height:var(--h);display:inline-flex;align-items:center;padding:0 16px}
        .file-list-control-container{display:flex;flex-grow:1;align-items:flex-end;gap:10px}
        .btn{display:inline-block;padding:10px 24px;border-radius:8px;cursor:pointer;font-size:16px;font-weight:600;text-decoration:none;transition:all .2s;border:none}
        .btn-primary{background:#0b57d0;color:#fff}.btn-primary:hover{background:#0949ae}
        .btn-quaylai{background:#dadce0;color:#202124}.btn-quaylai:hover{background:#c0c4c8}
        .form-buttons{grid-column:1/-1;display:flex;justify-content:flex-end;gap:10px;margin-top:25px}
        .note-red{color:#d32f2f;font-size:12px;margin-top:5px}
        @media (max-width:768px){
            .form-grid{grid-template-columns:1fr}
            .form-field,.form-field-full-width .form-field-inner,.file-upload-row,.file-list-row{flex-direction:column;align-items:stretch}
            .form-label{text-align:left;width:auto}
            .form-radio,.file-upload-custom{height:auto;min-height:var(--h)}
        }
    </style>

    <script src="Scripts/datepicker/jquery-1.10.2.js" type="text/javascript"></script>
    <script src="Scripts/datepicker/jquery-ui.js" type="text/javascript"></script>
    <link href="Scripts/datepicker/jquery-ui.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        jQuery(function ($) {
            $.datepicker.regional['vi'] = {
                closeText: 'Đóng', prevText: '&#x3c;Trước', nextText: 'Tiếp&#x3e;', currentText: 'Hôm nay',
                monthNames: ['Tháng Một', 'Tháng Hai', 'Tháng Ba', 'Tháng Tư', 'Tháng Năm', 'Tháng Sáu', 'Tháng Bảy', 'Tháng Tám', 'Tháng Chín', 'Tháng Mười', 'Th.Mười Một', 'Th.Mười Hai'],
                monthNamesShort: ['Th1', 'Th2', 'Th3', 'Th4', 'Th5', 'Th6', 'Th7', 'Th8', 'Th9', 'Th10', 'Th11', 'Th12'],
                dayNames: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'], dayNamesMin: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
                weekHeader: 'Tu', dateFormat: 'dd/mm/yy', firstDay: 0
            };
            $.datepicker.setDefaults($.datepicker.regional['vi']);
            $('#<%= txtngaybanhanh.ClientID %>').datepicker({ changeMonth:true, changeYear:true, yearRange:'2000:2040' });
            $('#<%= txtngaygui.ClientID %>').datepicker({ changeMonth: true, changeYear: true, yearRange: '2000:2040' });
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="content-header">
        <h2 class="content-header-title">SỬA CÔNG VĂN</h2>
    </div>

    <div class="welcome-bar">
        <marquee behavior="scroll" direction="left" scrollamount="6">
            Cập nhật thông tin công văn trong hệ thống Quản lý Công văn điện tử.
        </marquee>
    </div>

    <div class="form-container">
        <h3 class="form-title">CHỈNH SỬA CÔNG VĂN</h3>

        <div class="form-grid">
            <!-- Tiêu đề -->
            <div class="form-field-full-width">
                <div class="form-field-inner">
                    <asp:Label runat="server" Text="Tiêu đề:" CssClass="form-label" />
                    <div class="form-input-control">
                        <asp:TextBox ID="txttieude" CssClass="form-input" runat="server" placeholder="Nhập vào tiêu đề" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txttieude" ErrorMessage="* Nhập tiêu đề" CssClass="note-red" />
                    </div>
                </div>
            </div>

            <!-- Số CV -->
            <div class="form-field">
                <asp:Label runat="server" Text="Số CV:" CssClass="form-label" />
                <div class="form-input-control">
                    <asp:TextBox ID="txtsocv" CssClass="form-input" runat="server" placeholder="Nhập vào số công văn" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtsocv" ErrorMessage="* Nhập số công văn" CssClass="note-red" />
                </div>
            </div>

            <!-- Loại công văn (RO) -->
            <div class="form-field">
                <asp:Label runat="server" Text="Loại công văn:" CssClass="form-label" />
                <div class="form-input-control">
                    <asp:Label ID="lblLoaiCVValue" runat="server" CssClass="form-static" />
                </div>
            </div>

            <!-- Ngày ban hành -->
            <div class="form-field">
                <asp:Label runat="server" Text="Ngày ban hành:" CssClass="form-label" />
                <div class="form-input-control">
                    <asp:TextBox ID="txtngaybanhanh" runat="server" CssClass="form-input" placeholder="dd/mm/yyyy" />
                </div>
            </div>

            <!-- Ngày gửi -->
            <div class="form-field">
                <asp:Label runat="server" Text="Ngày gửi:" CssClass="form-label" />
                <div class="form-input-control">
                    <asp:TextBox ID="txtngaygui" runat="server" CssClass="form-input" placeholder="dd/mm/yyyy" />
                </div>
            </div>

            <!-- Cơ quan ban hành -->
            <div class="form-field">
                <asp:Label runat="server" Text="Cơ quan ban hành:" CssClass="form-label" />
                <div class="form-input-control">
                    <asp:TextBox ID="txtcqbh" CssClass="form-input" runat="server" placeholder="Nhập cơ quan ban hành" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtcqbh" ErrorMessage="* Nhập cơ quan ban hành" CssClass="note-red" />
                </div>
            </div>

            <!-- Đơn vị nhận (RO) -->
            <div class="form-field">
                <asp:Label runat="server" Text="Đơn vị nhận:" CssClass="form-label" />
                <div class="form-input-control">
                    <asp:Label ID="lblDonViNhan" runat="server" CssClass="form-static" />
                </div>
            </div>

            <!-- Người ký -->
            <div class="form-field">
                <asp:Label runat="server" Text="Người ký:" CssClass="form-label" />
                <div class="form-input-control">
                    <asp:TextBox ID="txtNguoiKy" CssClass="form-input" runat="server" placeholder="Nhập người ký" />
                </div>
            </div>

            <!-- Bảo mật -->
            <div class="form-field">
                <asp:Label runat="server" Text="Bảo mật:" CssClass="form-label"></asp:Label>
                <div class="form-input-control">
                    <div class="radio-group">
                        <asp:RadioButtonList ID="RadioButtonList1" runat="server" RepeatDirection="Horizontal" CssClass="radio-list" CellSpacing="20">
                            <asp:ListItem Selected="True" Value="Có">Có</asp:ListItem>
                            <asp:ListItem Value="Không">Không</asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>

            <!-- Người duyệt (RO) -->
            <div class="form-field">
                <asp:Label runat="server" Text="Người duyệt:" CssClass="form-label" />
                <div class="form-input-control">
                    <asp:Label ID="lblNguoiDuyetValue" runat="server" CssClass="form-static" />
                </div>
            </div>

            <!-- Trích yếu -->
            <div class="form-field-full-width">
                <div class="form-field-inner">
                    <asp:Label runat="server" Text="Trích yếu:" CssClass="form-label" />
                    <div class="form-input-control">
                        <asp:TextBox ID="txttrichyeu" CssClass="form-textarea" runat="server" TextMode="MultiLine" Rows="4" placeholder="Nhập nội dung trích yếu" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txttrichyeu" ErrorMessage="* Nhập trích yếu" CssClass="note-red" />
                    </div>
                </div>
            </div>

            <!-- Ghi chú -->
            <div class="form-field-full-width">
                <div class="form-field-inner">
                    <asp:Label runat="server" Text="Ghi chú:" CssClass="form-label" />
                    <div class="form-input-control">
                        <asp:TextBox ID="txtGhiChu" CssClass="form-textarea" runat="server" TextMode="MultiLine" Rows="4" placeholder="Nhập ghi chú" />
                    </div>
                </div>
            </div>

            <!-- Upload -->
            <div class="form-field-full-width">
                <div class="file-upload-row">
                    <asp:Label runat="server" Text="File (nếu có):" CssClass="form-label"></asp:Label>
                    <div class="file-upload-main-container">
                        <div class="file-upload-custom">
                            <asp:FileUpload ID="FileUpload1" runat="server" />
                            <label for="<%= FileUpload1.ClientID %>" class="file-upload-label">Choose File</label>
                            <span class="file-chosen-text">No file chosen</span>
                        </div>
                        <asp:Button ID="Button1" runat="server" CssClass="btn btn-primary" Text="Upload" OnClick="btnUp_Click" CausesValidation="False" />
                    </div>
                </div>
            </div>

            <!-- Danh sách tệp -->
            <div class="form-field-full-width">
                <div class="file-list-row">
                    <asp:Label runat="server" Text="Tệp đính kèm:" CssClass="form-label"></asp:Label>
                    <div class="file-list-control-container">
                        <asp:ListBox ID="ListBox1" runat="server" Width="100%" CssClass="form-listbox" />
                        <asp:Button ID="btnRemove" runat="server" CssClass="btn btn-primary" Text="Xóa" OnClick="btnRemove_Click" CausesValidation="False" />
                    </div>
                </div>
                <asp:Label ID="lblloi" runat="server" Text="" CssClass="note-red" Style="display:block;margin-left:135px;margin-top:6px;"></asp:Label>
            </div>

            <!-- Buttons -->
            <div class="form-buttons">
                <asp:Button ID="btnQuayLai" runat="server" CssClass="btn btn-quaylai" Text="Quay lại" OnClick="btnQuayLai_Click" CausesValidation="False" />
                <asp:Button ID="btnCapNhat" runat="server" CssClass="btn btn-primary" Text="Cập nhật" OnClick="btnCapNhat_Click" />
            </div>
        </div>
    </div>
</asp:Content>
