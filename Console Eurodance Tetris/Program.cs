using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMPLib;
using System.Diagnostics;

namespace ConsoleEurodanceTetris
{
    class Program
    {
        public static string square = "■";
        public static int[,] grid = new int[23, 10];
        public static int[,] droppedtetrominoeLocationGrid = new int[23, 10];
        public static Stopwatch timer = new Stopwatch();
        public static Stopwatch dropTimer = new Stopwatch();
        public static Stopwatch inputTimer = new Stopwatch();
        public static int dropTime, dropRate = 300;
        public static bool isDropped = false;
        static Gameplay tet;
        static Gameplay nexttet;
        public static ConsoleKeyInfo key;
        public static bool isKeyPressed = false;
        public static int linesCleared = 0, score = 0, level = 1;

        static void Main()
        {
            ConsoleProperties();
            DrawBorder();
            AuthorDetails();
            ControlsDescription();

            // Music playing
            WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer();
            string musicPath = System.Environment.CurrentDirectory;
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(@"Music");
            System.IO.FileInfo[] files = dir.GetFiles();
            WMPLib.IWMPPlaylist playlist = wplayer.playlistCollection.newPlaylist("myplaylist");
            foreach (System.IO.FileInfo file in files)
            {
                WMPLib.IWMPMedia media;
                media = wplayer.newMedia(file.FullName);
                playlist.appendItem(media);
            }
            wplayer.currentPlaylist = playlist;
            wplayer.settings.setMode("shuffle", true);
            wplayer.controls.play();

            timer.Start();
            dropTimer.Start();
            long time = timer.ElapsedMilliseconds;
            Console.SetCursorPosition(26, 0);
            Console.WriteLine("Level - " + level);
            Console.SetCursorPosition(26, 1);
            Console.WriteLine("Score - " + score);
            Console.SetCursorPosition(26, 2);
            Console.WriteLine("Lines cleared - " + linesCleared);
            nexttet = new Gameplay();
            tet = nexttet;
            tet.Spawn();
            nexttet = new Gameplay();
            Update();
            GameOver();
        }

        private static void ControlsDescription()
        {
            Console.SetCursorPosition(26, 10);
            Console.WriteLine("LeftArrow - Move left");
            Console.SetCursorPosition(26, 11);
            Console.WriteLine("RightArrow - Move right");
            Console.SetCursorPosition(26, 12);
            Console.WriteLine("UpArrow - Rotate");
            Console.SetCursorPosition(26, 13);
            Console.WriteLine("DownArrow - Drop");
            Console.SetCursorPosition(26, 14);
            Console.WriteLine("Spacebar - Fast drop");
            Console.SetCursorPosition(26, 15);
            Console.WriteLine("P - Pause");
        }

        private static void GameOver()
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Game over!\n\nPress ENTER for new game\nor ESC for exit");
            ConsoleKeyInfo cki;
            cki = Console.ReadKey(true);

            if (cki.Key == ConsoleKey.Enter)
            {
                int[,] grid = new int[23, 10];
                droppedtetrominoeLocationGrid = new int[23, 10];
                timer = new Stopwatch();
                dropTimer = new Stopwatch();
                inputTimer = new Stopwatch();
                dropRate = 300;
                isDropped = false;
                isKeyPressed = false;
                linesCleared = 0;
                score = 0;
                level = 1;
                GC.Collect();
                Console.Clear();
                Program.Main();
            }
            else if (cki.Key == ConsoleKey.Escape)
            {
                return;
            }
            else
            {
                Console.WriteLine("\nWrong key pressed!");
                GameOver();
            }
        }

        private static void AuthorDetails()
        {
            Console.SetCursorPosition(0, 25);
            Console.WriteLine("Author: Edo Komljenovic (edo.komljenovic@gmail.com)\nParts of the code: Kim Albireo (N/A)");
        }

        public static void DrawBorder()
        {
            // Left border
            for (int lengthCount = 0; lengthCount <= 22; ++lengthCount)
            {
                Console.SetCursorPosition(0, lengthCount);
                Console.Write("|");
            }

            // Right border
            for (int lengthCount = 0; lengthCount <= 22; ++lengthCount)
            {
                Console.SetCursorPosition(21, lengthCount);
                Console.Write("|");
            }

            // Bottom border
            for (int widthCount = 0; widthCount <= 21; widthCount++)
            {
                Console.SetCursorPosition(widthCount, 23);
                Console.Write("-");
            }
        }

