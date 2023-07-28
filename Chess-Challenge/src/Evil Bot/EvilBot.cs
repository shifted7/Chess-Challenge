﻿using ChessChallenge.API;
using System;

namespace ChessChallenge.Example
{
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
            {330, 340, 340, 340, 340, 340, 340, 330}
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
            int bestEvaluation = -999999; // MUST be lower than the evaluation returned by board.IsInCheckmate within search function. Otherwise computer in forced checkmate will return null

            foreach (Move move in allMoves)
            {
                board.MakeMove(move);
                int evaluation = -Search(board, 3, -99999999, 99999999); //hardcode 3 (2 per side plus one from previous line). this function is now recursive
                board.UndoMove(move);
                /*
                Console.Write(move.ToString());
                Console.Write("|");
                Console.WriteLine(evaluation);
                */
                if (evaluation > bestEvaluation)
                {
                    bestEvaluation = evaluation;
                    moveToPlay = move;
                }
            }
            /*
            Console.Write(moveToPlay.ToString());
            Console.Write("|");
            Console.WriteLine(bestEvaluation);
            Console.WriteLine("--------------------");
            */
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
            if (board.IsInCheckmate())
            {
                return -99999; // Current Player is in checkmate. That's bad
            }
            if (board.IsDraw())
            {
                return 0; // game over, draw
            }

            // int bestEvaluation = -9999999;
            Move[] allMoves = board.GetLegalMoves();
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
                /*
                if (evaluation > alpha)
                {
                    Console.Write("\t|");
                    Console.Write(move.ToString());
                    Console.Write("|");
                    Console.WriteLine(evaluation);
                }
                */
                alpha = Math.Max(alpha, evaluation);
            }
            return alpha;
        }

        // Main method for evaluating the "score" of a position.
        int Evalute(Board board)
        {
            PieceList[] arrayOfPieceLists = board.GetAllPieceLists();
            int positionEvaluation = GetAllPiecesPositionValue(arrayOfPieceLists);
            int perspective = (board.IsWhiteToMove) ? 1 : -1;
            // positive good for current player moving/being evaluted

            return positionEvaluation * perspective;
        }

        /* Integrated material values into GetAllPiecesPositionValue so this is no longer needed
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
        */

        int GetAllPiecesPositionValue(PieceList[] pieceLists)
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
    /*
    // A simple bot that can spot mate in one, and always captures the most valuable piece it can.
    // Plays randomly otherwise.
    public class EvilBot : IChessBot
    {
        // Piece values: null, pawn, knight, bishop, rook, queen, king
        int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };

        public Move Think(Board board, Timer timer)
        {
            Move[] allMoves = board.GetLegalMoves();

            // Pick a random move to play if nothing better is found
            Random rng = new();
            Move moveToPlay = allMoves[rng.Next(allMoves.Length)];
            int highestValueCapture = 0;

            foreach (Move move in allMoves)
            {
                // Always play checkmate in one
                if (MoveIsCheckmate(board, move))
                {
                    moveToPlay = move;
                    break;
                }

                // Find highest value capture
                Piece capturedPiece = board.GetPiece(move.TargetSquare);
                int capturedPieceValue = pieceValues[(int)capturedPiece.PieceType];

                if (capturedPieceValue > highestValueCapture)
                {
                    moveToPlay = move;
                    highestValueCapture = capturedPieceValue;
                }
            }

            return moveToPlay;
        }

        // Test if this move gives checkmate
        bool MoveIsCheckmate(Board board, Move move)
        {
            board.MakeMove(move);
            bool isMate = board.IsInCheckmate();
            board.UndoMove(move);
            return isMate;
        }
    }
    */
}

