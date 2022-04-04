using System;
using System.Collections.Generic;

using ConsoleTables;

namespace Rock_Paper_Scissors
{
    internal class Table
    {
        private List<string> _moves;

        public Table(List<string> moves)
        {
            _moves = moves;
        }

        public void Draw()
        {
            string[] columns = PrepareColumns();
            var table = new ConsoleTable(new ConsoleTableOptions { EnableCount = false });
            table.AddColumn(columns);
            for (int i = 0; i<_moves.Count; i++)
            {
                string[] row = PrepareRow(i);
                table.AddRow(row);
            }
            table.Write();
        }

        private string[] PrepareColumns()
        {
            string[] result = new string[_moves.Count + 1];
            result[0] = string.Empty;
            for (int i = 0; i < _moves.Count; i++)
                result[i + 1] = _moves[i];
            return result;
        }
        
        private string[] PrepareRow(int moveIndex)
        {
            string[] result = new string[_moves.Count + 1];
            result[0] = _moves[moveIndex];
            for (int i = 0; i < _moves.Count; i++)
                result[i + 1] = GameRules.GetResult(i, moveIndex, _moves).ToString(); 
            return result;
        }
    }
}
