using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tic_tac_toe.Services;

namespace Tic_tac_toe.Helpers
{
    public static class PatternHashes
    {
        static PatternHashes()
        {
            byte x = GameManager.PlayerMark;
            Hor5 = GetPatternHash(new byte[,] { { x, x, x, x, x } }, 0, 1, 0, 5);
            Vert5 = GetPatternHash(new byte[,] { { x }, { x }, { x }, { x }, { x } }, 0, 5, 0, 1);
        }

        public static ulong Hor5 { get; }

        public static ulong Vert5 { get; }

        public static ulong DiagS5 { get; }

        public static ulong DiagI5 { get; }

        public static ulong GetPatternHash(byte[,] field, int startI, int endI, int startJ, int endJ)
        {
            ulong res = 0;
            ulong mult = 1;
            for (int i = startI; i < endI; ++i)
            {
                for (int j = startJ; j < endJ; ++j)
                {
                    res += field[i, j] * mult;
                    mult *= 4;
                }
            }

            return res;
        }
    }
}
