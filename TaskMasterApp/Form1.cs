using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaskMasterApp.Model;

namespace TaskMasterApp
{
    public partial class Form1 : Form
    {
        private tmDBContext tmContext;
        public Form1()
        {
            InitializeComponent();

            tmContext = new tmDBContext();

            var statuses = tmContext.Statuses.ToList();

            foreach(Status s in statuses)
            {
                cboStaus.Items.Add(s);
            }
            refreshData();
        }

        private void refreshData()
        {
            BindingSource bi = new BindingSource();

            var query = from t in tmContext.Tasks
                        orderby t.DueDate
                        select new { t.Id, TaskName = t.Name, StatusName = t.Status.Name, t.DueDate };

            bi.DataSource = query.ToList();
            dataGridView1.DataSource = bi;
            dataGridView1.Refresh();
        }

        private void cmdCreateTask_Click(object sender, EventArgs e)
        {
            if(cboStaus.SelectedItem != null && txtTask.Text != string.Empty)
            {
                var newTask = new Model.Task
                {
                    Name = txtTask.Text,
                    StatusId = (cboStaus.SelectedItem as Status).Id,
                    DueDate = dateTimePicker1.Value
                };

                tmContext.Tasks.Add(newTask);
                tmContext.SaveChanges();
                refreshData();

                MessageBox.Show("Task created successfully");
            }
            else
            {
                MessageBox.Show("Please make sure all data has been entered.");
            }
        }

        private void cmdDeleteTask_Click(object sender, EventArgs e)
        {
            var t = tmContext.Tasks.Find((int) dataGridView1.SelectedCells[0].Value);
            tmContext.Tasks.Remove(t);
            tmContext.SaveChanges();

            refreshData();
        }

        private void cmdUpdateTask_Click(object sender, EventArgs e)
        {
            if(cmdUpdateTask.Text == "Update")
            {
                txtTask.Text = dataGridView1.SelectedCells[1].Value.ToString();
                dateTimePicker1.Value = (DateTime)dataGridView1.SelectedCells[3].Value;
                foreach(Status s in cboStaus.Items)
                {
                    if(s.Name == dataGridView1.SelectedCells[2].Value.ToString())
                    {
                        cboStaus.SelectedItem = s;
                    }
                }
                cmdUpdateTask.Text = "Save";
            }
            else if (cmdUpdateTask.Text == "Save")
            {
                var t = tmContext.Tasks.Find((int)dataGridView1.SelectedCells[0].Value);

                t.Name = txtTask.Text;
                t.StatusId = (cboStaus.SelectedItem as Status).Id;
                t.DueDate = dateTimePicker1.Value;

                tmContext.SaveChanges();
                refreshData();

                cmdUpdateTask.Text = "Update";
                txtTask.Text = string.Empty;
                cboStaus.Text = "Please Select...";
                dateTimePicker1.Value = DateTime.Now;
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            cmdUpdateTask.Text = "Update";
            txtTask.Text = string.Empty;
            cboStaus.Text = "Please Select...";
            dateTimePicker1.Value = DateTime.Now;
        }
    }
}
