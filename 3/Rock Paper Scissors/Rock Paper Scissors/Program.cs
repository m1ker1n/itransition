using System;
using System.Collections.Generic;
using System.Linq;

namespace Rock_Paper_Scissors
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool argsErr = false;
            var moves = InitializeMoves(args, ref argsErr);
            MovesValidityCheck(moves, ref argsErr);
            if (argsErr) return;

            bool gameOver = false;
            var table = new Table(moves);
            while(!gameOver)
            {
                int compMoveIndex = GenerateComputerMove(moves);
                string compMoveStr = "Computer move: " + moves[compMoveIndex];
                var verifier = new Verifier();
                string hmac = verifier.GetHMAC(compMoveStr);
                string hmacKey = verifier.Key;
                Console.WriteLine("HMAC: " + hmac);

                int userMoveIndex = GetUserMoveIndex(ref gameOver, moves, table);
                if (userMoveIndex < 0) break;

                OnReceivingInput(moves, userMoveIndex, compMoveIndex, compMoveStr);

                Console.WriteLine("HMAC Key: " + hmacKey);
            }
        }

        static void ShowMenu(List<string> moves)
        {
            Console.WriteLine("Available moves:");
            for (int i = 1; i <= moves.Count; i++)
            {
                Console.WriteLine(i + " - " + moves[i-1]);
            }
            Console.WriteLine("0 - exit");
            Console.WriteLine("? - help");
            Console.Write("Enter your move: ");
        }

        static int GenerateComputerMove(List<string> moves)
        {
            var rand = new Random();
            return rand.Next(0, moves.Count);
        }

        static void HandleResult(GameResult result)
        {
            switch (result)
            {
                case GameResult.Win:
                    Console.WriteLine("You win!");
                    break;
                case GameResult.Draw:
                    Console.WriteLine("Draw!");
                    break;
                case GameResult.Lose:
                    Console.WriteLine("You lose!");
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        static List<string> InitializeMoves(string[] args, ref bool argsErr)
        {
            var moves = new List<string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (moves.Contains(args[i]))
                {
                    Console.WriteLine(args[i] + " already added. All possible moves must be different.");
                    argsErr = true;
                }
                else
                    moves.Add(args[i]);
            }
            return moves;
        }

        static void MovesValidityCheck(List<string> moves, ref bool argsErr)
        {
            if (moves.Count < 3)
            {
                Console.WriteLine("There must be at least 3 moves, e.g. Rock Paper Scissors.");
                argsErr = true;
            }

            if (moves.Count % 2 == 0)
            {
                Console.WriteLine("There must be odd amount of moves, e.g. Rock Paper Scissors.");
                argsErr = true;
            }
        }

        static int GetUserMoveIndex(ref bool gameOver, List<string> moves, Table table)
        {
            bool repeatInput;
            do
            {
                repeatInput = false;
                ShowMenu(moves);
                var userInput = Console.ReadLine().Trim();

                switch (userInput)
                {
                    case "?":
                        table.Draw();
                        repeatInput = true;
                        break;
                    case "0":
                        gameOver = true;
                        break;
                    default:
                        int userMoveInd;
                        if (int.TryParse(userInput, out userMoveInd) && userMoveInd <= moves.Count)
                        {
                            return --userMoveInd;
                        }
                        else
                            repeatInput = true;
                        break;
                }
            } while (repeatInput);

            return -1;
        }

        static void OnReceivingInput(List<string> moves, int userMoveIndex, int compMoveIndex, string compMoveStr)
        {
            Console.WriteLine("Your move: " + moves[userMoveIndex]);
            Console.WriteLine(compMoveStr);
            var result = GameRules.GetResult(compMoveIndex, userMoveIndex, moves);
            HandleResult(result);
        }
    }
}
