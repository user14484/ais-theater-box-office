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
    public partial class addTicket : Form
    {
        MysqlQuary DB;
        Dictionary<int, Dictionary<string, string>> sessions = new Dictionary<int, Dictionary<string, string>>();
        Dictionary<int, string> sessionsComboBox = new Dictionary<int, string>();
        
        Dictionary<int, Dictionary<string, string>> performances = new Dictionary<int, Dictionary<string, string>>();
        Dictionary<int, string> performancesComboBox = new Dictionary<int, string>();

        Dictionary<int, Dictionary<string, string>> places = new Dictionary<int, Dictionary<string, string>>();
        Dictionary<int, string> placesComboBox = new Dictionary<int, string>();

        Dictionary<int, string> status = new Dictionary<int, string>() {
            { 1, "Куплен" },
            { 2, "Забронирован" }
        };

        private string TextPrint = "";
        public addTicket(string connectString)
        {
            DB = new MysqlQuary(connectString);
            InitializeComponent();
            DB.Open();
            sessions = DB.QuaryMas("SELECT * FROM sessions");
            performances = DB.QuaryMas("SELECT * FROM performances");
            places = DB.QuaryMas("SELECT * FROM places");


            // Заполнение сеансов
            foreach(Dictionary<string, string> temp in sessions.Values)
            {
                sessionsComboBox.Add(Convert.ToInt32(temp["id"]), temp["title"].ToString() + " | " + temp["start_time"].ToString() + " | " + temp["end_time"].ToString() + " | " + temp["price"].ToString() + " ₽");
            }
            // Заполнение спектаклей
            foreach(Dictionary<string, string> temp in performances.Values)
            {
                string genre = DB.QuaryStr("SELECT title FROM genres WHERE id=" + temp["id_genre"]);
                performancesComboBox.Add(Convert.ToInt32(temp["id"]), temp["title"].ToString() + " | " + temp["data"].ToString() + " | " + genre);
            }
            // Заполнение мест
            foreach (Dictionary<string, string> temp in places.Values)
            {
                placesComboBox.Add(Convert.ToInt32(temp["id"]), "Место - " + temp["number_row"].ToString() + temp["number_place"].ToString());
            }

            materialComboBox1.DataSource = sessionsComboBox.ToList();
            materialComboBox2.DataSource = performancesComboBox.ToList();
            materialComboBox3.DataSource = placesComboBox.ToList();
            materialComboBox4.DataSource = status.ToList();

            materialComboBox1.DisplayMember = "Value";
            materialComboBox1.ValueMember = "Key";

            materialComboBox2.DisplayMember = "Value";
            materialComboBox2.ValueMember = "Key";

            materialComboBox3.DisplayMember = "Value";
            materialComboBox4.ValueMember = "Key";

            materialComboBox4.DisplayMember = "Value";
            materialComboBox4.ValueMember = "Key";
        }

        private void lostCancelButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lostAcceptButton1_Click(object sender, EventArgs e)
        {
            try
            {
                int id_session, id_performance, id_place, status;
                string identifier;


                id_session = Convert.ToInt32(materialComboBox1.SelectedValue);
                id_performance = Convert.ToInt32(materialComboBox2.SelectedValue);
                id_place = ((KeyValuePair<int, string>)materialComboBox3.SelectedValue).Key;
                status = Convert.ToInt32(materialComboBox4.SelectedValue);
                identifier = Guid.NewGuid().ToString("N");

                string quary = string.Format(
                    "INSERT INTO tickets (id_session, id_performance, id_place, status, identifier) VALUES({0}, {1}, {2}, {3}, '{4}')",
                    id_session,
                    id_performance,
                    id_place,
                    status,
                    identifier
                    );

                string session, performance, place;

                session = $"Сеанс: {sessions[id_session]["title"]}\n" +
                    $"Начало сеанса: {sessions[id_session]["start_time"]}\n" +
                    $"Окончание сеанса: {sessions[id_session]["end_time"]}\n" +
                    $"Стоимость: {sessions[id_session]["price"]}";

                performance = $"Название спектакля: {performances[id_performance]["title"]}";

                place = $"Место: {places[id_place]["number_row"]}{places[id_place]["number_place"]}";

                TextPrint = $"Идентификатор - {identifier}\n{session}\n{performance}\n{place}";

                if (printPreviewDialog1.ShowDialog() == DialogResult.OK)
                {
                    printDocument1.Print();
                }
                DB.Quary(quary);
                DB.success("Билет успешно добавлен");
            }
            catch (Exception ex)
            {
                DB.error(ex.Message);
            }
        }

        private void addTicket_FormClosing(object sender, FormClosingEventArgs e)
        {
            DB.Close();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawString(TextPrint, new Font("Arial", 14), Brushes.Black, 0, 0);
        }
    }
}
