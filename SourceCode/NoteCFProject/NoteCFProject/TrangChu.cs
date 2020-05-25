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
        public FrmSoDoBan()
        {
            InitializeComponent();
        }

        private void FrmSoDoBan_Load(object sender, EventArgs e)
        {
            // Danh sách sơ đồ bàn
            DataTable tbSoDoBan = DBConnection.QueryBySELECT("SELECT * FROM DANHMUC_SODOBAN WHERE STATUS = 1");

            List<DANHMUC_SODOBAN_OBJ> listSoDoBan = DANHMUC_SODOBAN_CONTROLLER.GetListSoDoBan(tbSoDoBan);

            grcSoDoBan.DataSource = listSoDoBan;

            // Danh sách đồ ăn thức uống
            DataTable tbDoAnThucUong = DBConnection.QueryBySELECT(@"SELECT DATU.*, DVT.TENDONVITINH,GIADATU.GIABAN FROM DANHMUC_DOANTHUCUONG DATU
                                        LEFT JOIN DANHMUC_DONVITINH DVT ON DATU.MADONVITINH = DVT.MA
                                        LEFT JOIN GIADOANTHUCUONG GIADATU ON GIADATU.MADOANTHUCUONG = DATU.MA
                                        WHERE DATU.STATUS = 1 AND DVT.STATUS = 1 AND GIADATU.STATUS = 1
                                        ");

            List<DANHMUC_DOANTHUCUONG_OBJ> listDoAnThucUong = DANHMUC_DOANTHUCUONG_CONTROLLER.GetListDoAnThucUong(tbDoAnThucUong);

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

            if ((int)view.GetRowCellValue(e.Item.RowHandle, colTrangThaiHienTai) == 0) // Trống -> Không có người
            {
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
                if ((int)view.GetRowCellValue(e.Item.RowHandle, colTrangThaiHienTai) == 1) // Đang sử dụng -> Có người
                {
                    // Lấy danh sách đồ ăn đã chọn trước đó

                }
            }

            
        }

        private void tileViewSoDoBan_ItemClick(object sender, DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventArgs e)
        {

        }

        // Sự kiện xoá row in gridview
        private void repositoryItemButtonEditXoa_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
           
        }

        private void btnLuuHoaDon_Click(object sender, EventArgs e)
        {

        }

        private void repositoryItemSearchLookUpEditTenMon_EditValueChanged(object sender, EventArgs e)
        {
            SearchLookUpEdit edit = sender as SearchLookUpEdit;
            int rowHandle = edit.Properties.GetIndexByKeyValue(edit.EditValue);
            object row = edit.Properties.View.GetRow(rowHandle);
            var giaBan = (row as DANHMUC_DOANTHUCUONG_OBJ).GiaBan;

            MONDACHON_OBJ dataRowHandle = grvDanhSachThucUong.GetFocusedRow() as MONDACHON_OBJ;

            dataRowHandle.Gia = giaBan;

        }
    }
}