using System;
using System.Collections.Generic;

public enum PieceType
{
    None,
    Regular,
    King
}

public enum Player
{
    None,
    Player1,
    Player2
}

public class Piece
{
    public Player Owner { get; set; }
    public PieceType Type { get; set; }
    public bool IsKing => Type == PieceType.King;

    public Piece(Player owner, PieceType type)
    {
        Owner = owner;
        Type = type;
    }

    public void PromoteToKing()
    {
        if (Type == PieceType.Regular)
            Type = PieceType.King;
    }
}

public class Board
{
    public Piece[,] Grid { get; private set; }
    public const int Size = 8;

    public Board()
    {
        Grid = new Piece[Size, Size];
        InitializeBoard();
    }

    public void InitializeBoard()
    {
        for (int row = 0; row < Size; row++)
        {
            for (int col = 0; col < Size; col++)
            {
                if ((row + col) % 2 == 1)
                {
                    if (row < 3)
                        Grid[row, col] = new Piece(Player.Player1, PieceType.Regular);
                    else if (row > 4)
                        Grid[row, col] = new Piece(Player.Player2, PieceType.Regular);
                    else
                        Grid[row, col] = new Piece(Player.None, PieceType.None);
                }
                else
                {
                    Grid[row, col] = new Piece(Player.None, PieceType.None);
                }
            }
        }
    }

    public void DisplayBoard()
    {
        for (int row = 0; row < Size; row++)
        {
            for (int col = 0; col < Size; col++)
            {
                if (Grid[row, col].Owner == Player.Player1)
                    Console.Write(" P1 ");
                else if (Grid[row, col].Owner == Player.Player2)
                    Console.Write(" P2 ");
                else
                    Console.Write(" .  ");
            }
            Console.WriteLine();
        }
    }

    public bool IsMoveValid(int startRow, int startCol, int endRow, int endCol, Player currentPlayer)
    {
        if (startRow < 0 || startRow >= Size || startCol < 0 || startCol >= Size ||
            endRow < 0 || endRow >= Size || endCol < 0 || endCol >= Size)
        {
            return false;
        }

        var piece = Grid[startRow, startCol];
        if (piece.Owner != currentPlayer || piece.Owner == Player.None)
            return false;

        var destination = Grid[endRow, endCol];
        if (destination.Owner != Player.None)
            return false;

        // Regular piece movement (only forward)
        if (piece.Type == PieceType.Regular)
        {
            if (currentPlayer == Player.Player1)
            {
                if (endRow == startRow + 1 && Math.Abs(endCol - startCol) == 1)
                    return true;
            }
            else
            {
                if (endRow == startRow - 1 && Math.Abs(endCol - startCol) == 1)
                    return true;
            }
        }
        // King piece movement (can move both directions)
        else if (piece.Type == PieceType.King)
        {
            if (Math.Abs(endRow - startRow) == 1 && Math.Abs(endCol - startCol) == 1)
                return true;
        }

        return false;
    }

    public bool TryMovePiece(int startRow, int startCol, int endRow, int endCol, Player currentPlayer)
    {
        if (IsMoveValid(startRow, startCol, endRow, endCol, currentPlayer))
        {
            Grid[endRow, endCol] = Grid[startRow, startCol];
            Grid[startRow, startCol] = new Piece(Player.None, PieceType.None);

            // Promotion to King if it reaches the other side
            if ((currentPlayer == Player.Player1 && endRow == Size - 1) ||
                (currentPlayer == Player.Player2 && endRow == 0))
            {
                Grid[endRow, endCol].PromoteToKing();
            }

            return true;
        }

        return false;
    }

    public bool IsGameOver()
    {
        bool player1PiecesLeft = false;
        bool player2PiecesLeft = false;

        foreach (var piece in Grid)
        {
            if (piece.Owner == Player.Player1)
                player1PiecesLeft = true;
            if (piece.Owner == Player.Player2)
                player2PiecesLeft = true;
        }

        return !player1PiecesLeft || !player2PiecesLeft;
    }
}

public class CheckersGame
{
    private Board board;
    private Player currentPlayer;

    public CheckersGame()
    {
        board = new Board();
        currentPlayer = Player.Player1;
    }

    public void Start()
    {
        while (!board.IsGameOver())
        {
            board.DisplayBoard();
            Console.WriteLine($"Player {(currentPlayer == Player.Player1 ? 1 : 2)}'s turn:");
            Console.WriteLine("Enter your move (e.g., 'a3 b4'):");
            var input = Console.ReadLine();
            var move = input.Split(' ');
            var start = ParseCoordinates(move[0]);
            var end = ParseCoordinates(move[1]);

            if (start == null || end == null)
            {
                Console.WriteLine("Invalid move format.");
                continue;
            }

            if (board.TryMovePiece(start.Value.Item1, start.Value.Item2, end.Value.Item1, end.Value.Item2, currentPlayer))
            {
                currentPlayer = currentPlayer == Player.Player1 ? Player.Player2 : Player.Player1;
            }
            else
            {
                Console.WriteLine("Invalid move. Try again.");
            }
        }

        Console.WriteLine("Game Over!");
        Console.WriteLine($"Player {(currentPlayer == Player.Player1 ? 2 : 1)} wins!");
    }

    private (int, int)? ParseCoordinates(string coordinate)
    {
        if (coordinate.Length != 2)
            return null;

        int col = coordinate[0] - 'a';
        int row = 8 - (coordinate[1] - '0');

        if (col < 0 || col >= Board.Size || row < 0 || row >= Board.Size)
            return null;

        return (row, col);
    }
}

public class Program
{
    public static void Main()
    {
        CheckersGame game = new CheckersGame();
        game.Start();
    }
}
