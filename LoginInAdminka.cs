using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ReaLTaiizor.Controls;

namespace АИС_Театральная_касса
{
    public partial class LoginInAdminka : Form
    {
        MysqlQuary DB;
        /*Dictionary<int, Dictionary<string, string>> users = new Dictionary<int, Dictionary<string, string>>();*/
        Dictionary<string, string> DataUser = new Dictionary<string, string>();
        string connectString;

        public LoginInAdminka(string connectString1)
        {
            connectString = connectString1;
            DB = new MysqlQuary(connectString);
            InitializeComponent();
            DB.Open();
        }

        private void lostCancelButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lostAcceptButton1_Click(object sender, EventArgs e)
        {
            string login, password;

            login = foreverTextBox1.Text;
            password = DB.GetMD5Hash(foreverTextBox2.Text);

            string query = string.Format(
                "SELECT count(*) FROM users WHERE login='{0}' AND password='{1}'",
                login,
                password
                );

            switch(Convert.ToInt32(DB.QuaryStr(query)))
            {
                case 1:
                    query = string.Format(
                        "SELECT * FROM users WHERE login='{0}' AND password='{1}'",
                        login,
                        password
                        );
                    DataUser = DB.QuaryMas(query).First().Value;
                    openFormAdminka();
                    break;
                default:
                    DB.error("Логин или пароль неверные!");
                    break;
            }
        }

        private void openFormAdminka()
        {
            Adminka Adminka = new Adminka(DataUser, connectString);
            Adminka.FormClosed += ((s, e) => { 
                this.Show();
                foreverTextBox1.Text = "";
                foreverTextBox2.Text = "";
            });
            Adminka.Show();
            this.Hide();
        }

        private void LoginInAdminka_FormClosing(object sender, FormClosingEventArgs e)
        {
            DB.Close();
        }
    }
}
