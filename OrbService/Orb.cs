using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbService
{
    public class Orb
    {
        public static int[,,] Table = new int[8, 5, 5]
        {
            {
                { 15, 3, 1, 6, 24 },
                { 16, 0, 3, 6, 27 },
                { 14, 5, 0, 6, 32 },
                { 15, 2, 2, 6, 36 },
                { 14, 4, 1, 6, 48 }
            },
            {
                { 14, 1, 3, 7, 54 },
                { 12, 6, 0, 7, 64 },
                { 13, 3, 2, 7, 72 },
                { 14, 0, 4, 7, 81 },
                { 12, 5, 1, 7, 96 }
            },
            {
                { 12, 2, 3, 8, 108 },
                { 10, 7, 0, 8, 128 },
                { 11, 4, 2, 8, 144 },
                { 12, 1, 4, 8, 162 },
                { 10, 6, 1, 8, 192 }
            },
            {
                { 11, 3, 3, 8, 216 },
                { 12, 0, 5, 8, 243 },
                { 7, 8, 0, 10, 256 },
                { 8, 5, 2, 10, 288 },
                { 9, 2, 4, 10, 324 }
            },
            {
                { 7, 7, 1, 10, 384 },
                { 8, 4, 3, 10, 432 },
                { 9, 1, 5, 10, 486 },
                { 6, 9, 0, 10, 512 },
                { 7, 6, 2, 10, 576 }
            },
            {
                { 8, 3, 4, 10, 648 },
                { 9, 0, 6, 10, 729 },
                { 6, 8, 1, 10, 768 },
                { 7, 5, 3, 10, 864 },
                { 8, 2, 5, 10, 972 }
            },
            {
                { 6, 7, 2, 10, 1152 },
                { 7, 4, 4, 10, 1296 },
                { 5, 1, 6, 13, 1458 },
                { 4, 9, 1, 13, 1536 },
                { 6, 6, 3, 10, 1728 }
            },
            {
                { 8, 0, 7, 10, 2187 },
                { 5, 8, 2, 10, 2304 },
                { 6, 5, 4, 10, 2592 },
                { 7, 2, 6, 10, 2916 },
                { 5, 7, 3, 10, 3456 }
            }
        };

        public int[,] SelectDifficulty(int diff)
        {
            int[,] multi = new int[5, 5];
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    multi[i, j] = Table[diff - 1, i, j];
                }
            }

            return SelectField(multi);
        }

        private int[,] SelectField(int[,] arr2d)
        {
            Random random = new();
            int r = random.Next(5);
            int[] arr = new int[5];

            for (int i = 0; i < 5; i++)
            {
                arr[i] = arr2d[r, i];
            }

            return EstablishField(arr);
        }

        private int[,] EstablishField(int[] arr)
        {
            int[,] ints = new int[5, 5];
            int selectedValue = -1;
            Random random = new Random();
            int countdown = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    int r = random.Next(24 - countdown);

                    if (r < arr[0] && arr[0] > 0)
                    {
                        selectedValue = 1;
                        arr[0] = arr[0] - 1;
                    }
                    else if (r < arr[0] + arr[1] && arr[1] > 0)
                    {
                        selectedValue = 2;
                        arr[1] = arr[1] - 1;
                    }
                    else if (r < arr[0] + arr[1] + arr[2] && arr[2] > 0)
                    {
                        selectedValue = 3;
                        arr[2] = arr[2] - 1;
                    }
                    else if (r < arr[0] + arr[1] + arr[2] + arr[3] && arr[3] > 0)
                    {
                        selectedValue = 0;
                        arr[3] = arr[3] - 1;
                    }
                    ints[i, j] = selectedValue;
                    countdown = countdown + 1;
                }
            }
            return ints;
        }
    }
}
