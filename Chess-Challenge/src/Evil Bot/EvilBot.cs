using ChessChallenge.API;
using System;

public class EvilBot : IChessBot
{
    // https://seblague.github.io/chess-coding-challenge/documentation/
    // Documentation

    // Piece values: pawn, knight, bishop, rook, queen, king
    // int[] pieceValues = { 100, 320, 350, 500, 900, 10000 };
    int[,,] allPiecesBlackPositionValues =
    {
        { // 0 pawn (100)
            {  0,   0,   0,   0,   0,   0,   0,   0}, //h1 to a1 when calculated for white
            {180, 180, 180, 180, 180, 180, 180, 180},
            {130, 135, 140, 145, 145, 140, 135, 130},
            {105, 105, 110, 125, 125, 110, 105, 105},
            {100, 100, 110, 120, 120, 100, 100, 100},
            {105, 105,  95, 100, 100, 100, 105, 105}, // intentionally asymmetric - discourages f6/f3
            {105, 110, 110,  80,  80, 110, 110, 105},
            {  0,   0,   0,   0,   0,   0,   0,   0} // h8 to a8 when calculated for white
        },
        { // 1 knight (320)
            {290, 300, 305, 305, 305, 305, 300, 290},
            {300, 310, 320, 320, 320, 320, 310, 300},
            {305, 310, 331, 333, 333, 331, 310, 305},
            {305, 320, 335, 335, 335, 335, 320, 305},
            {305, 320, 335, 335, 335, 335, 320, 305},
            {305, 315, 330, 335, 335, 330, 335, 305},
            {300, 310, 320, 325, 325, 320, 310, 305},
            {290, 300, 305, 305, 305, 305, 300, 290}
        },
        { // 2 bishiop (350)
            {330, 340, 340, 340, 340, 340, 340, 330},
            {340, 350, 350, 350, 350, 350, 350, 340},
            {340, 350, 355, 355, 355, 355, 350, 340},
            {340, 355, 355, 360, 360, 355, 355, 340},
            {340, 350, 360, 360, 360, 360, 350, 340},
            {340, 360, 360, 360, 360, 360, 360, 340},
            {340, 360, 350, 350, 350, 350, 360, 340},
            {340, 340, 330, 330, 330, 330, 340, 340}
        },
        { // 3 rook (500)
            {500, 500, 500, 500, 500, 500, 500, 500},
            {505, 510, 510, 510, 510, 510, 510, 505},
            {495, 500, 500, 500, 500, 500, 500, 495},
            {495, 500, 500, 500, 500, 500, 500, 495},
            {495, 500, 500, 500, 500, 500, 500, 495},
            {495, 500, 500, 500, 500, 500, 500, 495},
            {495, 500, 500, 500, 500, 500, 500, 495},
            {498, 500, 505, 505, 505, 505, 500, 498}
        },
        { // 4 queen (900)
            {880, 890, 890, 895, 895, 890, 890, 880},
            {890, 900, 900, 900, 900, 900, 900, 890},
            {890, 900, 905, 905, 905, 905, 900, 890},
            {895, 900, 905, 905, 905, 905, 900, 895},
            {895, 900, 905, 905, 905, 905, 900, 895},
            {890, 900, 905, 905, 905, 905, 900, 890},
            {890, 900, 900, 900, 900, 900, 900, 890},
            {880, 890, 890, 895, 895, 890, 890, 880}
        },
        { // 5 king (10000)
            { 9970,  9960,  9960,  9950,  9950,  9960,  9960,  9970},
            { 9970,  9960,  9960,  9950,  9950,  9960,  9960,  9970},
            { 9970,  9960,  9960,  9950,  9950,  9960,  9960,  9970},
            { 9970,  9960,  9960,  9950,  9950,  9960,  9960,  9970},
            { 9980,  9970,  9970,  9960,  9960,  9970,  9970,  9980},
            { 9990,  9980,  9980,  9970,  9970,  9980,  9980,  9990},
            {10000, 10000,  9980,  9970,  9970,  9980, 10000, 10000},
            {10015, 10030, 10005, 10000, 10000, 10025, 10030, 10015} // intentionally assymetric. encourages castling 
        },
        { // 6 king endgame (10000)
            { 9950,  9960,  9970,  9980,  9980,  9970,  9960,  9950},
            { 9970,  9980,  9990, 10000, 10000,  9990,  9980,  9970},
            { 9970,  9990, 10020, 10030, 10030, 10020,  9990,  9970},
            { 9970,  9990, 10030, 10040, 10040, 10030,  9990,  9970},
            { 9970,  9990, 10030, 10040, 10040, 10030,  9990,  9970},
            { 9970,  9990, 10020, 10030, 10030, 10020,  9990,  9970},
            { 9970,  9970, 10000, 10000, 10000, 10000,  9970,  9970},
            { 9950,  9970,  9970,  9970,  9970,  9970,  9970,  9950}
        }
    };

