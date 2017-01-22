using Discord;
using System;
using System.Collections.Generic;

namespace YahurrBot_v._2.Games
{
	internal class ConnectFour : Modules.Game
	{
		private int[,] board = new int[7, 6];
		private bool won = false;
		private int goal = 4;

		public override void ConfigureGame ()
		{
			settings.maxPlayers = 1;
		}

		public override string StartGame ( User creator, List<User> joined )
		{
			drawBoard ();
			won = false;
			return printBoard ();
		}

		public override string PlayRound ( int player, string[] arguments )
		{
			int tryParseNumber = 0;
			if (!int.TryParse (arguments[0], out tryParseNumber))
			{
				return "";
			}
			if (!placeChip (tryParseNumber, player + 1))
			{
				return "";
			}
			else
			{
				checkIfWon (tryParseNumber);
				return printBoard ();
			}
		}

		public override bool HasWon ()
		{
			if (!won)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		private void drawBoard ()
		{
			for (int i = 0; i <= board.GetUpperBound (1); i++)
			{
				for (int o = 0; o <= board.GetUpperBound (0); o++)
				{
					board[o, i] = 0;
				}
			}
		}

		private string printBoard ()
		{
			string brett = "";
			for (int i = 0; i <= board.GetUpperBound (1); i++)
			{
				brett += "`|";
				for (int o = 0; o <= board.GetUpperBound (0); o++)
				{
					switch (board[o, board.GetUpperBound (1) - i])
					{
						case 0:
						brett += "ｏ";
						break;

						case 1:
						brett += "﻿Ｘ";
						break;

						case 2:
						brett += "０﻿";
						break;

						default:
						break;
					}
				}
				brett += "|`";
				if (i != board.GetUpperBound (1))
				{
					brett += Environment.NewLine;
				}
			}
			brett += Environment.NewLine + "`|﻿１２３４５６７|﻿`";
			return (brett);
		}

		private int findFirstFree ( int row )
		{
			for (int i = 0; i <= board.GetUpperBound (1); i++)
			{
				if (board[row, i] == 0)
				{
					return i;
				}
			}
			return board.GetUpperBound (1) + 1;
		}

		private bool placeChip ( int row, int player )
		{
			row -= 1;
			if (row < 0 || row > board.GetUpperBound (0))
			{
				return false;
			}
			int column = findFirstFree (row); //I dette tilfellet er column brukt for row og det motsatte med row. Jeg var for lat til å fikse det.
			if (column > board.GetUpperBound (1))
			{
				return false;
			}
			else
			{
				board[row, column] = player;
				return true;
			}
		}

		private void checkIfWon ( int row )
		{
			if (checkHorizontal (row) || checkVertical (row) || checkDiagonal1 (row) || checkDiagonal2 (row))
			{
				won = true;
			}
		}

		private bool checkHorizontal ( int row )
		{
			row -= 1;
			int column = findFirstFree (row) - 1;
			int player = board[row, column];
			int inARow = 1;

			for (int i = 1; i < goal; i++)
			{
				if (row - i < 0)
				{
					break;
				}
				else if (board[row - i, column] == player)
				{
					inARow += 1;
				}
				else
				{
					break;
				}
			}
			for (int i = 1; i < goal; i++)
			{
				if (row + i > board.GetUpperBound (0))
				{
					break;
				}
				else if (board[row + i, column] == player)
				{
					inARow += 1;
				}
				else
				{
					break;
				}
			}
			if (inARow >= goal)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool checkVertical ( int row )
		{
			row -= 1;
			int column = findFirstFree (row) - 1;
			int player = board[row, column];
			int inARow = 1;

			for (int i = 1; i < goal; i++)
			{
				if (column - i < 0)
				{
					break;
				}
				else if (board[row, column - i] == player)
				{
					inARow += 1;
				}
				else
				{
					break;
				}
			}
			for (int i = 1; i < goal; i++)
			{
				if (column + i > board.GetUpperBound (1))
				{
					break;
				}
				else if (board[row, column + i] == player)
				{
					inARow += 1;
				}
				else
				{
					break;
				}
			}
			if (inARow >= goal)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool checkDiagonal1 ( int row )
		{
			row -= 1;
			int column = findFirstFree (row) - 1;
			int player = board[row, column];
			int inARow = 1;

			for (int i = 1; i < goal; i++)
			{
				if (row - i < 0 || column - i < 0)
				{
					break;
				}
				else if (board[row - i, column - i] == player)
				{
					inARow += 1;
				}
				else
				{
					break;
				}
			}
			for (int i = 1; i < goal; i++)
			{
				if (row + i > board.GetUpperBound (0) || column + i > board.GetUpperBound (1))
				{
					break;
				}
				else if (board[row + i, column + i] == player)
				{
					inARow += 1;
				}
				else
				{
					break;
				}
			}
			if (inARow >= goal)
			{
				Console.WriteLine ("Diagonal1");
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool checkDiagonal2 ( int row )
		{
			row -= 1;
			int column = findFirstFree (row) - 1;
			int player = board[row, column];
			int inARow = 1;

			for (int i = 1; i < goal; i++)
			{
				if (row - i < 0 || column + i > board.GetUpperBound (1))
				{
					break;
				}
				else if (board[row - i, column + i] == player)
				{
					inARow += 1;
				}
				else
				{
					break;
				}
			}
			for (int i = 1; i < goal; i++)
			{
				if (row + i > board.GetUpperBound (0) || column - i < 0)
				{
					break;
				}
				else if (board[row + i, column - i] == player)
				{
					inARow += 1;
				}
				else
				{
					break;
				}
			}
			if (inARow >= goal)
			{
				Console.WriteLine ("Diagonal2");
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}