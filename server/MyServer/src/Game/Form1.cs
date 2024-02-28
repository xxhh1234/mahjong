using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyServer.src.Core.Data;
using MyServer.src.Game;

namespace MyServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ButtonStartClick(object sender, EventArgs e)
        {
            Server server = new Server("127.0.0.1", 4999);
            server.Start();
            ButtonStart.Enabled = false;
        }

        public Queue<int> Tiles = new Queue<int>(108);
        public TileTypeTileNum GetTypeTileNum(int num)
        {
            int left = 1, right = 4, start = (int)TileTypeTileNum.wan_1, end;
            for (int i = 1; i <= 10; ++i)
            {
                if (i <= 3) end = start + 9;
                else end = start + 1;
                for (; start < end; ++start)
                {
                    if (left <= num && num <= right) return (TileTypeTileNum)start;
                    left += 4; right += 4;
                }
                start = end + 11;
            }
            return TileTypeTileNum.bai;
        }
        public void InitTiles()
        {
            // 1. 获取1到108不重复的108个随机数
            Random ran = new Random();
            Queue<int> tile = new Queue<int>(108);
            while (true)
            {
                int num = ran.Next(1, 109);
                if (!tile.Contains(num)) tile.Enqueue(num);
                if (tile.Count == 108) break;
            }
            // 2. 获取对应的牌型数字并且添加到房间的牌堆
            int count = tile.Count;
            
            if (count > 0)
            {
                for (int i = 0; i < count; ++i)
                {
                    int num = tile.Dequeue();
                    Tiles.Enqueue(GetTypeTileNum(num).GetHashCode());
                }
            }
           
        }
        private void ButtonTestClick(object sender, EventArgs e)
        {
            UserData userInfo = new UserData();
            userInfo.id = "1";
            userInfo.username = "老大";
            userInfo.coin = 1000;
            userInfo.diamond = 100;
            userInfo.sex = 1;
            LocalDataManager.AddData("1", userInfo);

            userInfo.id = "2";
            userInfo.username = "老二";
            userInfo.coin = 2000;
            userInfo.diamond = 200;
            userInfo.sex = 2;
            LocalDataManager.AddData("2", userInfo);

            userInfo.id = "3";
            userInfo.username = "老三";
            userInfo.coin = 3000;
            userInfo.diamond = 300;
            userInfo.sex = 1;
            LocalDataManager.AddData("3", userInfo);

            userInfo.id = "4";
            userInfo.username = "老四";
            userInfo.coin = 4000;
            userInfo.diamond = 400;
            userInfo.sex = 2;
            LocalDataManager.AddData("4", userInfo);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
