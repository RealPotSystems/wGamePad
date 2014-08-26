using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Data;

namespace vGamePad
{
    public class AstClock
    {
        public ObjectDataProvider provider { get; set; }
        private DispatcherTimer timer = null;
        private List<string> emojiList = new List<string>()
        {
            "🕛","🕧",
            "🕐","🕜",
            "🕑","🕝",
            "🕒","🕞",
            "🕓","🕟",
            "🕔","🕠",
            "🕕","🕡",
            "🕖","🕢",
            "🕗","🕣",
            "🕘","🕤",
            "🕙","🕥",
            "🕚","🕦"
        };

        public AstClock()
        {
            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            provider.Refresh();
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        public string CurrentAstDateTime()
        {
            DateTime datetime = DateTime.Now;
            double sec = datetime.Hour * 60 * 60 + datetime.Minute * 60 + datetime.Second;
            sec = (sec * 20) % (24 * 60 * 60);
            double h = Math.Floor((double)(sec / 3600));
            double m = Math.Floor((double)(sec / 60)) % 60;
            int index = (int)h % 12 * 2 + (m >= 30 ? 1 : 0);
            string timestr = String.Format("{0} AST {1:00}:{2:00}", emojiList[index], h, m);
            double rt = 0.0;
            string temp = "";
            if (h < 6)
            {
                rt = (6 * 60 * 60 - sec) / 20;
                temp = "朝";
            }
            else if (h < 18)
            {
                // 夜まで
                rt = (18 * 60 * 60 - sec) / 20;
                temp = "夜";
            }
            else
            {
                // 朝まで
                rt = (24 * 60 * 60 - sec + 6 * 60 * 60) / 20;
                temp = "朝";
            }
            string rstr = String.Format("{0}まであと{1:00}分{2:00}秒",temp , Math.Floor(rt / 60), Math.Floor(rt % 60));
            return timestr + "  " + rstr;
        }
    }
}
