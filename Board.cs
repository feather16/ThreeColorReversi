using System;

public class Board
{
    public const int SIZE = 9;
    public const int WIDTH = 46;
    public const int EMPTY = 0;
    public const int RED = 1;
    public const int YELLOW = 2;
    public const int BLUE = 3;
    public const int WALL = 4;
    int[] dx = new int[] { 0, 0, 1, -1, 1, 1, -1, -1 };
    int[] dy = new int[] { 1, -1, 0, 0, 1, -1, 1, -1 };

    public const int DEF = 20;
    public const int MAX = 100;
    public int[,,] value = new int[2, Board.SIZE + 2, Board.SIZE + 2]
    {
            {
                {DEF,DEF,DEF,DEF,DEF,DEF,DEF,DEF,DEF,DEF,DEF},
                {DEF,MAX, -2,  2,  2,  2,  2,  2, -2,MAX,DEF},
                {DEF, -2, -2, -1, -1, -1, -1, -1, -2, -2,DEF},
                {DEF,  2, -1,  3,  3,  3,  3,  3, -1,  2,DEF},
                {DEF,  2, -1,  3,  3,  3,  3,  3, -1,  2,DEF},
                {DEF,  2, -1,  3,  3,  3,  3,  3, -1,  2,DEF},
                {DEF,  2, -1,  3,  3,  3,  3,  3, -1,  2,DEF},
                {DEF,  2, -1,  3,  3,  3,  3,  3, -1,  2,DEF},
                {DEF, -2, -2, -1, -1, -1, -1, -1, -2, -2,DEF},
                {DEF,MAX, -2,  2,  2,  2,  2,  2, -2,MAX,DEF},
                {DEF,DEF,DEF,DEF,DEF,DEF,DEF,DEF,DEF,DEF,DEF},
            },
            {
                {DEF,DEF,DEF,DEF,DEF,DEF,DEF,DEF,DEF,DEF,DEF},
                {DEF,MAX, -2,  2,  2,  2,  2,  2, -2,MAX,DEF},
                {DEF, -2, -2, -1, -1, -1, -1, -1, -2, -2,DEF},
                {DEF,  2, -1,  3,  3,  3,  3,  3, -1,  2,DEF},
                {DEF,  2, -1,  3,  3,  3,  3,  3, -1,  2,DEF},
                {DEF,  2, -1,  3,  3,  3,  3,  3, -1,  2,DEF},
                {DEF,  2, -1,  3,  3,  3,  3,  3, -1,  2,DEF},
                {DEF,  2, -1,  3,  3,  3,  3,  3, -1,  2,DEF},
                {DEF, -2, -2, -1, -1, -1, -1, -1, -2, -2,DEF},
                {DEF,MAX, -2,  2,  2,  2,  2,  2, -2,MAX,DEF},
                {DEF,DEF,DEF,DEF,DEF,DEF,DEF,DEF,DEF,DEF,DEF},
            }
    };
    public bool[,,] def;

    public int[,] color;
    public Board()
    {
        color = new int[SIZE + 2, SIZE + 2];
        color[SIZE / 2, SIZE / 2] = RED;
        color[SIZE / 2 + 1, SIZE / 2 + 1] = RED;
        color[SIZE / 2 + 2, SIZE / 2 + 2] = RED;
        color[SIZE / 2, SIZE / 2 + 1] = YELLOW;
        color[SIZE / 2 + 1, SIZE / 2 + 2] = YELLOW;
        color[SIZE / 2 + 2, SIZE / 2] = YELLOW;
        color[SIZE / 2, SIZE / 2 + 2] = BLUE;
        color[SIZE / 2 + 1, SIZE / 2] = BLUE;
        color[SIZE / 2 + 2, SIZE / 2 + 1] = BLUE;

        def = new bool[2, Board.SIZE + 2, Board.SIZE + 2];

        for (int i = 0; i <= SIZE + 1; i++)
        {
            for (int j = 0; j <= SIZE + 1; j++)
            {
                def[0, i, j] = def[1, i, j] = false;
                if (i == 0 || i == SIZE + 1 || j == 0 || j == SIZE + 1)
                {
                    color[i, j] = WALL;
                    def[0, i, j] = def[1, i, j] = true;
                }
            }
        }

    }

