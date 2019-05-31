using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tic_tac_toe.Models
{
    public interface IState
    {
        IState Prev { get; }

        HashSet<IState> NextStates { get; set; }

        int Depth { get; }

        int? HeurValue { get; set; }

        Move Move { get; }
    }
}
