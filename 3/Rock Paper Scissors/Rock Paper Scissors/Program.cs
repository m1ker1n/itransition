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
                bool repeatInput;
                int computerMoveInd = GenerateComputerMove(moves);
                string compMoveStr = "Computer move: " + moves[computerMoveInd];
                var verifier = new Verifier();
                string hmac = verifier.GetHMAC(compMoveStr);
                string hmacKey = verifier.Key;
                Console.WriteLine("HMAC: " + hmac);
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
                                userMoveInd--;
                                Console.WriteLine("Your move: " + moves[userMoveInd]);
                                Console.WriteLine(compMoveStr);
                                var result = GameRules.GetResult(computerMoveInd, userMoveInd, moves);
                                HandleResult(result);
                                Console.WriteLine("HMAC Key: " + hmacKey);
                            }
                            else
                                repeatInput = true;
                            break;
                    }
                } while (repeatInput);
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
    }
}