    public int StoneNumber(int stonecolor)
    {
        int ret = 0;
        for (int i = 1; i <= SIZE; i++)
        {
            for (int j = 1; j <= SIZE; j++)
            {
                if (color[i, j] == stonecolor) ret++;
            }
        }
        return ret;
    }

    public void Copyto(Board b)
    {
        for (int i = 1; i <= SIZE; i++)
        {
            for (int j = 1; j <= SIZE; j++)
            {
                b.color[i, j] = this.color[i, j];
                b.value[0, i, j] = this.value[0, i, j];
                b.value[1, i, j] = this.value[1, i, j];
                b.def[0, 1, j] = this.def[0, i, j];
                b.def[1, i, j] = this.def[1, i, j];
            }
        }
    }

    public void Put(int stonecolor, int x, int y)
    {
        //Console.WriteLine(x + "," + y+", color="+color[x,y]+":");
        if (color[x, y] != EMPTY) return;
        bool turned = false;
        for (int i = 0; i < 8; i++)
        {
            int step = 1;
            while (true)
            {
                if (color[x + step * dx[i], y + step * dy[i]] == stonecolor) break;
                else if (color[x + step * dx[i], y + step * dy[i]] == EMPTY || color[x + step * dx[i], y + step * dy[i]] == WALL)
                {
                    step = 1;
                    break;
                }
                else step++;
            }
            if (step > 1) turned = true;
            //Console.Write(step + ",");
            for (int j = 1; j < step; j++) color[x + j * dx[i], y + j * dy[i]] = stonecolor;
        }
        if (turned) color[x, y] = stonecolor;
        // Console.WriteLine("\n");
    }

    public bool isDef(int Player, int x, int y)
    {
        if (color[x, y] != Player) return false;
        return (
            (def[Player - 2, x - 1, y] || def[Player - 2, x + 1, y]) &&
            (def[Player - 2, x, y - 1] || def[Player - 2, x, y + 1]) &&
            (def[Player - 2, x - 1, y - 1] || def[Player - 2, x + 1, y + 1]) &&
            (def[Player - 2, x + 1, y - 1] || def[Player - 2, x - 1, y + 1])
        );
    }

    public bool willbeDef(int Player, int x, int y)
    {
        if (color[x, y] == Player || def[Player - 2, x, y]) return false;
        return (
            (def[Player - 2, x - 1, y] || def[Player - 2, x + 1, y]) &&
            (def[Player - 2, x, y - 1] || def[Player - 2, x, y + 1]) &&
            (def[Player - 2, x - 1, y - 1] || def[Player - 2, x + 1, y + 1]) &&
            (def[Player - 2, x + 1, y - 1] || def[Player - 2, x - 1, y + 1])
        );
    }

    //石を置く前に複数回呼び出す
    public bool DefUpdate(int Player)
    {
        bool ret = false;

        for (int i = 1; i <= Board.SIZE; i++)
        {
            for (int j = 1; j <= Board.SIZE; j++)
            {
                if ((!def[Player - 2, i, j]) && isDef(Player, i, j))
                {
                    def[Player - 2, i, j] = true;
                    ret = true;
                }
            }
        }
        return ret;
    }

    //DefUpdateを十分行った後に呼び出す
    public void ValueUpdate(int Player)
    {
        for (int i = 1; i <= Board.SIZE; i++)
        {
            for (int j = 1; j <= Board.SIZE; j++)
            {
                if (willbeDef(Player, i, j))
                {
                    value[Player - 2, i, j] = Math.Max(DEF, value[Player - 2, i, j]);
                }
            }
        }
    }

}
