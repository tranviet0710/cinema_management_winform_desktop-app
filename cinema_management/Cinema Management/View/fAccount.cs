﻿using PBL3_GiaBao.EF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PBL3_GiaBao.View
{
    public partial class fAccount : Form
    {
        string MaNV { get; set; }
        TaiKhoan TKhoanLogin { get; set; }
        public fAccount(string idnv,TaiKhoan TKLogin)
        {
            InitializeComponent();
            MaNV = idnv;
            txtMaNV.Text = MaNV;
            TKhoanLogin = TKLogin;
        }
        List<TaiKhoan> listtk;
        public List<TaiKhoan> ListTK { get => listtk; set { listtk = value; showTK(); } }

        private void showTK()
        {
            ArrayList arr = new ArrayList();
            BLL.BLL_TaiKhoan.Instance.GetTKByIDNV_BLL(MaNV).ForEach(p => arr.Add(new {p.idTK, p.idNV,p.NhanVien.HoTen,p.UserName,p.Pass,p.LoaiTK }));
            dataGridView1.DataSource = arr;
        }
        private void Clear()
        {
            txtIDTK.Text = "";
            txtUser.Text = "";
            txtPass.Text = "";
            txtLoaiTK.Text = "";
        }


        private void fAccount_Load(object sender, EventArgs e)
        {
            txtMaNV.Enabled = false;
            btSave.Hide();
            Clear();
            ListTK = BLL.BLL_TaiKhoan.Instance.GetTKByIDNV_BLL(MaNV);
        }
        private bool checktxt(string txt)
        {
            return txt != string.Empty;
        }

        private void btTaoTK_Click(object sender, EventArgs e)
        {
            if (checktxt(txtLoaiTK.Text) && checktxt(txtPass.Text) && checktxt(txtUser.Text))
            {
                if (Convert.ToInt32(txtLoaiTK.Text) > 2 || Convert.ToInt32(txtLoaiTK.Text) < 1)
                {
                    MessageBox.Show("Định dạng thông tin không hợp lệ, vui lòng kiểm tra!", "Thông báo");
                }
                else
                {
                    try
                    {
                        TaiKhoan tmp = new TaiKhoan
                        {
                            idTK=txtIDTK.Text,
                            LoaiTK = Convert.ToInt32(txtLoaiTK.Text),
                            UserName = txtUser.Text,
                            Pass = txtPass.Text,
                            idNV = MaNV,
                        };
                        if (!BLL.BLL_TaiKhoan.Instance.isExitsTK_BLL(tmp.UserName,tmp.Pass) && !BLL.BLL_TaiKhoan.Instance.isExitsTK_BLL(tmp.idTK))
                        {
                            BLL.BLL_TaiKhoan.Instance.AddTKhoan_BLL(tmp);
                            fAccount_Load(sender, e);
                        }
                        else MessageBox.Show("Đã tồn tại mã tài khoản này, mời nhập lại!", "Thông báo");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Định dạng thông tin không hợp lệ2, vui lòng kiểm tra!", "Thông báo");
                    }
                }

            }
            else
            {
                MessageBox.Show("Mời nhập đủ thông tin tài khoản!!");
            }
        
        }

        private void btSuaTK_Click(object sender, EventArgs e)
        {
            if(dataGridView1.SelectedRows.Count==1)
            {
                TaiKhoan tk = BLL.BLL_TaiKhoan.Instance.GetTaiKhoanByUserPass_BLL(dataGridView1.SelectedRows[0].Cells["UserName"].Value.ToString(), dataGridView1.SelectedRows[0].Cells["Pass"].Value.ToString());
                txtUser.Text = tk.UserName;
                txtLoaiTK.Text = tk.LoaiTK.ToString();
                txtPass.Text = tk.Pass;
                txtIDTK.Text = tk.idTK;
                txtIDTK.Enabled = false;

                btSave.Show();
            }   
            else
            {
                MessageBox.Show("Mời chọn 1 tài khoản để đổi mật khẩu!", "Thông báo");
            }    
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            if (checktxt(txtLoaiTK.Text) && checktxt(txtPass.Text) && checktxt(txtUser.Text))
            {
                if (Convert.ToInt32(txtLoaiTK.Text) > 2 || Convert.ToInt32(txtLoaiTK.Text) < 1)
                {
                    MessageBox.Show("Định dạng thông tin không hợp lệ, vui lòng kiểm tra!", "Thông báo");
                }
                else
                {
                    if (!BLL.BLL_TaiKhoan.Instance.isExitsTK_BLL(txtUser.Text,txtPass.Text))
                    {
                        try
                        {
                            TaiKhoan tk = BLL.BLL_TaiKhoan.Instance.GetTaiKhoanByIDTK_BLL(txtIDTK.Text);
                            tk.Pass = txtPass.Text;
                            tk.UserName = txtUser.Text;
                            tk.LoaiTK = Convert.ToInt32(txtLoaiTK.Text);

                            BLL.BLL_TaiKhoan.Instance.SuaThongTin_BLL(tk);
                            fAccount_Load(sender, e);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Định dạng thông tin không hợp lệ, vui lòng kiểm tra!", "Thông báo");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Đã tồn tại tài khoản này!", "Thông Báo");
                    }
                }
            }
        }

        private void btDong_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btXoaTK_Click(object sender, EventArgs e)
        {
            List<string> listMaTK = new List<string>();
            if (dataGridView1.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    listMaTK.Add(row.Cells["idTK"].Value.ToString());
                    if (listMaTK.Exists(p => p.Equals(TKhoanLogin.idTK)))
                    {
                        listMaTK.Remove(TKhoanLogin.idTK);
                        MessageBox.Show("Bạn không thể xóa tài khoản đang đăng nhập!");
                    }
                }
                listMaTK.ForEach(p => BLL.BLL_TaiKhoan.Instance.DeleteTK_BLL(p));
                fAccount_Load (sender, e);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn nhân viên để xóa", "Thông báo!!");
            }
        }
    }
}
