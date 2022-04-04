using System;
using System.Collections.Generic;

namespace Rock_Paper_Scissors
{
    internal class Table
    {
        private Dictionary<int, string> _moves;
        private int _maxLen;
        static private string borderCh = "*";

        public Table(Dictionary<int,string> moves)
        {
            _moves = moves;
            _maxLen = 4;
            foreach(var val in moves.Values)
            {
                _maxLen = _maxLen < val.Length ? val.Length : _maxLen;
            }
        }

        public void Draw()
        {
            DrawHorizontalLine();
            DrawElement("", true);
            foreach(var val in _moves.Values)
            {
                DrawElement(val);
            }
            Console.WriteLine();

            for(int index1 = 0; index1 < _moves.Count; index1++)
            {
                DrawHorizontalLine();
                DrawElement(_moves[index1], true);
                for (int index2 = 0; index2 < _moves.Count; index2++) 
                {
                    DrawElement(GameRules.GetResult(index1, index2, _moves).ToString());
                }
                Console.WriteLine();
            }
            
            DrawHorizontalLine();
        }

        private void DrawHorizontalLine()
        {
            for (int i = 0; i < (_moves.Count + 1) * (_maxLen + 1) + 1; i++)
                Console.Write(borderCh);
            Console.WriteLine();
        }

        private void DrawElement(string content, bool first = false)
        {
            if (first) Console.Write(borderCh);
            for (int i = 0; i < (_maxLen - content.Length) / 2 + (_maxLen - content.Length) % 2; i++)
                Console.Write(" ");
            Console.Write(content);
            for (int i = 0; i < (_maxLen - content.Length) / 2; i++)
                Console.Write(" ");
            Console.Write(borderCh);
        }
    }
}
