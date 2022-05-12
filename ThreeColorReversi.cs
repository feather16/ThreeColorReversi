using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThreeColorReversi
{
    public partial class ThreeColorReversi : Form
    {
        bool GameEnd = false;
        MersenneTwister MT = new MersenneTwister();
        
        int PassCounter = 0;
        Board MainBoard = new Board();
        Board SubBoard = new Board();
        public ThreeColorReversi()
        {
            InitializeComponent();
            Text = "Three Colors Reversi Ver.0.1";
            ClientSize = new Size(610, 500);
            BackColor = Color.White;
            Init();

            //ダブルバッファリング設定
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        public void Init()
        {
            setStoneNumbers(Board.RED);
            timer1.Start();
            //Console.WriteLine("Start!");
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.FillRectangle(Brushes.Green, 160, 10, Board.SIZE * Board.WIDTH, Board.SIZE * Board.WIDTH);

            for (int i = 1; i <= Board.SIZE + 1; i++)
            {
                e.Graphics.DrawLine(Pens.Black, (i - 1) * Board.WIDTH + 160, 10, (i - 1) * Board.WIDTH + 160, Board.WIDTH * Board.SIZE + 10);
                e.Graphics.DrawLine(Pens.Black, 160, (i - 1) * Board.WIDTH + 10, Board.WIDTH * Board.SIZE + 160, (i - 1) * Board.WIDTH + 10);
            }

            for (int i = 1; i <= Board.SIZE; i++)
            {
                for (int j = 1; j <= Board.SIZE; j++)
                {
                    DrawStone(e, MainBoard.color[i, j], (i - 1) * Board.WIDTH + 161, (j - 1) * Board.WIDTH + 11);
                }
            }

            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
        }

        //protected override void OnPaintBackground(PaintEventArgs pevent){}

        public void setStoneNumbers(int currentPlayer)
        {
            string Message;
            Message = "赤:" + MainBoard.StoneNumber(Board.RED);
            if (currentPlayer == Board.RED) Message += " ←";
            Message += "\n黄:" + MainBoard.StoneNumber(Board.YELLOW);
            if (currentPlayer == Board.YELLOW) Message += " ←";
            Message += "\n青:" + MainBoard.StoneNumber(Board.BLUE);
            if (currentPlayer == Board.BLUE) Message += " ←";
            this.label1.Text = Message;
        }

        private void DrawStone(PaintEventArgs e, int stonecolor, int x, int y)
        {
            if (stonecolor == Board.RED) e.Graphics.FillEllipse(Brushes.Red, x, y, Board.WIDTH - 2, Board.WIDTH - 2);
            else if (stonecolor == Board.YELLOW) e.Graphics.FillEllipse(Brushes.Yellow, x, y, Board.WIDTH - 2, Board.WIDTH - 2);
            else if (stonecolor == Board.BLUE) e.Graphics.FillEllipse(Brushes.Blue, x, y, Board.WIDTH - 2, Board.WIDTH - 2);
        }

        private int willGet(int Player, int x, int y)
        {
            MainBoard.Copyto(SubBoard);
            SubBoard.Put(Player, x, y);
            return SubBoard.StoneNumber(Player) - MainBoard.StoneNumber(Player);
        }

        private void CheckEnd()
        {
            int S = Board.SIZE * Board.SIZE - MainBoard.StoneNumber(Board.EMPTY);
            int R = MainBoard.StoneNumber(Board.RED);
            int Y = MainBoard.StoneNumber(Board.YELLOW);
            int B = MainBoard.StoneNumber(Board.BLUE);

            string Message;
            Message = "赤:" + R;
            Message += "\n黄:" + Y;
            Message += "\n青:" + B + "\n";

            if (S == Board.SIZE * Board.SIZE || PassCounter >= 3)
            {
                GameEnd=true;
                this.label2.Text = "";
                if (R >= Y && R >= B) Message += "You Won!";
                else Message += "You Lost...";
                this.label1.Text = Message;
            }
            else if (S == R)
            {
                GameEnd = true;
                this.label2.Text = "";
                Message += "You Won!";
                this.label1.Text = Message;
            }
            else if (S == Y || S == B)
            {
                GameEnd = true;
                this.label2.Text = "";
                Message += "You Lost...";
                this.label1.Text = Message;
            }
        }

        private int Positiontox()
        {
            System.Drawing.Point sp = System.Windows.Forms.Cursor.Position;
            System.Drawing.Point cp = this.PointToClient(sp);
            return (cp.X - 160) / Board.WIDTH + 1;
        }
        private int Positiontoy()
        {
            System.Drawing.Point sp = System.Windows.Forms.Cursor.Position;
            System.Drawing.Point cp = this.PointToClient(sp);
            return (cp.Y - 10) / Board.WIDTH + 1;
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (GameEnd) return;
            int x = Positiontox();
            int y = Positiontoy();

            string Message;
            if (x < 1 || Board.SIZE < x || y < 1 || Board.SIZE < y)
            {
                Message = "(?,?)  そこには置けません";
            }
            else
            {
                Message = "(" + x + "," + y + ")";
                if (willGet(Board.RED, x, y) > 0) Message += "  石を置く";
                else Message += "  そこには置けません";
            }
            this.label2.Text = Message;
        }

        private bool CanPutAnyStones(int Player)
        {
            for (int i = 1; i <= Board.SIZE; i++)
            {
                for (int j = 1; j <= Board.SIZE; j++)
                {
                    if (willGet(Player, i, j) > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (GameEnd) return;
            base.OnMouseDown(e);
            
            int x = Positiontox();
            int y = Positiontoy();
            if (x < 1 || Board.SIZE < x || y < 1 || Board.SIZE < y) return;
            int get = willGet(Board.RED, x, y);
            //Console.WriteLine("* * " + get + " * *");
            if (get == 0) return;
            this.label2.Text = "";
            MainBoard.Put(Board.RED, x, y);
            PassCounter = 0;
            setStoneNumbers(Board.RED);
            Refresh();
            CheckEnd();
            if (GameEnd) return;
            while (true)
            {
                Computer(Board.YELLOW);
                setStoneNumbers(Board.YELLOW);
                Refresh();
                CheckEnd();
                if (GameEnd) return;
                Computer(Board.BLUE);
                setStoneNumbers(Board.BLUE);
                Refresh();
                CheckEnd();
                if (GameEnd) return;
                if (CanPutAnyStones(Board.RED))
                {
                    setStoneNumbers(Board.RED);
                    return;
                }
                else
                {
                    PassCounter++;
                    CheckEnd();
                    if (GameEnd) return;
                }
            }
        }

        private void Computer(int Player)
        {
            if (!CanPutAnyStones(Player))
            {
                PassCounter++;
                return;
            }

            int[,] willGetValues = new int[Board.SIZE+2,Board.SIZE+2];
            int MaxValue = -100000;
            while (MainBoard.DefUpdate(Player)){}
            MainBoard.ValueUpdate(Player);
            
            /*
            Console.WriteLine(Player+":");
            for (int i = 1; i <= Board.SIZE; i++)
            {
                for (int j = 1; j <= Board.SIZE; j++)
                {
                   Console.Write(MainBoard.value[0,i,j]+",  ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            */

            for (int i = 1; i <= Board.SIZE; i++)
            {
                for (int j = 1; j <= Board.SIZE; j++)
                {
                    MainBoard.Copyto(SubBoard);
                    SubBoard.Put(Player, i, j);
                    if (SubBoard.StoneNumber(Player) - MainBoard.StoneNumber(Player) == 0) continue;

                    while (SubBoard.DefUpdate(Player)) { }
                    SubBoard.ValueUpdate(Player);

                    for (int k = 1; k <= Board.SIZE; k++)
                    {
                        for (int l = 1; l <= Board.SIZE; l++)
                        {
                            if (SubBoard.color[k, l] == Player && MainBoard.color[k, l] != Player)
                            {
                                willGetValues[i,j] += SubBoard.value[Player-2, k, l];
                            }
                        }
                    }
                    MaxValue = Math.Max(willGetValues[i, j],MaxValue);
                }
            }
            
            while (true)
            {
                int Randomx = (int)(MT.genrand_real2() * Board.SIZE + 1);
                int Randomy = (int)(MT.genrand_real2() * Board.SIZE + 1);
                if (willGet(Player,Randomx,Randomy)>0&&willGetValues[Randomx, Randomy] == MaxValue)
                {
                    Console.WriteLine(Player + ": (" + Randomx + "," + Randomy + "), value=" + MaxValue);
                    MainBoard.Put(Player, Randomx, Randomy);
                    PassCounter = 0;
                    return;
                }
            }
        }
    }
}