    public Move Think(Board board, Timer timer)
    {
        Move[] allMoves = board.GetLegalMoves();
        Move moveToPlay = new Move();
        int alpha = int.MinValue; // Alpha is the minimum evaluation we can guarantee for the current player ("maximizing player")
        int beta = int.MaxValue; // Beta is the maximum evaluation that we can guarantee for the opponent ("minimizing player")
        int evaluation = int.MinValue;
        int bestEval = int.MinValue;
        int depth = 3; // we can get rid of this variable in the future

        foreach (Move move in allMoves)
        {
            board.MakeMove(move);
            evaluation = Search(board, depth, alpha, beta, false, 0); // this function is now recursive
            board.UndoMove(move);

            /*
            Console.Write(move.ToString());
            Console.Write("|");
            Console.WriteLine(evaluation);
            */
            if (evaluation > bestEval)
            {
                bestEval = evaluation;
                moveToPlay = move;
            }
        }

        /*
        Console.Write(moveToPlay.ToString());
        Console.Write("|");
        Console.WriteLine(alpha);
        Console.WriteLine("--------------------");
        */

        return moveToPlay;
    }

    int Search(Board board, int depth, int alpha, int beta, bool maximizingPlayer, int numExtensions)
    {
        if (depth == 0)
        {
            // reached the end of recursive loop, evaluate the position now
            return Evaluate(board, maximizingPlayer);
        }

        Move[] allMoves = board.GetLegalMoves();
        if (allMoves.Length == 0)
        {
            if (board.IsInCheck()) // checkmate
            {
                if (maximizingPlayer) // we will lose, that's bad
                {
                    return -999999; // don't set to int.MinValue or else forced checkmates return Null
                }
                return int.MaxValue; // opponent will lose, that's good
            }
            else
            {
                return 0; //stalemate
            }
        }

        if (maximizingPlayer)
        {
            int evaluation = int.MinValue;
            foreach (Move move in allMoves)
            {
                board.MakeMove(move);
                int extension = (numExtensions < 2 && board.IsInCheck()) ? 2 : 0; // go one move deeper (2 ply) if we put opponent in check with max 2 extra
                evaluation = Math.Max(evaluation, Search(board, depth - 1 + extension, alpha, beta, false, numExtensions + extension));
                board.UndoMove(move);
                if (evaluation > beta) break; // this move is better for the MinPlayer (opponent) than a previous branch's maximum possible (beta). Stop this tree
                alpha = Math.Max(alpha, evaluation);
            }
            return evaluation;
        }

        else // not maximizingPlayer
        {
            int evaluation = int.MaxValue;
            foreach (Move move in allMoves)
            {
                board.MakeMove(move);
                evaluation = Math.Min(evaluation, Search(board, depth - 1, alpha, beta, true, numExtensions));
                board.UndoMove(move);
                if (evaluation < alpha) break; // this move is worse for the MaxPlayer (self) than a previous branch's minimum possible (alpha). Stop this tree
                beta = Math.Min(beta, evaluation);
            }
            return evaluation;
        }
    }

    // Main method for evaluating the "score" of a position.
    public int Evaluate(Board board, bool maximizingPlayer)
    {
        if (board.IsDraw()) return 0; // handles repetitions
        PieceList[] arrayOfPieceLists = board.GetAllPieceLists();
        int positionEvaluation = GetAllPiecesPositionValue(arrayOfPieceLists);
        if ((maximizingPlayer && !board.IsWhiteToMove) || (!maximizingPlayer && board.IsWhiteToMove))
        {
            return -positionEvaluation; // we're playing black or we're evaluating for the white opponent
        }
        return positionEvaluation; // we're playing white or we're evaluating for the black opponent (double negative)
    }

    public int GetAllPiecesPositionValue(PieceList[] pieceLists)
    {
        int total = 0;
        bool whitePieces = true;
        bool isEndgame = false;
        int minorPieceCount = pieceLists[1].Count + pieceLists[2].Count + pieceLists[3].Count;
        if ((pieceLists[4].Count == 0 && pieceLists[10].Count == 0 && minorPieceCount <= 4) || (minorPieceCount <= 2))
        // Queens are off the board or two or fewer knight/bishop/rook, we're endgame now. Technically we could check black's pieces too but if the two players are mismatched by an entire minor piece, the game is probably over
        {
            isEndgame = true;
        }
        for (int i = 0; i <= 11; i++)
        {
            if (i == 6)
            {
                whitePieces = false; // flip values for black for 6-11
            }
            foreach (Piece piece in pieceLists[i])
            {
                if ((i == 5 || i == 11) && isEndgame)
                {
                    if (whitePieces) // white king during endgame
                    {
                        total += allPiecesBlackPositionValues[6, 7 - piece.Square.Rank, piece.Square.File]; // "7 minus File" not necessary since king endgame table is symmetric
                    }
                    else // black king during endgame
                    {
                        total -= allPiecesBlackPositionValues[6, piece.Square.Rank, piece.Square.File];
                    }
                }
                else if (whitePieces) // white pieces
                {
                    total += allPiecesBlackPositionValues[i, 7 - piece.Square.Rank, 7 - piece.Square.File]; // "7 minus File" to handle assymetric cases
                }
                else // black pieces
                {
                    total -= allPiecesBlackPositionValues[i % 6, piece.Square.Rank, piece.Square.File];
                }
            }
        }
        return total;
    }
}