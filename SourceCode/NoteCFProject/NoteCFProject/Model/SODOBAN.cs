using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteCFProject.Model
{
    enum TRANGTHAIHIENTAIBAN
    {
        MOBAN = 1,
        DONGBAN = 0
    }

    enum STATUS
    {
        XOA = -1,
        DANGSUDUNG = 1
    }

    public class DANHMUC_SODOBAN_OBJ
    {
        public int Ma { get; set; }
        public string TenBan { get; set; }
        public int TrangThaiHienTai { get; set; }
        public int Status { get; set; }
    }

    public class DANHMUC_SODOBAN_CONTROLLER
    {
        public static List<DANHMUC_SODOBAN_OBJ> GetListSoDoBan(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                var listSoDoBan = (from rw in dt.AsEnumerable()
                                   select new DANHMUC_SODOBAN_OBJ()
                                   {
                                       Ma = Convert.ToInt32(rw["Ma"]),
                                       TenBan = Convert.ToString(rw["TenBan"]),
                                       TrangThaiHienTai = Convert.ToInt32(rw["TrangThaiHienTai"]),
                                       Status = Convert.ToInt32(rw["Status"])
                                   }).ToList();

                return listSoDoBan;
            }
            return null;
        }
    }
}