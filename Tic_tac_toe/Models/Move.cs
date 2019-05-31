using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tic_tac_toe.Services;

namespace Tic_tac_toe.Models
{
    public class Move
    {
        public int I { get; set; } = -1;

        public int J { get; set; } = -1;

        public byte MoveMark { get; set; } = GameManager.PlayerMark;

        public override bool Equals(object obj)
        {
            if (!(obj is Move))
            {
                return false;
            }

            Move move2 = obj as Move;
            return this.I == move2.I && this.J == move2.J && this.MoveMark == move2.MoveMark;
        }

        public override int GetHashCode()
        {
            return this.I.GetHashCode() + this.J.GetHashCode() + this.MoveMark.GetHashCode();
        }
    }
}
