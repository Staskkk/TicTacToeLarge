using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tic_tac_toe.Models;

namespace Tic_tac_toe.Services
{
    public class GameManager
    {
        public const int FieldSize = 10;

        public const int FieldOffset = 5;

        public const int WallMark = 0;

        public const int EmptyMark = 1;

        public const int PlayerMark = 2;

        public const int ComputerMark = 3;

        ////private static Random rand = new Random();

        private RootState currentState = new RootState();

        private GameManager()
        {
        }

        public event EventHandler<RootState> StateChanged;

        public event EventHandler<string> GameOver;

        private enum RowOpenClose
        {
            Unknown,
            Open,
            Close
        }

        public static GameManager Instance { get; } = new GameManager();

        public bool GameStarted { get; private set; } = false;

        public bool PlayerTurn { get; private set; } = true;

        public int GlobalDepth { get; private set; } = 2;

        public RootState CurrentState
        {
            get
            {
                return this.currentState;
            }

            private set
            {
                this.currentState = value;
                this.StateChanged?.Invoke(this, this.currentState);
            }
        }

        public void Start(int depth, bool playerTurn)
        {
            if (this.GameStarted)
            {
                return;
            }

            this.GlobalDepth = depth;
            this.PlayerTurn = playerTurn;
            this.GameStarted = true;
        }

        public void Finish()
        {
            if (!this.GameStarted)
            {
                return;
            }

            this.CurrentState.SetEmpty();
            this.StateChanged?.Invoke(this, this.currentState);
            this.GameStarted = false;
        }

        public void PlayerMakeMove(Move move)
        {
            if (!this.GameStarted || !this.PlayerTurn)
            {
                return;
            }

            this.CurrentState[move.I, move.J] = PlayerMark;
            this.StateChanged?.Invoke(this, this.currentState);
            if (this.IsGoodWin(GameManager.PlayerMark))
            {
                this.GameOver?.Invoke(this, "GAME OVER: PLAYER WIN!");
                this.Finish();
                return;
            }

            this.PlayerTurn = false;
            this.ComputerMakeMove();
        }

        public void ComputerMakeMove()
        {
            if (!this.GameStarted || this.PlayerTurn)
            {
                return;
            }

            Move move = this.GetBestMove(GameManager.ComputerMark, GameManager.PlayerMark, this.GlobalDepth);
            if (move.I == -1 || move.J == -1)
            {
                this.GameOver?.Invoke(this, "GAME OVER: DRAW!");
                this.Finish();
                return;
            }

            this.CurrentState[move.I, move.J] = ComputerMark;
            this.StateChanged?.Invoke(this, this.currentState);
            if (this.IsGoodWin(GameManager.ComputerMark))
            {
                this.GameOver?.Invoke(this, "GAME OVER: COMPUTER WIN!");
                this.Finish();
                return;
            }

            if (this.IsAllOccupied())
            {
                this.GameOver?.Invoke(this, "GAME OVER: DRAW!");
                this.Finish();
                return;
            }

            this.PlayerTurn = true;
        }

        private static bool IsMinMaxFound(int depth, int newVal, int? oldVal)
        {
            if (oldVal == null)
            {
                return true;
            }

            return depth % 2 == 0 ? newVal < oldVal : newVal > oldVal;
        }

        private static bool IsBranchCut(int depth, int newVal, int? oldVal)
        {
            if (oldVal == null)
            {
                return false;
            }

            return depth % 2 == 0 ? newVal >= oldVal : newVal <= oldVal;
        }

        ////private static int Heuristic(IState state, byte goodMark, byte badMark)
        ////{
        ////    return rand.Next(-100, 101);
        ////}

