using ChessChallenge.API;


namespace Chess_Challenge_Tests
{
    public class MyBotUnitTests
    {
        [Fact]
        public void TestCanCreateMyBot()
        {
            // Arrange
            // Act
            MyBot bot = new MyBot();
            // Assert
            Assert.NotNull(bot);
        }
        [Fact]
        public void TestCanEvaluateCorrectStartingBoardValueForAllPieces()
        {
            // Arrange
            MyBot bot = new MyBot();
            Board startBoard = Board.CreateBoardFromFEN(ChessChallenge.Chess.FenUtility.StartPositionFEN);

            // Act
            int value = bot.Evaluate(startBoard);

            // Assert
            Assert.Equal(0, value);
        }
        [Fact]
        public void TestCanEvaluateCorrectBoardValueForFirstMoveKingPawn()
        {
            // Arrange
            MyBot bot = new MyBot();
            Board board = Board.CreateBoardFromFEN(ChessChallenge.Chess.FenUtility.StartPositionFEN);
            Move move = new Move("e2e4", board);
            board.MakeMove(move);
            // Act
            int value = bot.Evaluate(board);
            // Assert
            Assert.Equal(-40, value);
        }

        [Fact]
        public void TestCanEvaluateCorrectBoardValueForKnightMove()
        {
            // Arrange
            MyBot bot = new MyBot();
            Board board = Board.CreateBoardFromFEN(ChessChallenge.Chess.FenUtility.StartPositionFEN);
            Move[] moves = { 
                new Move("e2e4", board), 
                new Move("b8c6", board),
            };
            foreach (Move move in moves)
            {
                board.MakeMove(move);
            }
            // Act
            int value = bot.Evaluate(board);
            // Assert
            Assert.Equal(10, value);
        }
    }
}