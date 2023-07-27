﻿using ChessChallenge.API;
using System;

public class MyBot : IChessBot
{
    // https://seblague.github.io/chess-coding-challenge/documentation/
    // Documentation

    // Piece values: pawn, knight, bishop, rook, queen, king
    int[] pieceValues = { 100, 320, 350, 500, 900, 10000 };
    int[,] pawnBlackPositionValues =
        {
            { 0,  0,  0,  0,  0,  0,  0,  0 },
            {50, 50, 50, 50, 50, 50, 50, 50 },
            {20, 20, 20, 30, 30, 20, 20, 20 },
            { 5,  5, 10, 25, 25, 10,  5,  5 },
            { 0,  0,  0, 20, 20,  0,  0,  0 },
            { 5, -5,-10,  0,  0,-10, -5,  5 },
            { 5, 10, 10,-20,-20, 10, 10,  5 },
            { 0,  0,  0,  0,  0,  0,  0,  0 }
        };
    int[,] knightBlackPositionValues =
        {
            {-50,-40,-30,-30,-30,-30,-40,-50},
            {-40,-20,  0,  0,  0,  0,-20,-40},
            {-30,  0, 10, 15, 15, 10,  0,-30},
            {-30,  5, 15, 15, 15, 15,  5,-30},
            {-30,  0, 15, 15, 15, 15,  0,-30},
            {-30,  5, 10, 15, 15, 10,  5,-30},
            {-40,-20,  0,  5,  5,  0,-20,-40},
            {-50,-40,-30,-30,-30,-30,-40,-50}
        };
    int[,] bishopBlackPositionValues =
        {
            {-20,-10,-10,-10,-10,-10,-10,-20},
            {-10,  0,  0,  0,  0,  0,  0,-10},
            {-10,  0,  5, 10, 10,  5,  0,-10},
            {-10,  5,  5, 10, 10,  5,  5,-10},
            {-10,  0, 10, 10, 10, 10,  0,-10},
            {-10, 10, 10, 10, 10, 10, 10,-10},
            {-10,  5,  0,  0,  0,  0,  5,-10},
            {-20,-10,-10,-10,-10,-10,-10,-20}
        };
    int[,] rookBlackPositionValues =
        {
            { 0,  0,  0,  0,  0,  0,  0,  0},
            { 5, 10, 10, 10, 10, 10, 10,  5},
            {-5,  0,  0,  0,  0,  0,  0, -5},
            {-5,  0,  0,  0,  0,  0,  0, -5},
            {-5,  0,  0,  0,  0,  0,  0, -5},
            {-5,  0,  0,  0,  0,  0,  0, -5},
            {-5,  0,  0,  0,  0,  0,  0, -5},
            { 0,  0,  0,  5,  5,  0,  0,  0}
        };
    int[,] queenBlackPositionValues =
        {
            {-20,-10,-10, -5, -5,-10,-10,-20},
            {-10,  0,  0,  0,  0,  0,  0,-10},
            {-10,  0,  5,  5,  5,  5,  0,-10},
            { -5,  0,  5,  5,  5,  5,  0, -5},
            {  0,  0,  5,  5,  5,  5,  0, -5},
            {-10,  5,  5,  5,  5,  5,  0,-10},
            {-10,  0,  5,  0,  0,  0,  0,-10},
            {-20,-10,-10, -5, -5,-10,-10,-20}
        };
    int[,] kingMidBlackPositionValues =
        {
            {-30,-40,-40,-50,-50,-40,-40,-30},
            {-30,-40,-40,-50,-50,-40,-40,-30},
            {-30,-40,-40,-50,-50,-40,-40,-30},
            {-30,-40,-40,-50,-50,-40,-40,-30},
            {-20,-30,-30,-40,-40,-30,-30,-20},
            {-10,-20,-20,-20,-20,-20,-20,-10},
            { 20, 20,  0,  0,  0,  0, 20, 20},
            { 20, 30, 10,  0,  0, 10, 30, 20}
        };
    int[,] kingEndBlackPositionValues =
        {
            {-50,-40,-30,-20,-20,-30,-40,-50},
            {-30,-20,-10,  0,  0,-10,-20,-30},
            {-30,-10, 20, 30, 30, 20,-10,-30},
            {-30,-10, 30, 40, 40, 30,-10,-30},
            {-30,-10, 30, 40, 40, 30,-10,-30},
            {-30,-10, 20, 30, 30, 20,-10,-30},
            {-30,-30,  0,  0,  0,  0,-30,-30},
            {-50,-30,-30,-30,-30,-30,-30,-50}
        };
    public MyBot() {
        //Random random = new Random();
    }
    public Move Think(Board board, Timer timer)
    {
        Move[] allMoves = board.GetLegalMoves();
        Move moveToPlay = new Move();
        int bestEvaluation = -999999;

        foreach (Move move in allMoves)
        {
            board.MakeMove(move);
            int evaluation = -Search(board, 3, -99999999, 99999999); //hardcode 3 (2 per side plus one from previous line). this function is now recursive
            if (evaluation > bestEvaluation)
            {
                bestEvaluation = evaluation;
                moveToPlay = move;
            }
            board.UndoMove(move);
        }

        return moveToPlay;
    }