        private bool IsAllOccupied()
        {
            for (int i = 0; i < FieldSize; ++i)
            {
                for (int j = 0; j < FieldSize; ++j)
                {
                    if (this.currentState[i, j] == EmptyMark)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void GetAllLayers(IState prevState, byte goodMark, byte badMark, int currDepth, int maxDepth, List<IState>[] res)
        {
            if (currDepth == maxDepth)
            {
                return;
            }

            if (this.IsGoodWin(goodMark) || this.IsGoodWin(badMark))
            {
                return;
            }

            var nextStates = this.GetAllNextStates(prevState, goodMark);
            if (res[currDepth + 1] == null)
            {
                res[currDepth + 1] = new List<IState>();
            }

            res[currDepth + 1].AddRange(nextStates);
            foreach (var state in nextStates)
            {
                byte bufferMark = this.currentState[state.Move.I, state.Move.J];
                this.currentState[state.Move.I, state.Move.J] = state.Move.MoveMark;
                this.GetAllLayers(state, badMark, goodMark, currDepth + 1, maxDepth, res);
                this.currentState[state.Move.I, state.Move.J] = bufferMark;
            }
        }

        private HashSet<IState> GetAllNextStates(IState prevState, byte goodMark)
        {
            HashSet<IState> res = new HashSet<IState>();
            for (int i = 0; i < FieldSize; ++i)
            {
                for (int j = 0; j < FieldSize; ++j)
                {
                    if (this.currentState[i, j] == EmptyMark)
                    {
                        bool isNear = false;
                        for (int ii = i - 2; ii < i + 3; ++ii)
                        {
                            for (int jj = j - 2; jj < j + 3; ++jj)
                            {
                                if (this.currentState[ii, jj] == PlayerMark || this.currentState[ii, jj] == ComputerMark)
                                {
                                    isNear = true;
                                    break;
                                }
                            }

                            if (isNear)
                            {
                                break;
                            }
                        }

                        if (isNear)
                        {
                            var newState = new State()
                            {
                                Move = new Move
                                {
                                    I = i,
                                    J = j,
                                    MoveMark = goodMark
                                },
                                Depth = prevState.Depth + 1,
                                Prev = prevState
                            };
                            res.Add(newState);
                        }
                    }
                }
            }

            if (res.Count == 0 && this.currentState[FieldSize / 2, FieldSize / 2] == EmptyMark)
            {
                var newState = new State()
                {
                    Move = new Move
                    {
                        I = FieldSize / 2,
                        J = FieldSize / 2,
                        MoveMark = goodMark
                    },
                    Depth = prevState.Depth + 1,
                    Prev = prevState
                };
                res.Add(newState);
            }

            prevState.NextStates = res;
            return res;
        }

        private Move GetBestMove(byte goodMark, byte badMark, int depth)
        {
            this.currentState.HeurValue = null;
            List<IState>[] layerStates = new List<IState>[depth + 1];
            layerStates[0] = new List<IState>() { this.currentState };
            this.GetAllLayers(this.currentState, goodMark, badMark, 0, depth, layerStates);
            while (layerStates[depth] == null || layerStates[depth].Count == 0)
            {
                depth--;
            }

            int currDepth = depth - 1;
            while (currDepth >= 0)
            {
                IState leftLayerState = null;
                if (layerStates[currDepth].Count > 0)
                {
                    leftLayerState = layerStates[currDepth][0];
                    IState prevLeftLayerState = leftLayerState;
                    while (prevLeftLayerState.Move != null)
                    {
                        this.currentState[prevLeftLayerState.Move.I, prevLeftLayerState.Move.J] = prevLeftLayerState.Move.MoveMark;
                        prevLeftLayerState = prevLeftLayerState.Prev;
                    }
                }

                List<IState> currLayerStates = layerStates[currDepth];
                int? currLayerHeurValue = null;
                foreach (var currLayerState in currLayerStates)
                {
                    var prevCurrLayerState = currLayerState;
                    List<Move> moves = new List<Move>(depth);
                    if (leftLayerState != null)
                    {
                        while (leftLayerState.Move != prevCurrLayerState.Move)
                        {
                            moves.Add(prevCurrLayerState.Move);
                            this.currentState[leftLayerState.Move.I, leftLayerState.Move.J] = EmptyMark;
                            prevCurrLayerState = prevCurrLayerState.Prev;
                            leftLayerState = leftLayerState.Prev;
                        }
                    }

                    leftLayerState = currLayerState;
                    foreach (var move in moves)
                    {
                        this.currentState[move.I, move.J] = move.MoveMark;
                    }

                    if (currLayerState.NextStates != null && currLayerState.NextStates.Count > 0)
                    {
                        foreach (var nextLayerState in currLayerState.NextStates)
                        {
                            this.currentState[nextLayerState.Move.I, nextLayerState.Move.J] = nextLayerState.Move.MoveMark;
                            nextLayerState.HeurValue = nextLayerState.HeurValue ?? this.Heuristic(goodMark, badMark);
                            this.currentState[nextLayerState.Move.I, nextLayerState.Move.J] = EmptyMark;

                            if (IsMinMaxFound(currDepth + 1, nextLayerState.HeurValue.Value, currLayerState.HeurValue))
                            {
                                currLayerState.HeurValue = nextLayerState.HeurValue;

                                if (IsBranchCut(currDepth, currLayerState.HeurValue.Value, currLayerHeurValue))
                                {
                                    currLayerState.Prev.NextStates.Remove(currLayerState);
                                    break;
                                }
                            }
                        }

                        if (IsMinMaxFound(currDepth, currLayerState.HeurValue.Value, currLayerHeurValue))
                        {
                            currLayerHeurValue = currLayerState.HeurValue;
                        }
                    }
                    else
                    {
                        currLayerState.HeurValue = this.Heuristic(goodMark, badMark);
                        if (IsMinMaxFound(currDepth, currLayerState.HeurValue.Value, currLayerHeurValue))
                        {
                            currLayerHeurValue = currLayerState.HeurValue;
                        }
                    }
                }

                while (leftLayerState?.Move != null)
                {
                    this.currentState[leftLayerState.Move.I, leftLayerState.Move.J] = EmptyMark;
                    leftLayerState = leftLayerState.Prev;
                }

                currDepth--;
            }

            IState moveState = null;
            foreach (var state in this.currentState.NextStates)
            {
                if (this.currentState.HeurValue == state.HeurValue)
                {
                    moveState = state;
                    break;
                }
            }

            return moveState?.Move;
        }

        private bool IsGoodWin(byte goodMark)
        {
            for (int i = 0; i < FieldSize; ++i)
            {
                int sum = 0;
                for (int j = 0; j < FieldSize - 1; ++j)
                {
                    if (this.currentState[i, j] == goodMark && this.currentState[i, j] == this.currentState[i, j + 1])
                    {
                        sum++;
                        if (sum == 4)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        sum = 0;
                    }
                }
            }

            for (int j = 0; j < FieldSize; ++j)
            {
                int sum = 0;
                for (int i = 0; i < FieldSize - 1; ++i)
                {
                    if (this.currentState[i, j] == goodMark && this.currentState[i, j] == this.currentState[i + 1, j])
                    {
                        sum++;
                        if (sum == 4)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        sum = 0;
                    }
                }
            }

            for (int i = 0; i < FieldSize - 4; ++i)
            {
                int sum1 = 0;
                int sum2 = 0;
                int localI = i;
                for (int j = 0; j < FieldSize - i - 1; ++j)
                {
                    if (this.currentState[localI, j] == goodMark && this.currentState[localI, j] == this.currentState[localI + 1, j + 1])
                    {
                        sum1++;
                        if (sum1 == 4)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        sum1 = 0;
                    }

                    if (this.currentState[j, localI] == goodMark && this.currentState[j, localI] == this.currentState[j + 1, localI + 1])
                    {
                        sum2++;
                        if (sum2 == 4)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        sum2 = 0;
                    }

                    localI++;
                }
            }

            for (int i = 4; i < FieldSize; ++i)
            {
                int sum1 = 0;
                int sum2 = 0;
                int localI = i;
                for (int j = 0; j < i; ++j)
                {
                    if (this.currentState[localI, j] == goodMark && this.currentState[localI, j] == this.currentState[localI - 1, j + 1])
                    {
                        sum1++;
                        if (sum1 == 4)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        sum1 = 0;
                    }

                    if (this.currentState[FieldSize - j - 1, FieldSize - localI - 1] == goodMark
                        && this.currentState[FieldSize - j - 1, FieldSize - localI - 1] == this.currentState[FieldSize - j - 2, FieldSize - localI])
                    {
                        sum2++;
                        if (sum2 == 4)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        sum2 = 0;
                    }

                    localI--;
                }
            }

            return false;
        }

        #region Heuristic

        private int Heuristic(byte goodMark, byte badMark)
        {
            int globalSum = 0;
            for (int i = 0; i < FieldSize; ++i)
            {
                for (int j = 0; j < FieldSize; ++j)
                {
                    if (this.currentState[i, j] == goodMark)
                    {
                        var singleHeuristic = this.SingleHeuristic(i, j, goodMark);
                        globalSum += singleHeuristic;
                    }
                    else if (this.currentState[i, j] == badMark)
                    {
                        var singleHeuristic = this.SingleHeuristic(i, j, badMark);
                        globalSum -= singleHeuristic;
                    }
                }
            }

            this.currentState.DisplayHeurValue = globalSum;
            this.StateChanged?.Invoke(this, this.currentState);
            MainWindow.WaitEvent.WaitOne();
            return globalSum;
        }

        private int SingleHeuristic(int i0, int j0, byte goodMark)
        {
            int[] sums = new int[4] { 1, 1, 1, 1 };
            RowOpenClose[] ends = new RowOpenClose[8];
            for (int i = 1; i < 6; ++i)
            {
                this.CheckCellInRow(goodMark, i0 + i, j0, 0, 0, sums, ends);
                this.CheckCellInRow(goodMark, i0 - i, j0, 0, 1, sums, ends);
                this.CheckCellInRow(goodMark, i0, j0 + i, 1, 2, sums, ends);
                this.CheckCellInRow(goodMark, i0, j0 - i, 1, 3, sums, ends);
                this.CheckCellInRow(goodMark, i0 + i, j0 + i, 2, 4, sums, ends);
                this.CheckCellInRow(goodMark, i0 - i, j0 - i, 2, 5, sums, ends);
                this.CheckCellInRow(goodMark, i0 + i, j0 - i, 3, 6, sums, ends);
                this.CheckCellInRow(goodMark, i0 - i, j0 + i, 3, 7, sums, ends);
            }

            int globalSum = 0;
            for (int i = 0; i < sums.Length; ++i)
            {
                if (sums[i] >= 5)
                {
                    return int.MaxValue;
                }

                globalSum += sums[i] * this.GetWeightedSum(sums[i], ends[i * 2], ends[(i * 2) + 1]);
            }

            return globalSum;
        }

        private int GetWeightedSum(int sum, RowOpenClose end1, RowOpenClose end2)
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

        private void CheckCellInRow(int goodMark, int i, int j, int sumI, int endI, int[] sums, RowOpenClose[] ends)
        {
            if (ends[endI] != RowOpenClose.Unknown)
            {
                return;
            }

            if (this.currentState[i, j] == goodMark)
            {
                sums[sumI]++;
            }
            else
            {
                if (this.currentState[i, j] == GameManager.EmptyMark)
                {
                    ends[endI] = RowOpenClose.Open;
                }
                else
                {
                    ends[endI] = RowOpenClose.Close;
                }
            }
        }

        #endregion
    }
}
