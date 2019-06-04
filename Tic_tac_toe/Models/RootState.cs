using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tic_tac_toe.Services;

namespace Tic_tac_toe.Models
{
    public class RootState : IState
    {
        private byte[,] arr;

        public RootState()
        {
            this.SetEmpty();
        }

        public IState Prev { get; } = null;

        public HashSet<IState> NextStates { get; set; }

        public int Depth { get; } = 0;

        public int? HeurValue { get; set; }

        public int DisplayHeurValue { get; set; }

        public Move Move { get; } = null;

        public byte this[int i, int j]
        {
            get
            {
                return this.arr[i + GameManager.FieldOffset, j + GameManager.FieldOffset];
            }

            set
            {
                this.arr[i + GameManager.FieldOffset, j + GameManager.FieldOffset] = value;
            }
        }

        public void SetEmpty()
        {
            this.arr = new byte[GameManager.FieldSize + (2 * GameManager.FieldOffset), GameManager.FieldSize + (2 * GameManager.FieldOffset)];
            for (int i = 0; i < this.arr.GetLength(0); ++i)
            {
                for (int j = 0; j < this.arr.GetLength(1); ++j)
                {
                    this.arr[i, j] = GameManager.WallMark;
                }
            }

            for (int i = GameManager.FieldOffset; i < GameManager.FieldSize + GameManager.FieldOffset; ++i)
            {
                for (int j = GameManager.FieldOffset; j < GameManager.FieldSize + GameManager.FieldOffset; ++j)
                {
                    this.arr[i, j] = GameManager.EmptyMark;
                }
            }
        }
    }
}
