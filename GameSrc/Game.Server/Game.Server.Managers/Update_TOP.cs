using Bussiness;
using Bussiness.Managers;
using System;
using System.Net;
using System.Configuration;

namespace Game.Server
{
    public class Update_TOP
    {
        private DateTime _lastUpdateTime = DateTime.MinValue;
        // URL: http://gunnyarena.com/request/Tank.Request.CelebList/CreateAllCeleb.ashx
        // Note: /request/ is IIS virtual directory mapping to Tank.Request application
        private string req = ConfigurationManager.AppSettings["request"] + "Tank.Request.CelebList/CreateAllCeleb.ashx";

        // Interval in milliseconds: 10 seconds = 10000ms (for testing)
        // When stable, change to 5 minutes = 300000ms
        private int _updateInterval = 10000; // 10 seconds

        private void uptop()
        {
            try
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] DEBUG: Calling URL: {req}");
                WebClient webClient = new WebClient();
                webClient.Headers.Add("User-Agent", "Game.Server/1.0");
                string result = webClient.DownloadString(req);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] TU DONG UPDATE PHONG CAO THU THANH CONG!");
                Console.ResetColor();
                _lastUpdateTime = DateTime.Now;
            }
            catch (WebException webEx)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                if (webEx.Response is HttpWebResponse httpResponse)
                {
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] WARNING: Ranking update skipped - HTTP {httpResponse.StatusCode}: {webEx.Message}");
                }
                else
                {
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] WARNING: Ranking update skipped - {webEx.Message}");
                }
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] WARNING: Ranking update skipped - {ex.Message}");
                Console.ResetColor();
            }
        }

        public void UpdateCeleb()
        {
            // Check if enough time has passed since last update
            if (_lastUpdateTime == DateTime.MinValue ||
                (DateTime.Now - _lastUpdateTime).TotalMilliseconds >= _updateInterval)
            {
                this.uptop();
            }
        }
    }
}