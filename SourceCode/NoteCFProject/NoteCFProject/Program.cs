using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.UserSkins;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DBContext;

namespace NoteCFProject
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            BonusSkins.Register();
            SkinManager.EnableFormSkins();
            UserLookAndFeel.Default.SetSkinStyle("DevExpress Style");

            DBUtils.DATASOURCE = @"DESKTOP-0U2BU05\SQLEXPRESS";
            DBUtils.DATABASE = "CoffeeShopManagementSoftware";
            DBUtils.USERNAME = "";
            DBUtils.PASSWORD = "";

            Application.Run(new FrmSoDoBan());
        }
    }
}
