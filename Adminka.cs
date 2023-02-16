using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Controls;

namespace АИС_Театральная_касса
{
    public partial class Adminka : LostForm
    {
        MysqlQuary DB;
        Dictionary<int, Dictionary<string, string>> tickets = new Dictionary<int, Dictionary<string, string>>();
        Dictionary<int, string> ticketsComboBox = new Dictionary<int, string>();
        Dictionary<int, string> places = new Dictionary<int, string>();
        Dictionary<int, string> sessions = new Dictionary<int, string>();
        Dictionary<int, string> performances = new Dictionary<int, string>();
        Dictionary<int, string> status = new Dictionary<int, string>() {
            { 1, "Куплен" },
            { 2, "Забронирован" }
        };
        int idTicketSelected = 0;


        DataSet DataSessions = new DataSet();
        DataSet DataPerformances = new DataSet();
        DataSet DataGenres = new DataSet();
        DataSet DataUsers = new DataSet();





        public Adminka(Dictionary<string, string> DataUser, string connectString)
        {
            DB = new MysqlQuary(connectString);
            InitializeComponent();
            DB.Open();

            foreach(Dictionary<string, string> temp in DB.QuaryMas("SELECT * FROM sessions").Values)
            {
                sessions.Add(Convert.ToInt32(temp["id"]), temp["title"]);
            }
            foreach(Dictionary<string, string> temp in DB.QuaryMas("SELECT * FROM places").Values)
            {
                places.Add(Convert.ToInt32(temp["id"]), temp["number_row"] + temp["number_place"]);
            }
            foreach(Dictionary<string, string> temp in DB.QuaryMas("SELECT * FROM performances").Values)
            {
                performances.Add(Convert.ToInt32(temp["id"]), temp["title"]);
            }
            LoadListTickets();
            LoadTicket();
            LoadSessions();
            LoadPerformances();
            LoadGenres();
            LoadUsers();
        }

        private void LoadSessions()
        {
            DataSessions.Clear();
            DataSessions = LoadGrids(DataSessions, "SELECT * FROM sessions", dataGridView1);
            dataGridView1.Columns["title"].HeaderText = "Название";
            dataGridView1.Columns["start_time"].HeaderText = "Время начала";
            dataGridView1.Columns["end_time"].HeaderText = "Время окончания";
            dataGridView1.Columns["price"].HeaderText = "Цена";
        }

        private void LoadPerformances()
        {
            DataPerformances.Clear();
            DataPerformances = LoadGrids(DataPerformances, "SELECT * FROM performances", dataGridView2);
            dataGridView2.Columns["title"].HeaderText = "Название";
            dataGridView2.Columns["duration"].HeaderText = "Длительность";
            dataGridView2.Columns["data"].HeaderText = "Дата";
            dataGridView2.Columns["id_genre"].HeaderText = "Айди жанра";
        }

        private void LoadGenres()
        {
            DataGenres.Clear();
            DataGenres = LoadGrids(DataGenres, "SELECT * FROM genres", dataGridView3);
            dataGridView3.Columns["title"].HeaderText = "Название";
            dataGridView3.Columns["description"].HeaderText = "Описание";
        }

        private void LoadUsers()
        {
            DataUsers= LoadGrids(DataUsers, "SELECT * FROM users", dataGridView4);
            dataGridView4.Columns["name"].HeaderText = "Имя";
            dataGridView4.Columns["login"].HeaderText = "Логин";
            dataGridView4.Columns["password"].HeaderText = "Хеш MD5 пароля";
        }

        private void LoadListTickets()
        {
            tickets.Clear();
            ticketsComboBox.Clear();
            aloneComboBox1.DataSource = null;
            tickets = DB.QuaryMas("SELECT * FROM tickets");

            foreach (Dictionary<string, string> temp in tickets.Values)
            {
                ticketsComboBox.Add(Convert.ToInt32(temp["id"]), "Сеанс: " + sessions[Convert.ToInt32(temp["id_session"])] + " | Спектакль: " + performances[Convert.ToInt32(temp["id_performance"])] + " | Место: " + places[Convert.ToInt32(temp["id_place"])] + " | Статус: " + status[Convert.ToInt32(temp["status"])] + " | Идентификатор: " + temp["identifier"]);
            }

            idTicketSelected = Convert.ToInt32(tickets.First().Value["id"]);

            aloneComboBox1.DataSource = ticketsComboBox.ToList();

            aloneComboBox1.DisplayMember = "Value";
            aloneComboBox1.ValueMember = "Key";
            aloneComboBox1.SelectedValue = idTicketSelected;
            aloneComboBox1.Refresh();
        }

