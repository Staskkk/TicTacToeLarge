using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tic_tac_toe.Models;
using Tic_tac_toe.Services;

namespace Tic_tac_toe.Helpers
{
    public static class HeuristicManager
    {
        private enum RowOpenClose
        {
            Unknown,
            Open,
            Close
        }

        public static int Heuristic(RootState rootState, byte goodMark, byte badMark)
        {
            int globalSum = 0;
            for (int i = 0; i < GameManager.FieldSize; ++i)
            {
                for (int j = 0; j < GameManager.FieldSize; ++j)
                {
                    if (rootState[i, j] == goodMark)
                    {
                        globalSum += SingleHeuristic(rootState, i, j, goodMark);
                    }
                    else if (rootState[i, j] == badMark)
                    {
                        globalSum -= SingleHeuristic(rootState, i, j, badMark);
                    }
                }
            }

            MainWindow.GlobalCells.State = rootState;
            Thread.Sleep(1000);

            ////for (int i = 0; i < GameManager.FieldSize; ++i)
            ////{
            ////    for (int j = 0; j < GameManager.FieldSize; ++j)
            ////    {
            ////        Console.Write(rootState[i, j] + " ");
            ////    }

            ////    Console.WriteLine();
            ////}

            ////Console.WriteLine(globalSum);
            ////Console.WriteLine();

            return globalSum;
        }

        private static int SingleHeuristic(RootState rootState, int i0, int j0, byte goodMark)
        {
            int[] sums = new int[4] { 1, 1, 1, 1 };
            RowOpenClose[] ends = new RowOpenClose[8];
            for (int i = 1; i < 6; ++i)
            {
                CheckCellInRow(rootState, goodMark, i0 + i, j0, 0, 0, sums, ends);
                CheckCellInRow(rootState, goodMark, i0 - i, j0, 0, 1, sums, ends);
                CheckCellInRow(rootState, goodMark, i0, j0 + i, 1, 2, sums, ends);
                CheckCellInRow(rootState, goodMark, i0, j0 - i, 1, 3, sums, ends);
                CheckCellInRow(rootState, goodMark, i0 + i, j0 + i, 2, 4, sums, ends);
                CheckCellInRow(rootState, goodMark, i0 - i, j0 - i, 2, 5, sums, ends);
                CheckCellInRow(rootState, goodMark, i0 + i, j0 - i, 3, 6, sums, ends);
                CheckCellInRow(rootState, goodMark, i0 - i, j0 + i, 3, 7, sums, ends);
            }

            int globalSum = 0;
            for (int i = 0; i < sums.Length; ++i)
            {
                if (sums[i] >= 5)
                {
                    return int.MaxValue;
                }

                globalSum += sums[i] * GetWeightedSum(sums[i], ends[i * 2], ends[(i * 2) + 1]);
            }

            return globalSum;
        }

        private static int GetWeightedSum(int sum, RowOpenClose end1, RowOpenClose end2)
        {
            if (end1 == RowOpenClose.Close && end2 == RowOpenClose.Close)
            {
                return sum;
            }
            else if (end1 == RowOpenClose.Close || end2 == RowOpenClose.Close)
            {
                return sum * 2;
            }
            else
            {
                return sum * 3;
            }
        }

        private static void CheckCellInRow(RootState rootState, int goodMark, int i, int j, int sumI, int endI, int[] sums, RowOpenClose[] ends)
        {
            if (ends[endI] == RowOpenClose.Unknown && rootState[i, j] == goodMark)
            {
                sums[sumI]++;
            }
            else
            {
                if (rootState[i, j] == GameManager.EmptyMark)
                {
                    ends[sumI] = RowOpenClose.Open;
                }
                else
                {
                    ends[sumI] = RowOpenClose.Close;
                }
            }
        }
    }
}
