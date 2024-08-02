using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pexeso
{
    public partial class Pexeso : Form
    {
        Image blind = Properties.Resources.blind;
        Random random = new Random();
        int p1 = 0, p2;
        int score = 0;
        int counter = 0;
        int[] positions;
        static bool spiders = false;
        bool inGame = false;


        List<Image> pics = new List<Image>();   
    
        public List<Image> theHardChoice() 
        {
            List<Image> SpiderPics = new List<Image>()
            {
                Properties.Resources.sa, Properties.Resources.sb, Properties.Resources.sc, Properties.Resources.sd, Properties.Resources.se, Properties.Resources.sf,
                Properties.Resources.sg, Properties.Resources.sh, Properties.Resources.si, Properties.Resources.sj, Properties.Resources.sk, Properties.Resources.sl,
                Properties.Resources.sm, Properties.Resources.sn, Properties.Resources.so, Properties.Resources.sp, Properties.Resources.sq, Properties.Resources.sr,
                Properties.Resources.sa, Properties.Resources.sb, Properties.Resources.sc, Properties.Resources.sd, Properties.Resources.se, Properties.Resources.sf,
                Properties.Resources.sg, Properties.Resources.sh, Properties.Resources.si, Properties.Resources.sj, Properties.Resources.sk, Properties.Resources.sl,
                Properties.Resources.sm, Properties.Resources.sn, Properties.Resources.so, Properties.Resources.sp, Properties.Resources.sq, Properties.Resources.sr
            };

            List<Image> CityPics = new List<Image>()
            {
                Properties.Resources.a, Properties.Resources.b, Properties.Resources.c, Properties.Resources.d, Properties.Resources.e, Properties.Resources.f,
                Properties.Resources.g, Properties.Resources.h, Properties.Resources.i, Properties.Resources.j, Properties.Resources.k, Properties.Resources.l,
                Properties.Resources.m, Properties.Resources.n, Properties.Resources.o, Properties.Resources.p, Properties.Resources.q, Properties.Resources.r,
                Properties.Resources.a, Properties.Resources.b, Properties.Resources.c, Properties.Resources.d, Properties.Resources.e, Properties.Resources.f,
                Properties.Resources.g, Properties.Resources.h, Properties.Resources.i, Properties.Resources.j, Properties.Resources.k, Properties.Resources.l,
                Properties.Resources.m, Properties.Resources.n, Properties.Resources.o, Properties.Resources.p, Properties.Resources.q, Properties.Resources.r
            };

            if (spiders == true) return SpiderPics;
            else return CityPics;
          
        }

        PictureBox first, second;

        public Pexeso()
        {
            InitializeComponent();
            HideAll();
            
        }

        public void GetScore() //maybe overcomplicating it rather than simplifying...
        {
            scoreLabel.Text = score.ToString();            
        }
       
        public int[] GetBoxes() //ultimately useless function (no indexes are extra)
        {
            int[] boxList = new int[36];
            int j = 0;
            for (int i = 0; i < tableLayoutPanel1.Controls.Count; i++)
            {
                if (tableLayoutPanel1.Controls[i] is PictureBox) boxList[j] = i;
                j++;
            }
            return boxList;
        }

        public int[] Assign() //Only assignes, doesn't reveal.
        {
            List<int> picIndex = new List<int>();   //separate list of indexes for the pics list
            for (int index = 0; index<36; index++)  //it should go 0,1,2,...,35
                picIndex.Add(index); 
            int i = 35;
            int[] assignment = new int[36];     //this array should remember randomly picked indexes
            PictureBox box;
            int rand;
          int[] boxPos = GetBoxes();
            for (int num = 0; num <36; num++)
            {
                box = (PictureBox)tableLayoutPanel1.Controls[boxPos[num]];  //it gets assigned from PictureBox36 down
                rand = random.Next(0, picIndex.Count);
                assignment[i] = picIndex[rand];     //i=35 contains index of a picture assigned to Box36
                picIndex.RemoveAt(rand);
                i--;
            }
            return assignment;
        }

       

        

        public void RevealAll()
        {
            PictureBox box;
            int num;
            for (int i = 0; i < tableLayoutPanel1.Controls.Count; i++)
            {
                if (tableLayoutPanel1.Controls[i] is PictureBox)
                    box = (PictureBox)tableLayoutPanel1.Controls[i];
                else
                    continue;
                num = 36 - boxIndex(box.Name);
                box.Image = pics[positions[num]];
            }
        }

        private void Hide(PictureBox box) //hides a specific box
        {
            box.Image = blind;
        }

        private void HideAll()
        {
            PictureBox box;
            for (int i = 0; i < tableLayoutPanel1.Controls.Count; i++)
            {
                if (tableLayoutPanel1.Controls[i] is PictureBox)
                    box = (PictureBox)tableLayoutPanel1.Controls[i];
                else
                    continue;
                box.Image = blind;

            }
        }

        public int boxIndex(string name)
        {
            string text = "";
            for (int i = 10; i < name.Length; i++)
            {
                text += name[i];
            }
            return Int32.Parse(text);
        }

        private void SpiderBox_CheckedChanged(object sender, EventArgs e)
        {
            if (spiders == false) spiders = true;
            else spiders = false;
        }

        private async void clickBox(object sender, EventArgs e)
        {

            if (first != null && second != null)
                return;
            int i;
            PictureBox clickedBox = sender as PictureBox;
            if (inGame == false)
                return;
            if (clickedBox == null)
                return;
            if (clickedBox.Image != blind)
                return;
            if (first == null)
            {
                first = clickedBox;
                i = 36 - boxIndex(first.Name);
                p1 = positions[i];
                first.Image = pics[p1];
               
                return;
            }else if (second == null)
            {
                second = clickedBox;
                i = 36 - boxIndex(second.Name);
                p2 = positions[i];
                second.Image = pics[p2];

                if ((p1 == p2 + 18) || (p1 == p2 - 18))
                {
                    score++;
                    counter++;
                }
                else
                {
                    score--;
                    await Task.Delay(1000);
                    Hide(first);
                    Hide(second);
                }
            }
            GetScore();
            if (counter == 18)
            {
                infoLabel.Text = "You've found them all!";
                infoLabel.ForeColor = Color.Red;
            }
            p1 = 0;
            p2 = 0;
            first = null;
            second = null;
            return;

        }

        private async void NGbutton_Click(object sender, EventArgs e)
        {
            pics = theHardChoice();
            first = null;
            score = 0;
            GetScore();
            
            positions = Assign();
            RevealAll();
            infoLabel.Text = "Look carefully!";
            infoLabel.ForeColor = Color.Red;
            for (int i = 9; i > 0; i--)
            {
                await Task.Delay(1000);
                infoLabel.Text = i.ToString();
            }
            await Task.Delay(1000);
            HideAll();
            infoLabel.Text = "Play!";
            infoLabel.ForeColor = Color.GreenYellow;
            inGame = true;
        }

    }
}
