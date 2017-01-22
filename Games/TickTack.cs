using Discord;
using System;
using System.Collections.Generic;

namespace YahurrBot_v._2.Games
{
	internal class TickTack : Modules.Game
	{
		private Square[,] board;
		private List<char> symbols = new List<char> () { 'X', 'O', '/', '|', '-' };
		private int lastX, lastY, lastPlayer;

		public override string StartGame ( User creator, List<User> joined )
		{
			board = MakeBoard (3);

			return ShowBard ();
		}

		public override string PlayRound ( int player, string[] arguments )
		{
			int x = char.ToUpper (arguments[0][0]) - 65;
			int y = (int)char.GetNumericValue (char.ToUpper (arguments[0][1])) - 1;

			Console.WriteLine (x + ":" + y);

			if (!board[x, y].taken)
			{
				board[x, y].Take (symbols[player]);
				lastX = x;
				lastY = y;
				lastPlayer = player;
			}

			return ShowBard ();
		}

		public override bool HasWon ()
		{
			return HasWon (lastX, lastY, symbols[lastPlayer]);
		}

		public bool HasWon ( int x, int y, char symbol )
		{
			int winCountX = 0;
			for (int i = 0; i < board.GetUpperBound (1); i++)
			{
				if (board[x, i].symbol == symbol)
				{
					winCountX++;
				}
			}

			int winCountY = 0;
			for (int i = 0; i < board.GetUpperBound (0); i++)
			{
				if (board[i, y].symbol == symbol)
				{
					winCountY++;
				}
			}

			for (int ix = 0; ix < board.GetUpperBound (0); ix++)
			{
				if (board.GetUpperBound (1) - ix >= 3)
				{
					int spree = 0;
					for (int iy = 0; iy < board.GetUpperBound (1) - ix; iy++)
					{
						if (board[ix + iy, iy].symbol == symbol)
						{
							spree++;
						}
						else
						{
							spree--;
						}
					}

					if (spree >= 3)
					{
						return true;
					}
				}
			}

			if (x == 1 && y == 1)
			{
				if (board[x + 1, y + 1].symbol == symbol && board[x - 1, y - 1].symbol == symbol)
				{
					return true;
				}

				if (board[x - 1, y + 1].symbol == symbol && board[x + 1, y - 1].symbol == symbol)
				{
					return true;
				}
			}

			return winCountX >= 3 || winCountY >= 3;
		}

		private Square[,] MakeBoard ( int size )
		{
			size++;
			Square[,] board = new Square[size, size];

			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					board[x, y] = new Square ();
				}
			}

			return board;
		}

		private string ShowBard ()
		{
			string toSend = "```  ";

			for (int x = 1; x < board.GetUpperBound (0) + 1; x++)
			{
				toSend += " " + x + "  ";
			}

			toSend += Environment.NewLine;

			for (int x = 0; x < board.GetUpperBound (0) * 2 - 1; x++)
			{
				string line = "  ";
				if (x % 2 == 0)
				{
					line = Number2String (x / 2 + 1, true) + " ";
				}

				for (int y = 0; y < board.GetUpperBound (1); y++)
				{
					if (x % 2 == 0)
					{
						line += " " + board[x / 2, y].symbol + " |";
					}
					else
					{
						line += "---+";
					}
				}

				toSend += line.Substring (0, line.Length - 1) + Environment.NewLine;
			}

			toSend += "```";

			return toSend;
		}

		private string Number2String ( int number, bool isCaps )
		{
			char c = (char)((isCaps ? 65 : 97) + (number - 1));
			return c.ToString ();
		}
	}

	internal class Square
	{
		private char squareSymbol = ' ';

		public char symbol
		{
			get
			{
				return squareSymbol;
			}
		}

		private bool sqareTaken;

		public bool taken
		{
			get
			{
				return sqareTaken;
			}
		}

		public void Take ( char symbol )
		{
			squareSymbol = symbol;
			sqareTaken = true;
		}
	}
}