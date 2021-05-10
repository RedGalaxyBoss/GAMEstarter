﻿using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FontAwesome.Sharp;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GAMEstarter
{
    public partial class FormDevBoard : Form
    {
        public FormDevBoard()
        {
            InitializeComponent();
        }
        
        #region Управление формой
        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;

            lblTime.Text = dt.ToString("HH:mm:ss");

            string dov = CultureInfo.CurrentCulture.TextInfo.
                ToTitleCase(dt.ToString("dddd", CultureInfo.GetCultureInfo("ru-ru")));
            lblMDY.Text = (dov + ", " + dt.ToLongDateString()).ToString();
        }

        void VisibleMenu()
        {
            if (btnHideMenu.Text != "")
            {
                btnHideMenu.IconChar = IconChar.ThList;
                btnHideMenu.Text = btnAnalytics.Text =
                    btnEditPr.Text = btnAccount.Text =
                    btnStudio.Text = btnCatalog.Text =
                    btnHome.Text = btnSettings.Text = btnExit.Text = "";
                //btnAddPr.Text = "";
                panelMenu.Width = 70;
            }
            else
            {
                btnHideMenu.IconChar = IconChar.EllipsisV;

                btnHideMenu.Text = "Скрыть меню";
                btnAnalytics.Text = "Аналитаика";
                btnEditPr.Text = "Мои проекты";
                //btnAddPr.Text = "Создать проект";
                btnAccount.Text = "Аккаунт";
                btnStudio.Text = "Студия";
                btnCatalog.Text = "Каталог";
                btnHome.Text = "Домой";
                btnSettings.Text = "Настройки";
                btnExit.Text = "Выход";

                panelMenu.Width = 220;
            }
        }

        private void btnHideMenu_Click(object sender, EventArgs e)
        {
            VisibleMenu();
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
                WindowState = FormWindowState.Maximized;
            else WindowState = FormWindowState.Normal;
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void FormDevBoard_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        #endregion

        #region Дизайн

        Settings clrs = new Settings();

        private void FormDevBoard_Load(object sender, EventArgs e)
        {
            leftBorderBtn = new Panel();
            leftBorderBtn.Size = new Size(7, 60);
            panelMenu.Controls.Add(leftBorderBtn);

            clrs.getSettings();
            lblTime.Visible = lblMDY.Visible = clrs.Clock;


            panelMenu.BackColor = btnExit.BackColor = 
                panelFormManage.BackColor = panelDownSepar.BackColor = clrs.Ucolor;

            panelChildForm.BackColor = panelLogo.BackColor = btnExit.BackColor = clrs.Dcolor;

            btnHideMenu.ForeColor = btnAnalytics.ForeColor = btnEditPr.ForeColor =
                btnAccount.ForeColor = btnStudio.ForeColor = btnCatalog.ForeColor =
                btnHome.ForeColor = btnSettings.ForeColor = btnExit.ForeColor =
                lblTime.ForeColor = lblMDY.ForeColor = clrs.Fcolor;

            btnHideMenu.IconColor = btnAnalytics.IconColor = btnEditPr.IconColor =
                btnAccount.IconColor = btnStudio.IconColor = btnCatalog.IconColor =
                btnHome.IconColor = btnSettings.IconColor = btnExit.IconColor = clrs.Fcolor;

            if (clrs.ChildStart != "") OpenDefault(clrs.ChildStart);
        }

        private IconButton currentButton;
        private Panel leftBorderBtn;
        int tempIndex;
        private Random random = new Random();
        private Color SelectThemeColor()
        {
            if(clrs.ChildColor != "")
                return ColorTranslator.FromHtml(clrs.ChildColor);

            int index = random.Next(clrs.ColorList.Count);
            while (tempIndex == index)
            {
                index = random.Next(clrs.ColorList.Count);
            }
            tempIndex = index;
            string color = clrs.ColorList[index];
            return ColorTranslator.FromHtml(color);
        }

        private void ActivateButton(object senderBtn, Color color)
        {
            DisableButton();
            if (senderBtn == null) return;

            currentButton = (IconButton)senderBtn;
            currentButton.BackColor = Color.FromArgb(14, 52, 65);
            panelFormManage.BackColor = color;
            panelDownSepar.BackColor = color;
            currentButton.ForeColor = color;
            currentButton.IconColor = color;

            leftBorderBtn.BackColor = color;
            leftBorderBtn.Location = new Point(0, currentButton.Location.Y);
            leftBorderBtn.Show();
            leftBorderBtn.BringToFront();
        }

        private void DisableButton()
        {
            if (currentButton == null) return;

            currentButton.BackColor = clrs.Ucolor;
            panelFormManage.BackColor = clrs.Ucolor;
            panelDownSepar.BackColor = clrs.Ucolor;

            currentButton.ForeColor = clrs.Fcolor;
            currentButton.IconColor = clrs.Fcolor;
            leftBorderBtn.Hide();
        }

        #endregion

        #region Код
        void OpenDefault(string name)
        {
            if (name == "Аналитика") btnAnalytics_Click(btnAnalytics, new EventArgs());
            if (name == "Мои проекты") btnEditPr_Click(btnEditPr, new EventArgs());
            if (name == "Аккаунт") btnAccount_Click(btnAccount, new EventArgs());
            if (name == "Студия") btnStudio_Click(btnStudio, new EventArgs());
        }

        public static Color color;
        public int idCur, idStudCur;

        private Form activeForm;
        private void OpenChildForm(Form childForm, object btnSender)
        {
            if (activeForm != null)
                activeForm.Close();

            color = SelectThemeColor();
            ActivateButton(btnSender, color);
            
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panelChildForm.Controls.Add(childForm);
            panelChildForm.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
            Text = "GameSTARTER - " + childForm.Text;
        }

        private void btnAnalytics_Click(object sender, EventArgs e)
        {
            if (!CheckStudio(idCur))
            {
                if (MessageBox.Show("Для начала вам необходимо создать студию. Перейти к созданию?",
                    "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No) return;

                btnAccount_Click(sender as Button, e);
            }
            else
            {
                FormAnalytics fra = new FormAnalytics();
                fra.usersBindingSource.Filter = "id_user = " + idCur;
                fra.studiosBindingSource.Filter = "id_studio = " + idStudCur;
                fra.id_studio = idStudCur;
                OpenChildForm(fra, sender);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите выйти из аккаунта? " +
                "\r\nПри повторном входе потребуется снова ввести пароль", "Внимание",
                MessageBoxButtons.YesNo,MessageBoxIcon.Information) == DialogResult.No) return;

            try
            {
                RegistryKey currentUserKey = Registry.CurrentUser;
                RegistryKey BookShelf = currentUserKey.OpenSubKey("GameSTARTER", true);
                BookShelf.DeleteValue("login");
                BookShelf.DeleteValue("password");
                currentUserKey.DeleteSubKey("GameSTARTER");
                BookShelf.Close();
            }
            catch
            {

            }

            Close();
        }

        bool CheckStudio(int id)
        {
            bool flag = true;
            string txtquery = @"select id_studio from Users where id_user = " + id;

            SqlConnection con = new SqlConnection(Form1.txtcon);

            con.Open();
            SqlCommand query1 = new SqlCommand(txtquery, con);
            SqlDataReader read = query1.ExecuteReader();
            read.Read();

            try
            {
                if (read["id_studio"] == null) flag = true;
                else idStudCur = Convert.ToInt32(read["id_studio"]);
                con.Close();
            }
            catch { flag = false; }
            
            return flag;
        }

        private void btnEditPr_Click(object sender, EventArgs e)
        {
            if (!CheckStudio(idCur))
            {
                if (MessageBox.Show("Для начала вам необходимо создать студию. Перейти к созданию?",
                    "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No) return;

                btnAccount_Click(sender as Button, e);
            }
            else
            {
                FormMyPr frmp = new FormMyPr();
                frmp.id_studio = idStudCur.ToString();
                OpenChildForm(frmp, sender);
            }
        }

        private void btnAccount_Click(object sender, EventArgs e)
        {
            sender = btnAccount;
            FormAccount fra = new FormAccount();
            fra.usersBindingSource.Filter = "id_user = " + idCur;
            OpenChildForm(fra, sender);
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormSettings(), sender);
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            if (activeForm == null) return;
            if (MessageBox.Show("Все несохранённые данные исчезнут.\r\nВыйти в панель управления?", 
                "Внимание", MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.No) return;

            DisableButton();
            activeForm.Close();
            Text = "GameSTARTER - Панель управления";
        }

        private void btnStudio_Click(object sender, EventArgs e)
        {

        }

        private void btnCatalog_Click(object sender, EventArgs e)
        {

        }

        private void pbLogo_Click(object sender, EventArgs e)
        {
            if (activeForm == null) return;

            if (MessageBox.Show("Вернуться в панель управления?" +
                "\r\nвсе несохранённые данные удалятся", "Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            DisableButton();
            activeForm.Close();
            activeForm = null;
            Text = "GameSTARTER - Панель разработчика";
        }
        #endregion
    }
}