        private void aloneComboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            idTicketSelected = (int)aloneComboBox1.SelectedValue;
            LoadTicket();
        }

        private void LoadTicket()
        {
            aloneComboBox2.DataSource = sessions.ToList();

            aloneComboBox2.DisplayMember = "Value";
            aloneComboBox2.ValueMember = "Key";
            aloneComboBox2.SelectedValue = Convert.ToInt32(tickets[idTicketSelected]["id_session"]);

            aloneComboBox3.DataSource = performances.ToList();

            aloneComboBox3.DisplayMember = "Value";
            aloneComboBox3.ValueMember = "Key";
            aloneComboBox3.SelectedValue = Convert.ToInt32(tickets[idTicketSelected]["id_performance"]);

            aloneComboBox4.DataSource = places.ToList();

            aloneComboBox4.DisplayMember = "Value";
            aloneComboBox4.ValueMember = "Key";
            aloneComboBox4.SelectedValue = Convert.ToInt32(tickets[idTicketSelected]["id_place"]);

            aloneComboBox5.DataSource = status.ToList();

            aloneComboBox5.DisplayMember = "Value";
            aloneComboBox5.ValueMember = "Key";
            aloneComboBox5.SelectedValue = Convert.ToInt32(tickets[idTicketSelected]["status"]);

            textBox1.Text = tickets[idTicketSelected]["identifier"];
        }

        private void lostAcceptButton1_Click(object sender, EventArgs e)
        {
            try
            {
                string query = string.Format(
                    "UPDATE tickets SET id_session={0}, id_performance={1}, id_place={2}, status={3} WHERE id={4}",
                    (int)aloneComboBox2.SelectedValue,
                    (int)aloneComboBox3.SelectedValue,
                    (int)aloneComboBox4.SelectedValue,
                    (int)aloneComboBox5.SelectedValue,
                    idTicketSelected
                    );

                DB.Quary(query);
                DB.success("Билет успешно отредактирован!");
                LoadListTickets();
            }
            catch (Exception ex)
            {
                DB.error(ex.Message);
            }
        }

        private void lostCancelButton1_Click(object sender, EventArgs e)
        {
            try
            {
                DB.Quary($"DELETE FROM TICKETS WHERE id={idTicketSelected}");
                DB.success("Билет успешно удалён!");
                LoadListTickets();
                LoadTicket();
            } catch(Exception ex)
            {
                DB.error(ex.Message);
            }
        }

        private void Adminka_FormClosing(object sender, FormClosingEventArgs e)
        {
            DB.Close();
        }

        private DataSet LoadGrids(DataSet data, string query, DataGridView dataGridView)
        {
            data.Clear();
            data = DB.QuaryData(query);
            dataGridView.DataSource = null;
            dataGridView.DataSource = data.Tables[0].DefaultView;
            dataGridView.Refresh();
            return data;
        }

        private void lostButton1_Click(object sender, EventArgs e)
        {
            LoadSessions();
        }

        private void lostButton2_Click(object sender, EventArgs e)
        {
            LoadPerformances();
        }

        private void lostButton3_Click(object sender, EventArgs e)
        {
            LoadGenres();
        }

        private void lostButton4_Click(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void lostAcceptButton2_Click(object sender, EventArgs e)
        {
            DB.QuaryDataUpdate("SELECT * FROM sessions", DataSessions);
            DB.success("Изменения применены!");
            LoadSessions();
        }

        private void lostAcceptButton3_Click(object sender, EventArgs e)
        {
            DB.QuaryDataUpdate("SELECT * FROM performances", DataPerformances);
            DB.success("Изменения применены!");
            LoadPerformances();
        }

        private void lostAcceptButton4_Click(object sender, EventArgs e)
        {
            DB.QuaryDataUpdate("SELECT * FROM genres", DataGenres);
            DB.success("Изменения применены!");
            LoadGenres();
        }

        private void lostAcceptButton5_Click(object sender, EventArgs e)
        {
            DB.QuaryDataUpdate("SELECT * FROM users", DataUsers);
            DB.success("Изменения применены!");
            LoadUsers();
        }
    }
}