        private static void ConsoleProperties()
        {
            Console.Title = "Console Eurodance Tetris";
            Console.CursorVisible = false;
            Console.BackgroundColor = System.ConsoleColor.DarkBlue;
            Console.Clear();
            Console.ForegroundColor = System.ConsoleColor.Yellow;
        }

        private static void Update()
        {
            while (true)//Update Loop
            {
                dropTime = (int)dropTimer.ElapsedMilliseconds;
                if (dropTime > dropRate)
                {
                    dropTime = 0;
                    dropTimer.Restart();
                    tet.Drop();
                }
                if (isDropped == true)
                {
                    tet = nexttet;
                    nexttet = new Gameplay();
                    tet.Spawn();

                    isDropped = false;
                }
                int j;
                for (j = 0; j < 10; j++)
                {
                    if (droppedtetrominoeLocationGrid[0, j] == 1)
                        return;
                }

                Input();
                ClearBlock();

            } //end Update
        }

        private static void FillGrid()
        {
            for (int i = 0; i < 23; ++i)
            {
                Console.SetCursorPosition(1, i);
                for (int j = 0; j < 10; j++)
                {
                    Console.Write(square);
                }
                Console.WriteLine();
            }
        }

        private static void ClearBlock()
        {
            int combo = 0;
            for (int i = 0; i < 23; i++)
            {
                int j;
                for (j = 0; j < 10; j++)
                {
                    if (droppedtetrominoeLocationGrid[i, j] == 0)
                        break;
                }
                if (j == 10)
                {
                    linesCleared++;
                    combo++;
                    for (j = 0; j < 10; j++)
                    {
                        droppedtetrominoeLocationGrid[i, j] = 0;
                    }
                    int[,] newdroppedtetrominoeLocationGrid = new int[23, 10];
                    for (int k = 1; k < i; k++)
                    {
                        for (int l = 0; l < 10; l++)
                        {
                            newdroppedtetrominoeLocationGrid[k + 1, l] = droppedtetrominoeLocationGrid[k, l];
                        }
                    }
                    for (int k = 1; k < i; k++)
                    {
                        for (int l = 0; l < 10; l++)
                        {
                            droppedtetrominoeLocationGrid[k, l] = 0;
                        }
                    }
                    for (int k = 0; k < 23; k++)
                        for (int l = 0; l < 10; l++)
                            if (newdroppedtetrominoeLocationGrid[k, l] == 1)
                                droppedtetrominoeLocationGrid[k, l] = 1;
                    Draw();
                }
            }
            if (combo == 1)
                score += 40 * level;
            else if (combo == 2)
                score += 100 * level;
            else if (combo == 3)
                score += 300 * level;
            else if (combo > 3)
                score += 300 * combo * level;

            if (linesCleared < 25) level = 1;
            else if (linesCleared < 50) level = 2;
            else if (linesCleared < 75) level = 3;
            else if (linesCleared < 100) level = 4;
            else if (linesCleared < 125) level = 5;
            else if (linesCleared < 150) level = 6;
            else if (linesCleared < 175) level = 7;
            else if (linesCleared < 200) level = 8;
            else if (linesCleared < 225) level = 9;
            else if (linesCleared < 250) level = 10;

            if (combo > 0)
            {
                Console.SetCursorPosition(26, 0);
                Console.WriteLine("Level - " + level);
                Console.SetCursorPosition(26, 1);
                Console.WriteLine("Score - " + score);
                Console.SetCursorPosition(26, 2);
                Console.WriteLine("Lines cleared - " + linesCleared);
            }

            dropRate = 300 - 22 * level;

        }

        private static void Input()
        {
            if (Console.KeyAvailable)
            {
                key = Console.ReadKey();
                isKeyPressed = true;
            }
            else
                isKeyPressed = false;

            if (Program.key.Key == ConsoleKey.LeftArrow & !tet.IsSomethingLeft() & isKeyPressed)
            {
                for (int i = 0; i < 4; i++)
                {
                    tet.location[i][1] -= 1;
                }
                tet.Update();
                //    Console.Beep();
            }
            else if (Program.key.Key == ConsoleKey.RightArrow & !tet.IsSomethingRight() & isKeyPressed)
            {
                for (int i = 0; i < 4; i++)
                {
                    tet.location[i][1] += 1;
                }
                tet.Update();
            }

            if (Program.key.Key == ConsoleKey.DownArrow & isKeyPressed)
            {
                tet.Drop();
            }

            if (Program.key.Key == ConsoleKey.Spacebar & isKeyPressed)
            {
                for (; tet.IsSomethingBelow() != true;)
                {
                    tet.Drop();
                }
            }

            if (Program.key.Key == ConsoleKey.UpArrow & isKeyPressed)
            {
                //rotate
                tet.Rotate();
                tet.Update();
            }

            if (Program.key.Key == ConsoleKey.P & isKeyPressed)
            {
                Pause();
            }
        }

