using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteCFProject.Model
{
    public class MONDACHON_OBJ // MÓN ĐÃ CHỌN
    {
        public int STT { get; set; }
        public int Ma { get; set; }
        public int MaMon { get; set; }
        public decimal? Gia { get; set; }
        public int? SoLuong { get; set; }
        public string GhiChu { get; set; }
        public int Xoa { get; set; }
    }

    public class DANHMUC_DOANTHUCUONG_OBJ
    {
        public int Ma { get; set; }
        public string TenMon { get; set; }
        public int MaDonViTinh { get; set; }
        public string TenDonViTinh { get; set; }
        public decimal GiaBan { get; set; }
        public int Status { get; set; }
    }

    public class DANHMUC_DOANTHUCUONG_CONTROLLER
    {
        public static List<DANHMUC_DOANTHUCUONG_OBJ> GetListDoAnThucUong(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                var listDoAnThucUong = (from rw in dt.AsEnumerable()
                                   select new DANHMUC_DOANTHUCUONG_OBJ()
                                   {
                                       Ma = Convert.ToInt32(rw["Ma"]),
                                       TenMon = Convert.ToString(rw["TenMon"]),
                                       MaDonViTinh = Convert.ToInt32(rw["MaDonViTinh"]),
                                       TenDonViTinh = Convert.ToString(rw["TenDonViTinh"]),
                                       GiaBan = Convert.ToDecimal(rw["GiaBan"]),
                                       Status = Convert.ToInt32(rw["Status"])
                                   }).ToList();

                return listDoAnThucUong;
            }
            return null;
        }
    }
}
