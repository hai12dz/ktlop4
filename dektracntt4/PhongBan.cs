using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dektracntt4
{
    public class PhongBan
    {
        public int MaPhongBan { get; set; }
        public string TenPhongBan { get; set; }

        public PhongBan(int maPhongBan, string tenPhongBan)
        {
            MaPhongBan = maPhongBan;
            TenPhongBan = tenPhongBan;
        }
    }

}
