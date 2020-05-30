using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteCFProject.Model
{
    public class HOADON_OBJ // MÓN ĐÃ CHỌN
    {
        public int Ma { get; set; }
        public int MaBan { get; set; }
        public int? TongTien { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public int MaTaiKhoanTao { get; set; }
        public int ThanhToan { get; set; }
        public int Status { get; set; }
    }
}
