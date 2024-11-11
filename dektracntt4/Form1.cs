using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace dektracntt4
{
    public partial class Form1 : Form
    {
        string connectString = @"Data Source=hai\SQLEXPRESS;Initial Catalog=kiemtrahosonhanvien;Integrated Security=True;Encrypt=False";

        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adt;//cau noi giua datatable vs sql server
        DataTable dt = new DataTable();//tuong tac vs grid data view
        public Form1()
        {
            InitializeComponent();
            con = new SqlConnection(connectString);
            loadData();

            LoadChatLieuComboBox();

        }



        private void loadData()
        {
            try
            {
                dt.Clear();
                con.Open(); // Mở kết nối thủ công
                cmd = new SqlCommand("SELECT * FROM NhanVien", con);
                adt = new SqlDataAdapter(cmd);
                adt.Fill(dt);
                con.Close(); // Đóng kết nối thủ công
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
            }
        }

        private void LoadChatLieuComboBox()
        {
            try
            {
                con.Open();
            string query = "SELECT MaPhongBan, TenPhongBan FROM PhongBan";
                SqlCommand command = new SqlCommand(query, con);

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // Thêm MaPhongBan và TenPhongBan vào ComboBox (giả sử ComboBox là comboBoxPhongBan)
                    comboBoxPhongBan.Items.Add(new { MaPhongBan = reader["MaPhongBan"], TenPhongBan = reader["TenPhongBan"].ToString() });
                }
                reader.Close();
                comboBoxPhongBan.DisplayMember = "TenPhongBan";
                comboBoxPhongBan.ValueMember = "MaPhongBan";



            }
            finally
            {
                con.Close();
            }


        






        }


      
        private void btnAnh_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            openFileDialog1.Title = "Chọn ảnh đại diện";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string imagePath = openFileDialog1.FileName;
                pictureBox1.Image = Image.FromFile(imagePath);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }


        private void btnThem_Click(object sender, EventArgs e)
        {
            // Kiểm tra các trường dữ liệu cần nhập
            if (string.IsNullOrEmpty(textBoxMaNV.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã NV.");
                return;
            }

            if (string.IsNullOrEmpty(textBoxTenNV.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên NV.");
                return;
            }

            // Kiểm tra Giới tính
            if (!radioButtonNam.Checked && !radioButtonNu.Checked)
            {
                MessageBox.Show("Vui lòng chọn Giới tính.");
                return;
            }

            if (string.IsNullOrEmpty(textBoxMucLuong.Text))
            {
                MessageBox.Show("Vui lòng nhập Mức lương.");
                return;
            }

            // Nếu vượt qua tất cả các kiểm tra, tiếp tục xử lý thêm nhân viên.


            foreach (DataRow row in dt.Rows)
            {
                if (row["MaNV"].ToString() == textBoxMaNV.Text)
                {
                    MessageBox.Show("Mã nhân viên đã tồn tại. Vui lòng nhập mã khác.");
                    textBoxMaNV.Focus();
                    return;
                }
            }






            string maNV = textBoxMaNV.Text;
            string tenNV = textBoxTenNV.Text;
            string soDT = textBoxSDT.Text;
            string gioiTinh = radioButtonNam.Checked ? "Nam" : "Nữ";
            string anh = openFileDialog1.FileName != "" ? SaveImageToLocal(openFileDialog1.FileName) : ""; // Lưu ảnh vào thư mục và lấy tên ảnh
            decimal mucLuong = Convert.ToDecimal(textBoxMucLuong.Text);


            // Kiểm tra xem người dùng có chọn phòng ban trong ComboBox không
         

            int maPhongBan = (int)comboBoxPhongBan.SelectedIndex;
            maPhongBan++;


            try
            {
                con.Open();

                cmd = new SqlCommand("INSERT INTO NhanVien (MaNV, TenNV,SoDT, GioiTinh, Anh,MaPhongBan,MucLuong) VALUES (@MaNV, @TenNV, @SoDT, @GioiTinh, @Anh, @MaPhongBan, @MucLuong)", con);
                cmd.Parameters.AddWithValue("@MaNV", maNV);
                cmd.Parameters.AddWithValue("@TenNV", tenNV);
                cmd.Parameters.AddWithValue("@SoDT", soDT);
                cmd.Parameters.AddWithValue("@GioiTinh", gioiTinh);
                cmd.Parameters.AddWithValue("@Anh", anh);
                cmd.Parameters.AddWithValue("@MaPhongBan", maPhongBan);
                cmd.Parameters.AddWithValue("@MucLuong", mucLuong);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Thêm vật liệu thành công!");
                con.Close(); // Đóng kết nối thủ công
                clearTextBoxes();
                loadData();


            }

            finally { con.Close(); }

        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            // Kiểm tra các trường dữ liệu cần nhập
            if (string.IsNullOrEmpty(textBoxMaNV.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã NV.");
                return;
            }

            if (string.IsNullOrEmpty(textBoxTenNV.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên NV.");
                return;
            }

            // Kiểm tra Giới tính
            if (!radioButtonNam.Checked && !radioButtonNu.Checked)
            {
                MessageBox.Show("Vui lòng chọn Giới tính.");
                return;
            }

            if (string.IsNullOrEmpty(textBoxMucLuong.Text))
            {
                MessageBox.Show("Vui lòng nhập Mức lương.");
                return;
            }

            string maNV = textBoxMaNV.Text;
            string tenNV = textBoxTenNV.Text;
            string soDT = textBoxSDT.Text;
            string gioiTinh = radioButtonNam.Checked ? "Nam" : "Nữ";
            string anh = openFileDialog1.FileName != "" ? SaveImageToLocal(openFileDialog1.FileName) : ""; // Lưu ảnh vào thư mục và lấy tên ảnh
            decimal mucLuong = Convert.ToDecimal(textBoxMucLuong.Text);
            string currentImage = GetCurrentImagePath(maNV);
            if (string.IsNullOrEmpty(anh) && !string.IsNullOrEmpty(currentImage))
            {
                anh = currentImage; // Giữ nguyên ảnh cũ nếu không chọn ảnh mới
            }

            int maPhongBan = (int)comboBoxPhongBan.SelectedIndex;
            maPhongBan++;

            try
            {
                con.Open();
                // Cập nhật thông tin nhân viên
                cmd = new SqlCommand("UPDATE NhanVien SET TenNV = @TenNV, SoDT = @SoDT, GioiTinh = @GioiTinh, Anh = @Anh, MaPhongBan = @MaPhongBan, MucLuong = @MucLuong WHERE MaNV = @MaNV", con);
                cmd.Parameters.AddWithValue("@MaNV", maNV);
                cmd.Parameters.AddWithValue("@TenNV", tenNV);
                cmd.Parameters.AddWithValue("@SoDT", soDT);
                cmd.Parameters.AddWithValue("@GioiTinh", gioiTinh);
                cmd.Parameters.AddWithValue("@Anh", anh);
                cmd.Parameters.AddWithValue("@MaPhongBan", maPhongBan);
                cmd.Parameters.AddWithValue("@MucLuong", mucLuong);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Sửa thông tin nhân viên thành công!");
                con.Close(); // Đóng kết nối thủ công
                clearTextBoxes();
                loadData(); // Tải lại dữ liệu
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa thông tin: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }


        private void btnXoa_Click(object sender, EventArgs e)
        {
            // Kiểm tra Mã NV đã được chọn
            if (string.IsNullOrEmpty(textBoxMaNV.Text))
            {
                MessageBox.Show("Vui lòng chọn nhân viên để xóa.");
                return;
            }

            string maNV = textBoxMaNV.Text;

            // Xác nhận việc xóa
            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa nhân viên {maNV}?", "Xác nhận", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                try
                {
                    con.Open();
                    // Câu lệnh xóa nhân viên
                    cmd = new SqlCommand("DELETE FROM NhanVien WHERE MaNV = @MaNV", con);
                    cmd.Parameters.AddWithValue("@MaNV", maNV);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Xóa nhân viên thành công!");

                    con.Close();
                    clearTextBoxes();
                    loadData(); // Tải lại dữ liệu sau khi xóa
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa nhân viên: " + ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
        }


        private void btnThoat_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có muốn thoát không?", "Xác nhận", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }



        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Kiểm tra xem có click vào hàng không
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex]; // Lấy hàng vừa click

                textBoxMaNV.Text = row.Cells["MaNV"].Value.ToString();
                textBoxTenNV.Text = row.Cells["TenNV"].Value.ToString();
                textBoxSDT.Text = row.Cells["SoDT"].Value.ToString();

                string gioiTinh = row.Cells["GioiTinh"].Value.ToString();
                if (gioiTinh.Equals("Nam"))
                    radioButtonNam.Checked = true;
                else
                    radioButtonNu.Checked = true;

             
                if (row.Cells["Anh"].Value != DBNull.Value)
                {
                    string imageFile = row.Cells["Anh"].Value.ToString();
                    string imagePath = Path.Combine(Application.StartupPath, "Images", imageFile);
                    if (!File.Exists(imagePath))
                    {
                        pictureBox1.Image = null;

                    }
                    else
                    {
                        pictureBox1.Image = Image.FromFile(imagePath);
                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;


                    }

                }

                textBoxMucLuong.Text = row.Cells["MucLuong"].Value.ToString();

                // Hiển thị lại ComboBox với Phòng Ban đã được chọn
                int maPhongBan = Convert.ToInt32(row.Cells["MaPhongBan"].Value); // Lấy MaPhongBan từ dòng hiện tại

                // Đặt giá trị cho ComboBox
                comboBoxPhongBan.SelectedIndex = maPhongBan-1;

                // Kích hoạt các nút
                btnSua.Enabled = true;
                btnXoa.Enabled = true;
                btnThem.Enabled = false;
            }
        }


        private string GetCurrentImagePath(string maNV)
        {
            string currentImagePath = null;

            try
            {
                using (SqlConnection con = new SqlConnection(connectString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT Anh FROM NhanVien WHERE MaNV = @MaNV", con))
                    {
                        cmd.Parameters.AddWithValue("@MaNV", maNV);
                        object result = cmd.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                        {
                            currentImagePath = result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lấy ảnh cũ: " + ex.Message);
            }

            return currentImagePath;
        }


        private string SaveImageToLocal(string imagePath)
        {

            if (string.IsNullOrEmpty(imagePath))
            {
                return "";  // Hoặc bạn có thể trả về một giá trị khác tùy theo nhu cầu
            }

            if (!File.Exists(imagePath))
            {
                return "";  // Trả về null nếu file không tồn tại
            }

            string imageDirectory = Path.Combine(Application.StartupPath, "Images");
            if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);  // Tạo thư mục nếu chưa tồn tại
            }

            string fileName = Path.GetFileNameWithoutExtension(imagePath);
            string extension = Path.GetExtension(imagePath);
            TimeSpan timeNow = DateTime.Now.TimeOfDay;
            int totalSeconds = (int)timeNow.TotalSeconds;
            string newFileName = $"{fileName}_{totalSeconds}{extension}";

            string destinationPath = Path.Combine(imageDirectory, newFileName);
            File.Copy(imagePath, destinationPath, true);  // Lưu ảnh vào thư mục Images

            return newFileName;  // Trả về tên file để lưu vào CSDL
        }



        private void clearTextBoxes()
        {
            textBoxMaNV.Clear();
            textBoxTenNV.Clear();
            textBoxSDT.Clear();
            textBoxMucLuong.Clear();
            radioButtonNam.Checked = false;
            radioButtonNu.Checked = false;

            // Đặt lại ComboBox (nếu có)
            comboBoxPhongBan.SelectedIndex = -1;
            pictureBox1.Image = null;
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
            btnThem.Enabled = true;
        }







    }
}
