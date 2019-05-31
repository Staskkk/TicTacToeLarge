using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tic_tac_toe.Services;

namespace Tic_tac_toe.Models
{
    public class State : IState
    {
        public IState Prev { get; set; }

        public HashSet<IState> NextStates { get; set; }

        public int Depth { get; set; }

        public int? HeurValue { get; set; }

        public Move Move { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is State))
            {
                return false;
            }

            return this.Move?.Equals((obj as State).Move) ?? false;
        }

        public override int GetHashCode()
        {
            return this.Move?.GetHashCode() ?? 0;
        }
    }
}
