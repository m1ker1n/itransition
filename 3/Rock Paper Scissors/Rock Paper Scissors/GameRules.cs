using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rock_Paper_Scissors
{
    public enum GameResult
    {
        Win = 0,
        Draw,
        Lose
    }

    static class GameRules
    {
        static public GameResult GetResult(int computerMoveIndex, int userMoveIndex, List<string> moves)
        {
            if (userMoveIndex == computerMoveIndex) return GameResult.Draw;
            if (computerMoveIndex + ((computerMoveIndex < userMoveIndex) ? moves.Count : 0) <= userMoveIndex + moves.Count / 2)
                return GameResult.Win;
            return GameResult.Lose;
        }
    }
}