        private static void Pause()
        {
            Console.SetCursorPosition(0, 28);
            var pauseProc = Process.Start(new ProcessStartInfo()
            {
                FileName = "cmd",
                Arguments = "/C pause",
                UseShellExecute = false
            });
            pauseProc.WaitForExit();
        }

        public static void Draw()
        {
            for (int i = 0; i < 23; ++i)
            {
                for (int j = 0; j < 10; j++)
                {
                    Console.SetCursorPosition(1 + 2 * j, i);
                    if (grid[i, j] == 1 | droppedtetrominoeLocationGrid[i, j] == 1)
                    {
                        Console.SetCursorPosition(1 + 2 * j, i);
                        Console.Write(square);
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                }
            }
        }
    }

    public class Gameplay
    {
        public static int[,] I = new int[1, 4] { { 1, 1, 1, 1 } };//3
        public static int[,] O = new int[2, 2] { { 1, 1 }, { 1, 1 } };
        public static int[,] T = new int[2, 3] { { 0, 1, 0 }, { 1, 1, 1 } };//3
        public static int[,] S = new int[2, 3] { { 0, 1, 1 }, { 1, 1, 0 } };//4
        public static int[,] Z = new int[2, 3] { { 1, 1, 0 }, { 0, 1, 1 } };//3
        public static int[,] J = new int[2, 3] { { 1, 0, 0 }, { 1, 1, 1 } };//3
        public static int[,] L = new int[2, 3] { { 0, 0, 1 }, { 1, 1, 1 } };//3
        public static List<int[,]> tetrominoes = new List<int[,]>() { I, O, T, S, Z, J, L };

        private bool isErect = false;
        private int[,] shape;
        private int[] pix = new int[2];
        public List<int[]> location = new List<int[]>();

