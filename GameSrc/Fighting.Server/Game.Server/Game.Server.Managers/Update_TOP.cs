using Bussiness;
using Bussiness.Managers;
using System;
using System.Net;
using System.Configuration;

namespace Game.Server
{
    public class Update_TOP
    {
        private bool isuptop;
        private string req = ConfigurationManager.AppSettings["request"] + "CelebList/CreateAllCeleb.ashx";

        private void uptop()
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.DownloadString(req);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("TU DONG UPDATE PHONG CAO THU THANH CONG!");
                Console.ResetColor();
            }
            catch
            {
                Console.WriteLine("ERROR");
            }
        }

        public void UpdateCeleb()
        {
            if (DateTime.Now.Minute > 5)
                isuptop = false;
            if (DateTime.Now.Minute <= 5 && !isuptop)
            {
                this.uptop();
                isuptop = true;
            }

        }
    }
}