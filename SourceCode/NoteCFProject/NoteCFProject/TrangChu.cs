using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DBContext;
using NoteCFProject.Model;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Tile;

namespace NoteCFProject
{
    public partial class FrmSoDoBan : DevExpress.XtraEditors.XtraForm
    {
        TileView TileView_DangChon = new TileView();
        int MaBan_DangChon = -1;
        string TenBan_DangChon = "";
        int Handle_DangChon = -1;
        int TinhTrangHienTai_DangChon = -1;

        public FrmSoDoBan()
        {
            InitializeComponent();
        }

        private void FrmSoDoBan_Load(object sender, EventArgs e)
        {
            // Danh sách sơ đồ bàn
            DataTable tbSoDoBan = DBConnection.QueryBySELECT("SELECT * FROM DANHMUC_SODOBAN WHERE STATUS = 1");

            List<DANHMUC_SODOBAN_OBJ> listSoDoBan = tbSoDoBan.ToList<DANHMUC_SODOBAN_OBJ>();

            //List<DANHMUC_SODOBAN_OBJ> listSoDoBan = DANHMUC_SODOBAN_CONTROLLER.GetListSoDoBan(tbSoDoBan);

            grcSoDoBan.DataSource = listSoDoBan;

            // Danh sách đồ ăn thức uống
            DataTable tbDoAnThucUong = DBConnection.QueryBySELECT(@"SELECT DATU.*, DVT.TENDONVITINH,GIADATU.GIABAN FROM DANHMUC_DOANTHUCUONG DATU
                                        LEFT JOIN DANHMUC_DONVITINH DVT ON DATU.MADONVITINH = DVT.MA
                                        LEFT JOIN GIADOANTHUCUONG GIADATU ON GIADATU.MADOANTHUCUONG = DATU.MA
                                        WHERE DATU.STATUS = 1 AND DVT.STATUS = 1 AND GIADATU.STATUS = 1
                                        ");

            List<DANHMUC_DOANTHUCUONG_OBJ> listDoAnThucUong = tbDoAnThucUong.ToList<DANHMUC_DOANTHUCUONG_OBJ>().ToList();

            //List<DANHMUC_DOANTHUCUONG_OBJ> listDoAnThucUong = DANHMUC_DOANTHUCUONG_CONTROLLER.GetListDoAnThucUong(tbDoAnThucUong);

            repositoryItemSearchLookUpEditTenMon.DataSource = listDoAnThucUong;

            repositoryItemSearchLookUpEditTenMon.ValueMember = "Ma";
            repositoryItemSearchLookUpEditTenMon.DisplayMember = "TenMon";
        }

        private void tileViewSoDoBan_ItemCustomize(object sender, TileViewItemCustomizeEventArgs e)
        {
            TileView view = sender as TileView;
            if ((int)view.GetRowCellValue(e.RowHandle, colTrangThaiHienTai) == 0) // Trống -> Không có người
            {
                e.Item.AppearanceItem.Normal.BackColor = Color.FromArgb(252, 251, 172);
                //e.Item.AppearanceItem.Focused.BackColor = Color.Fuchsia;
            }
            else
            {
                if ((int)view.GetRowCellValue(e.RowHandle, colTrangThaiHienTai) == 1) // Đang sử dụng -> Có người
                {
                    e.Item.AppearanceItem.Normal.BackColor = Color.FromArgb(253, 207, 218);
                    //e.Item.AppearanceItem.Focused.BackColor = Color.Fuchsia;
                }
            }
        }

        private void btnMoDongBan_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem bàn đó đang mở hay đóng

            // Nếu đang đóng -> mở
            // Kiểm tra danh sach thức uống đã có chưa
            // Nếu có thì mở
            // Danh sách món đã chọn


            // Nếu đang mở -> đóng 
            // Kiểm tra đã thanh toán hay in bill chưa
            // Nếu đã thanh toán thì mở 
        }

