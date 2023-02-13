using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReaLTaiizor.Forms;
using System.Windows.Forms;

namespace АИС_Театральная_касса
{
    public partial class main : LostForm
    {
        const string connectString = "Data Source=data.sqlite";
        MysqlQuary DB = new MysqlQuary(connectString);
        public main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openForm1();
        }

        private void openForm1()
        {
            addTicket addTicket = new addTicket(connectString);
            addTicket.FormClosed += ((s, e) => { this.Show(); });
            addTicket.Show();
            this.Hide();
        }

        private void openForm2()
        {
            LoginInAdminka LoginInAdminka = new LoginInAdminka(connectString);
            LoginInAdminka.FormClosed += ((s, e) => { this.Show(); });
            LoginInAdminka.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openForm2();
        }
    }
}