        public Gameplay()
        {
            Random rnd = new Random();
            shape = tetrominoes[rnd.Next(0, 7)];
            for (int i = 23; i < 33; ++i)
            {
                for (int j = 3; j < 10; j++)
                {
                    Console.SetCursorPosition(i, j);
                    Console.Write("  ");
                }
            }

            Program.DrawBorder();

            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    if (shape[i, j] == 1)
                    {
                        Console.SetCursorPosition(((10 - shape.GetLength(1)) / 2 + j) * 2 + 20, i + 5);
                        Console.Write(Program.square);
                    }
                }
            }
        }

        public void Spawn()
        {
            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    if (shape[i, j] == 1)
                    {
                        location.Add(new int[] { i, (10 - shape.GetLength(1)) / 2 + j });
                    }
                }
            }
            Update();
        }

        public void Drop()
        {
            if (IsSomethingBelow())
            {
                for (int i = 0; i < 4; i++)
                {
                    Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] = 1;
                }
                Program.isDropped = true;
            }
            else
            {
                for (int numCount = 0; numCount < 4; numCount++)
                {
                    location[numCount][0] += 1;
                }

                Update();

            }
        }

        public void Rotate()
        {
            List<int[]> templocation = new List<int[]>();
            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    if (shape[i, j] == 1)
                    {
                        templocation.Add(new int[] { i, (10 - shape.GetLength(1)) / 2 + j });
                    }
                }
            }

            if (shape == tetrominoes[0])
            {
                if (isErect == false)
                {
                    for (int i = 0; i < location.Count; i++)
                    {
                        templocation[i] = TransformMatrix(location[i], location[2], "Clockwise");
                    }
                }
                else
                {
                    for (int i = 0; i < location.Count; i++)
                    {
                        templocation[i] = TransformMatrix(location[i], location[2], "Counterclockwise");
                    }
                }
            }
            else if (shape == tetrominoes[3])
            {
                for (int i = 0; i < location.Count; i++)
                {
                    templocation[i] = TransformMatrix(location[i], location[3], "Clockwise");
                }
            }
            else if (shape == tetrominoes[1]) return;
            else
            {
                for (int i = 0; i < location.Count; i++)
                {
                    templocation[i] = TransformMatrix(location[i], location[2], "Clockwise");
                }
            }

            for (int count = 0; IsOverlayLeft(templocation) != false | IsOverlayRight(templocation) != false | IsOverlayBelow(templocation) != false; count++)
            {
                if (IsOverlayLeft(templocation) == true)
                {
                    for (int i = 0; i < location.Count; i++)
                    {
                        templocation[i][1] += 1;
                    }
                }

                if (IsOverlayRight(templocation) == true)
                {
                    for (int i = 0; i < location.Count; i++)
                    {
                        templocation[i][1] -= 1;
                    }
                }

                if (IsOverlayBelow(templocation) == true)
                {
                    for (int i = 0; i < location.Count; i++)
                    {
                        templocation[i][0] -= 1;
                    }
                }

                if (count == 3)
                {
                    return;
                }
            }

            location = templocation;

        }

        public int[] TransformMatrix(int[] coord, int[] axis, string dir)
        {
            int[] pcoord = { coord[0] - axis[0], coord[1] - axis[1] };
            if (dir == "Counterclockwise")
            {
                pcoord = new int[] { -pcoord[1], pcoord[0] };
            }
            else if (dir == "Clockwise")
            {
                pcoord = new int[] { pcoord[1], -pcoord[0] };
            }
            return new int[] { pcoord[0] + axis[0], pcoord[1] + axis[1] };
        }

        public bool IsSomethingBelow()
        {
            for (int i = 0; i < 4; i++)
            {
                if (location[i][0] + 1 >= 23)
                    return true;
                if (location[i][0] + 1 < 23)
                {
                    if (Program.droppedtetrominoeLocationGrid[location[i][0] + 1, location[i][1]] == 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool? IsOverlayBelow(List<int[]> location)
        {
            List<int> ycoords = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                ycoords.Add(location[i][0]);
                if (location[i][0] >= 23)
                    return true;
                if (location[i][0] < 0)
                    return null;
                if (location[i][1] < 0)
                {
                    return null;
                }
                if (location[i][1] > 9)
                {
                    return null;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                if (ycoords.Max() - ycoords.Min() == 3)
                {
                    if (ycoords.Max() == location[i][0] | ycoords.Max() - 1 == location[i][0])
                    {
                        if (Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            return true;
                        }
                    }

                }
                else
                {
                    if (ycoords.Max() == location[i][0])
                    {
                        if (Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool IsSomethingLeft()
        {
            for (int i = 0; i < 4; i++)
            {
                if (location[i][1] == 0)
                {
                    return true;
                }
                else if (Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1] - 1] == 1)
                {
                    return true;
                }
            }
            return false;
        }

        public bool? IsOverlayLeft(List<int[]> location)
        {
            List<int> xcoords = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                xcoords.Add(location[i][1]);
                if (location[i][1] < 0)
                {
                    return true;
                }
                if (location[i][1] > 9)
                {
                    return false;
                }
                if (location[i][0] >= 23)
                    return null;
                if (location[i][0] < 0)
                    return null;
            }
            for (int i = 0; i < 4; i++)
            {
                if (xcoords.Max() - xcoords.Min() == 3)
                {
                    if (xcoords.Min() == location[i][1] | xcoords.Min() + 1 == location[i][1])
                    {
                        if (Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    if (xcoords.Min() == location[i][1])
                    {
                        if (Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool IsSomethingRight()
        {
            for (int i = 0; i < 4; i++)
            {
                if (location[i][1] == 9)
                {
                    return true;
                }
                else if (Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1] + 1] == 1)
                {
                    return true;
                }
            }
            return false;
        }

        public bool? IsOverlayRight(List<int[]> location)
        {
            List<int> xcoords = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                xcoords.Add(location[i][1]);
                if (location[i][1] > 9)
                {
                    return true;
                }
                if (location[i][1] < 0)
                {
                    return false;
                }
                if (location[i][0] >= 23)
                    return null;
                if (location[i][0] < 0)
                    return null;
            }
            for (int i = 0; i < 4; i++)
            {
                if (xcoords.Max() - xcoords.Min() == 3)
                {
                    if (xcoords.Max() == location[i][1] | xcoords.Max() - 1 == location[i][1])
                    {
                        if (Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    if (xcoords.Max() == location[i][1])
                    {
                        if (Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void Update()
        {
            for (int i = 0; i < 23; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Program.grid[i, j] = 0;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                Program.grid[location[i][0], location[i][1]] = 1;
            }

            Program.Draw();

        }
    }
}