        // SỰ KIỆN NHẤN VÀO BÀN
        // NẾU BÀN ĐANG MỞ THÌ LOAD CÁC THỨC UỐNG ĐÃ CHỌN
        private void tileViewSoDoBan_ItemPress(object sender, DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventArgs e)
        {
            TileView view = sender as TileView;

            TileView_DangChon = view;

            Handle_DangChon = e.Item.RowHandle;

            MaBan_DangChon = (int)view.GetRowCellValue(e.Item.RowHandle, colMaBan);

            TenBan_DangChon = (string)view.GetRowCellValue(e.Item.RowHandle, colTenBan);

            TinhTrangHienTai_DangChon = (int)view.GetRowCellValue(e.Item.RowHandle, colTrangThaiHienTai);

            if (TinhTrangHienTai_DangChon == 0) // Trống -> Không có người
            {
                btnLuuHoaDon.Text = "Thêm hoá đơn";

                List<MONDACHON_OBJ> listMonDaChon = new List<MONDACHON_OBJ>();

                for (int i = 1; i < 21; i++)
                {
                    MONDACHON_OBJ newMonDaChon = new MONDACHON_OBJ
                    {
                        STT = i,
                    };
                    listMonDaChon.Add(newMonDaChon);
                }

                grcDanhSachThucUong.DataSource = listMonDaChon;
            }
            else
            {
                if (TinhTrangHienTai_DangChon == 1) // Đang sử dụng -> Có người
                {
                    HOADON_OBJ hoaDon = DBConnection.QueryBySELECT(string.Format(@"SELECT * FROM HOADON WHERE MABAN = '{0}' AND STATUS = 1", MaBan_DangChon)).ToList<HOADON_OBJ>().FirstOrDefault();

                    if (hoaDon.ThanhToan == 1) // Đã thanh toán -> chỉ được correct thanh toán, in
                    {
                        AnHienNutTheoChucNang("DaThanhToan");
                    }
                    else // Chưa thanh toán
                    {
                        AnHienNutTheoChucNang("ChuaThanhToan");

                        // Lấy danh sách đồ ăn đã chọn trước đó
                        string sql = string.Format(@"SELECT CAST(ROW_NUMBER() OVER(ORDER BY CT.MAHOADON) AS INT) AS STT,
                        HD.MA AS MAHOADON, CT.MA AS MACHITIETHOADON, CT.MAMON, GIA.GIABAN AS GIA, CT.SOLUONG, 
                        GIA.GIABAN * CT.SOLUONG AS THANHTIEN, CT.GHICHU 
                        FROM HOADON HD
                        LEFT JOIN CHITIETHOADON CT ON HD.MA = CT.MAHOADON
                        LEFT JOIN GIADOANTHUCUONG GIA ON GIA.MADOANTHUCUONG = CT.MAMON
                        WHERE HD.MABAN = '{0}' AND HD.STATUS = 1
                        AND CT.STATUS = 1", MaBan_DangChon);

                        DataTable tbMonDaChon = DBConnection.QueryBySELECT(sql);

                        List<MONDACHON_OBJ> listMonDaChon = tbMonDaChon.ToList<MONDACHON_OBJ>();

                        int stt = listMonDaChon.Count() + 1;

                        for (int i = stt; i < 21; i++)
                        {
                            MONDACHON_OBJ newMonDaChon = new MONDACHON_OBJ
                            {
                                STT = i,
                            };
                            listMonDaChon.Add(newMonDaChon);
                        }

                        grcDanhSachThucUong.DataSource = listMonDaChon;

                        TinhTongTien();
                    }
                }
            }
        }

        private void tileViewSoDoBan_ItemClick(object sender, DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventArgs e)
        {

        }

        #region Cac chuc nang hoa don
        private void btnLuuHoaDon_Click(object sender, EventArgs e)
        {
            if (TinhTrangHienTai_DangChon == 0) // Trống - > Thêm Món
            {
                List<MONDACHON_OBJ> listMonDaChon = new List<MONDACHON_OBJ>();

                listMonDaChon = (grvDanhSachThucUong.DataSource as List<MONDACHON_OBJ>);

                List<MONDACHON_OBJ> listMonDaChonResult = new List<MONDACHON_OBJ>();

                // Lấy danh sách đã chọn -> chỉ lấy khi có số lượng

                foreach (var item in listMonDaChon)
                {
                    if (item.MaMon != null && item.SoLuong > 0)
                    {
                        listMonDaChonResult.Add(item);
                    }
                }

                if (listMonDaChonResult.Count > 0)
                {
                    // Lưu Hoá Đơn
                    int maHoaDon = DBConnection.QueryByINSERT(string.Format(@"INSERT INTO [dbo].[HoaDon] ([MaBan],[NgayTao],[Status],[MaTaiKhoanTao],[ThanhToan])
                            VALUES ('{0}','{1}','{2}','{3}','{4}') ; ", MaBan_DangChon, DateTime.Now, (int)STATUS.DANGSUDUNG, 1, 0));

                    foreach (var item in listMonDaChonResult)
                    {
                        // Lưu Chi Tiết Hoá Đơn
                        int maChiTietHoaDon = DBConnection.QueryByINSERT(string.Format(@"INSERT INTO [dbo].[ChiTietHoaDon] ([MaHoaDon],[MaMon],[SoLuong],[Status])
                            VALUES ('{0}','{1}','{2}','{3}') ; ", maHoaDon, item.MaMon, item.SoLuong, (int)STATUS.DANGSUDUNG));
                    }

                    // Mở bàn
                    int maBan = DBConnection.QueryByUPDATE(string.Format(@"UPDATE [dbo].[DanhMuc_SoDoBan]
                            SET [TrangThaiHienTai] = '{0}'  WHERE Ma = '{1}'", (int)TRANGTHAIHIENTAIBAN.MOBAN, MaBan_DangChon));

                    // Cập nhật trạng thái bàn
                    tileViewSoDoBan.SetRowCellValue(Handle_DangChon, colTrangThaiHienTai, 1);

                    tileViewSoDoBan.RefreshData();

                }
                else
                {
                    MessageBox.Show("Vui lòng chọn món!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else // Cập nhật lại món
            {
                int maHoaDon = -1;

                HOADON_OBJ hoaDon = DBConnection.QueryBySELECT(string.Format(@"SELECT * FROM HOADON WHERE MABAN = '{0}' AND STATUS = 1", MaBan_DangChon)).ToList<HOADON_OBJ>().FirstOrDefault();

                if (hoaDon != null)
                {
                    maHoaDon = hoaDon.Ma;

                    // Cập nhật status chi tiết = -1 tất cả theo bàn
                    int maChiTietHoaDon = DBConnection.QueryByUPDATE(string.Format(@"UPDATE CHITIETHOADON SET STATUS = -1 WHERE MAHOADON = '{0}' AND STATUS = 1", maHoaDon));

                    // Thêm chi tiết mới
                    List<MONDACHON_OBJ> listMonDaChon = new List<MONDACHON_OBJ>();

                    listMonDaChon = (grvDanhSachThucUong.DataSource as List<MONDACHON_OBJ>);

                    List<MONDACHON_OBJ> listMonDaChonResult = new List<MONDACHON_OBJ>();

                    // Lấy danh sách đã chọn -> chỉ lấy khi có số lượng

                    foreach (var item in listMonDaChon)
                    {
                        if (item.MaMon != null && item.SoLuong > 0)
                        {
                            listMonDaChonResult.Add(item);
                        }
                    }

                    if (listMonDaChonResult.Count > 0)
                    {
                        foreach (var item in listMonDaChonResult)
                        {
                            // Lưu Chi Tiết Hoá Đơn
                            maChiTietHoaDon = DBConnection.QueryByINSERT(string.Format(@"INSERT INTO [dbo].[ChiTietHoaDon] ([MaHoaDon],[MaMon],[SoLuong],[GhiChu],[Status])
                            VALUES ('{0}','{1}','{2}','{3}', '{4}') ; ", maHoaDon, item.MaMon, item.SoLuong, item.GhiChu, (int)STATUS.DANGSUDUNG));
                        }
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng chọn món!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            HOADON_OBJ hoaDon = DBConnection.QueryBySELECT(string.Format(@"SELECT * FROM HOADON WHERE MABAN = '{0}' AND STATUS = 1", MaBan_DangChon)).ToList<HOADON_OBJ>().FirstOrDefault();

            if (hoaDon.ThanhToan == 1) // Đã thanh toán
            {
                if (MessageBox.Show(string.Format("Bạn muốn huỷ thanh toán hoá đơn cho {0} ?", TenBan_DangChon), "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) // Đồng ý huỷ thanh toán
                {
                    // Cập nhật thanh toán = 1 trong bảng Hoá Đơn
                    int maChiTietHoaDon = DBConnection.QueryByUPDATE(string.Format(@"UPDATE HOADON SET STATUS = -1, NGAYCAPNHAT = '{0}' WHERE MA = '{1}' AND STATUS = 1", DateTime.Now, hoaDon.Ma));

                    List<MONDACHON_OBJ> listMonDaChon = new List<MONDACHON_OBJ>();

                    listMonDaChon = (grvDanhSachThucUong.DataSource as List<MONDACHON_OBJ>);

                    List<MONDACHON_OBJ> listMonDaChonResult = new List<MONDACHON_OBJ>();

                    // Lấy danh sách đã chọn -> chỉ lấy khi có số lượng

                    foreach (var item in listMonDaChon)
                    {
                        if (item.MaMon != null && item.SoLuong > 0)
                        {
                            listMonDaChonResult.Add(item);
                        }
                    }

                    if (listMonDaChonResult.Count > 0)
                    {
                        // Lưu Hoá Đơn
                        int maHoaDon = DBConnection.QueryByINSERT(string.Format(@"INSERT INTO [dbo].[HoaDon] ([MaBan],[NgayTao],[Status],[MaTaiKhoanTao],[ThanhToan])
                            VALUES ('{0}','{1}','{2}','{3}','{4}') ; ", hoaDon.MaBan, DateTime.Now, (int)STATUS.DANGSUDUNG, 1, 0));

                        foreach (var item in listMonDaChonResult)
                        {
                            // Lưu Chi Tiết Hoá Đơn
                            maChiTietHoaDon = DBConnection.QueryByINSERT(string.Format(@"INSERT INTO [dbo].[ChiTietHoaDon] ([MaHoaDon],[MaMon],[SoLuong],[Status])
                            VALUES ('{0}','{1}','{2}','{3}') ; ", maHoaDon, item.MaMon, item.SoLuong, (int)STATUS.DANGSUDUNG));
                        }
                    }
                }
            }
            else
            {
                if (MessageBox.Show(string.Format("Bạn muốn thanh toán hoá đơn cho {0} ?", TenBan_DangChon), "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) // Đồng ý thanh toán
                {
                    // Cập nhật thanh toán = 1 trong bảng Hoá Đơn
                    int maChiTietHoaDon = DBConnection.QueryByUPDATE(string.Format(@"UPDATE HOADON SET THANHTOAN = 1, NGAYCAPNHAT = '{0}' WHERE MA = '{1}' AND STATUS = 1", DateTime.Now, hoaDon.Ma));

                    AnHienNutTheoChucNang("DaThanhToan");
                }
            }
        }

        #endregion

        #region Cac chuc nang ban

        #endregion

        #region Cac chuc nang tren danh sach hoa don
        private void repositoryItemSearchLookUpEditTenMon_EditValueChanged(object sender, EventArgs e)
        {
            SearchLookUpEdit edit = sender as SearchLookUpEdit;
            int rowHandle = edit.Properties.GetIndexByKeyValue(edit.EditValue);
            object row = edit.Properties.View.GetRow(rowHandle);
            var giaBan = (row as DANHMUC_DOANTHUCUONG_OBJ).GiaBan;

            MONDACHON_OBJ dataRowHandle = grvDanhSachThucUong.GetFocusedRow() as MONDACHON_OBJ;

            dataRowHandle.Gia = giaBan;

            dataRowHandle.ThanhTien = dataRowHandle.Gia * dataRowHandle.SoLuong;

            grvDanhSachThucUong.RefreshData();

            TinhTongTien();
        }

        private void repositoryItemSpinEditSoLuong_EditValueChanged(object sender, EventArgs e)
        {
            SpinEdit edit = sender as SpinEdit;

            MONDACHON_OBJ dataRowHandle = grvDanhSachThucUong.GetFocusedRow() as MONDACHON_OBJ;

            dataRowHandle.SoLuong = Convert.ToInt32(edit.Value);

            dataRowHandle.ThanhTien = dataRowHandle.Gia * dataRowHandle.SoLuong;

            grvDanhSachThucUong.RefreshData();

            TinhTongTien();
        }

        // Sự kiện xoá row in gridview
        private void repositoryItemButtonEditXoa_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            // Món cần xoá
            MONDACHON_OBJ dataRowHandle = grvDanhSachThucUong.GetFocusedRow() as MONDACHON_OBJ;

            if (dataRowHandle.MaChiTietHoaDon > 0)
            {
                // Update chi tiết hoá đơn = -1 theo mã hoá đơn chi tiết;
                int maChiTietHoaDon = DBConnection.QueryByUPDATE(string.Format(@"UPDATE [dbo].[ChiTietHoaDon]SET [Status] = -1WHERE [Ma] = {0}", dataRowHandle.MaChiTietHoaDon));
            }

            dataRowHandle.MaHoaDon = 0;
            dataRowHandle.MaChiTietHoaDon = 0;
            dataRowHandle.MaMon = 0;
            dataRowHandle.Gia = null;
            dataRowHandle.ThanhTien = null;
            dataRowHandle.SoLuong = null;
            dataRowHandle.GhiChu = "";
            grvDanhSachThucUong.RefreshData();

            // update lại tổng tiền
            TinhTongTien();
        }
        #endregion

        void TinhTongTien()
        {
            decimal tongcong = 0;

            List<MONDACHON_OBJ> listMonDaChon = new List<MONDACHON_OBJ>();

            listMonDaChon = (grvDanhSachThucUong.DataSource as List<MONDACHON_OBJ>);

            List<MONDACHON_OBJ> listMonDaChonResult = new List<MONDACHON_OBJ>();

            // Lấy danh sách đã chọn -> chỉ lấy khi có số lượng

            foreach (var item in listMonDaChon)
            {
                if (item.MaMon != null && item.SoLuong > 0)
                {
                    tongcong = tongcong + (item.Gia.Value * item.SoLuong.Value);
                }
            }

            txtTongCong.Text = tongcong.ToString("N0");

            decimal phuThu = txtPhuThu.Text != "" ? Convert.ToDecimal(txtPhuThu.Text) : 0;

            decimal giamGia = txtGiamGia.Text != "" ? Convert.ToDecimal(txtGiamGia.Text) : 0;

            decimal thanhTien = tongcong + phuThu - giamGia;

            txtThanhTien.Text = thanhTien.ToString("N0");
        }

        void AnHienNutTheoChucNang(string chucNang)
        {
            switch (chucNang)
            {
                case "DaThanhToan":
                    btnLuuHoaDon.Enabled = false;
                    btnThanhToan.Text = "Huỷ thanh toán";
                    break;
                case "ChuaThanhToan":
                    btnLuuHoaDon.Enabled = true;
                    btnThanhToan.Text = "Thanh toán";
                    btnLuuHoaDon.Text = "Cập nhật hoá đơn";
                    break;
            }
        }
    }
}