    int Search(Board board, int depth, int alpha, int beta)
    {
        // Alpha is the best choice for you we have found so far at any point along the path
        // Beta is the best choice for the opponent we have found so far at any point along the path
        if (depth == 0)
        {
            // reached the end of recursive loop, evaluate the position now
            return Evalute(board);
        }

        Move[] allMoves = board.GetLegalMoves();

        if (board.IsInCheckmate())
        {
            return -99999; // Current Player is in checkmate. That's bad
        }

        if (board.IsDraw())
        {
            return 0; // game over, draw
        }

        // int bestEvaluation = -9999999;

        // recursive loop from video
        foreach (Move move in allMoves)
        {
            board.MakeMove(move);
            int evaluation = -Search(board, depth - 1, -beta, -alpha); //negative because we're now evaluating for opponent and what's good for them is bad for us, swapping beta/alpha 
            board.UndoMove(move);
            //bestEvaluation = Math.Max(bestEvaluation, evaluation);
            if (evaluation >= beta)
            {
                // Move was too good, opponent will avoid this position
                return beta; // snip this tree
            }
            alpha = Math.Max(alpha, evaluation);
        }
        return alpha;
    }

    // Main method for evaluating the "score" of a position. Currently just material evaluation
    int Evalute(Board board)
    {
        PieceList[] arrayOfPieceLists = board.GetAllPieceLists();
        int materialEvaluation = GetAllPiecesMaterialValue(arrayOfPieceLists);
        int positionEvaluation = GetAllPiecesPositionValue(arrayOfPieceLists);
        int perspective = (board.IsWhiteToMove) ? 1 : -1;
        // positive good for current player moving/being evaluted

        return (materialEvaluation + positionEvaluation) * perspective;
    }


    // Uses pieceValue array and arrayOfPieceLists to calculate total piece value for a specific player. There's probably an efficient way to slim this down but that's for later
    int GetAllPiecesMaterialValue(PieceList[] arrayOfPieceLists)
    {
        int index = 0;
        int total = 0;
        int flip = 1;
        // Loop to get total piece value count
        // Pawns(white), Knights (white), Bishops (white), Rooks (white), Queens (white), King (white), Pawns (black), Knights (black), Bishops (black), Rooks (black), Queens (black), King (black).
        foreach (PieceList listOfPiece in arrayOfPieceLists)
        {
            total += listOfPiece.Count * pieceValues[index % 6] * flip;
            if (index == 5)
            {
                flip = -1; // After pieces 0-5, we're using black PieceList
            }
            index++;
        }
        return total;
    }

