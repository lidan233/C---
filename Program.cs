using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ESRI.ArcGIS;

namespace MapControlApplication7
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

           
            if (!RuntimeManager.Bind(ProductCode.Desktop))
            {
                if (!RuntimeManager.Bind(ProductCode.Engine))
                {
                    MessageBox.Show("Unable to bind to ArcGIS runtime. Application will be shut down.");
                    return;
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ESRI.ArcGIS.RuntimeManager.BindLicense(ESRI.ArcGIS.ProductCode.EngineOrDesktop, ESRI.ArcGIS.LicenseLevel.GeodatabaseUpdate);
            Application.Run(new Login());
        }
    }
}