    int GetAllPiecesPositionValue(PieceList[] pieceLists)
    {
        int total = 0;
        // Pawn
        foreach (Piece piece in pieceLists[0]) // Add points for white's good positions
        {
            total += pawnBlackPositionValues[7 - piece.Square.File, piece.Square.Rank]; // Row, Column (currently assumes position values are symmetrical because it simplifies our calculation)
        }
        foreach (Piece piece in pieceLists[6]) // Subtract points for black's good positions
        {
            total += pawnBlackPositionValues[piece.Square.File, piece.Square.Rank] * -1; // Row, Column (currently assumes position values are symmetrical because it simplifies our calculation)
        }

        // Knight
        foreach (Piece piece in pieceLists[1]) // Add points for white's good positions
        {
            total += knightBlackPositionValues[7 - piece.Square.File, piece.Square.Rank]; // Row, Column (currently assumes position values are symmetrical because it simplifies our calculation)
        }
        foreach (Piece piece in pieceLists[7]) // Subtract points for black's good positions
        {
            total += knightBlackPositionValues[piece.Square.File, piece.Square.Rank] * -1; // Row, Column (currently assumes position values are symmetrical because it simplifies our calculation)
        }

        // Bishop
        foreach (Piece piece in pieceLists[2]) // Add points for white's good positions
        {
            total += bishopBlackPositionValues[7 - piece.Square.File, piece.Square.Rank]; // Row, Column (currently assumes position values are symmetrical because it simplifies our calculation)
        }
        foreach (Piece piece in pieceLists[8]) // Subtract points for black's good positions
        {
            total += bishopBlackPositionValues[piece.Square.File, piece.Square.Rank] * -1; // Row, Column (currently assumes position values are symmetrical because it simplifies our calculation)
        }

        // Rook
        foreach (Piece piece in pieceLists[3]) // Add points for white's good positions
        {
            total += rookBlackPositionValues[7 - piece.Square.File, piece.Square.Rank]; // Row, Column (currently assumes position values are symmetrical because it simplifies our calculation)
        }
        foreach (Piece piece in pieceLists[9]) // Subtract points for black's good positions
        {
            total += rookBlackPositionValues[piece.Square.File, piece.Square.Rank] * -1; // Row, Column (currently assumes position values are symmetrical because it simplifies our calculation)
        }

        // Queen
        foreach (Piece piece in pieceLists[4]) // Add points for white's good positions
        {
            total += queenBlackPositionValues[7 - piece.Square.File, piece.Square.Rank]; // Row, Column (currently assumes position values are symmetrical because it simplifies our calculation)
        }
        foreach (Piece piece in pieceLists[10]) // Subtract points for black's good positions
        {
            total += queenBlackPositionValues[piece.Square.File, piece.Square.Rank] * -1; // Row, Column (currently assumes position values are symmetrical because it simplifies our calculation)
        }

        // King
        int minorPieceCount = pieceLists[1].Count + pieceLists[2].Count + pieceLists[3].Count;
        if ((pieceLists[4].Count == 0 && pieceLists[10].Count == 0 && minorPieceCount <= 4) || (minorPieceCount <= 2)) 
        // Queens are off the board or two or fewer knight/bishop/rook, we're endgame now. Technically we could check black's pieces too but if the two players are mismatched by an entire minor piece, the game is probably over
        {
            foreach (Piece piece in pieceLists[5]) // Add points for white's good positions
            {
                total += kingEndBlackPositionValues[7 - piece.Square.File, piece.Square.Rank]; // Row, Column (currently assumes position values are symmetrical because it simplifies our calculation)
            }
            foreach (Piece piece in pieceLists[11]) // Subtract points for black's good positions
            {
                total += kingEndBlackPositionValues[piece.Square.File, piece.Square.Rank] * -1; // Row, Column (currently assumes position values are symmetrical because it simplifies our calculation)
            }
        }
        else
        {
            foreach (Piece piece in pieceLists[5]) // Add points for white's good positions
            {
                total += kingMidBlackPositionValues[7 - piece.Square.File, piece.Square.Rank]; // Row, Column (currently assumes position values are symmetrical because it simplifies our calculation)
            }
            foreach (Piece piece in pieceLists[11]) // Subtract points for black's good positions
            {
                total += kingMidBlackPositionValues[piece.Square.File, piece.Square.Rank] * -1; // Row, Column (currently assumes position values are symmetrical because it simplifies our calculation)
            }
        }
        return total;
    }


}