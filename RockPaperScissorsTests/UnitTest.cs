
using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RockPaperScissors.Data;
using RockPaperScissors.Logic;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace RockPaperScissorsTests
{
    [TestClass]
    public class GameLogicTests
    {
        [TestMethod]
        public async Task RPSLogicAsync()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Game game = Game.Instance;
                game.Init(4);
                game.Mode = GameMode.RPS;
                game.LoadMoves();


                var rock = game.Moves.Find(m => m.Name.Equals("Rock"));
                var paper = game.Moves.Find(m => m.Name.Equals("Paper"));
                var scissors = game.Moves.Find(m => m.Name.Equals("Scissors"));

                Assert.IsTrue(game.GetRoundWinner(rock, scissors) == 1);
                Assert.IsTrue(game.GetRoundWinner(paper, rock) == 1);
                Assert.IsTrue(game.GetRoundWinner(scissors, paper) == 1);
                Assert.IsTrue(game.GetRoundWinner(rock, rock) == 0);
                Assert.IsTrue(game.GetRoundWinner(paper, paper) == 0);
                Assert.IsTrue(game.GetRoundWinner(scissors, scissors) == 0);

                game.SaveCurrentRound(rock, paper, Constants.LooseColor, Constants.WinColor);
                game.SaveCurrentRound(scissors, paper, Constants.WinColor, Constants.LooseColor);
                game.SaveCurrentRound(paper, paper, Constants.DrawColor, Constants.DrawColor);
                game.SaveCurrentRound(paper, rock, Constants.WinColor, Constants.LooseColor);
                Assert.IsTrue(game.GetMatchWinner() == 1);
            }
            );

        }

        [TestMethod]
        public async Task RPSLSLogicAsync()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Game game = Game.Instance;
                game.Init(3);
                game.Mode = GameMode.RPSLS;
                game.LoadMoves();

                var rock = game.Moves.Find(m => m.Name.Equals("Rock"));
                var paper = game.Moves.Find(m => m.Name.Equals("Paper"));
                var scissors = game.Moves.Find(m => m.Name.Equals("Scissors"));
                var lizard = game.Moves.Find(m => m.Name.Equals("Lizard"));
                var spock = game.Moves.Find(m => m.Name.Equals("Spock"));

                Assert.IsTrue(game.GetRoundWinner(rock, scissors) == 1);
                Assert.IsTrue(game.GetRoundWinner(paper, rock) == 1);
                Assert.IsTrue(game.GetRoundWinner(scissors, paper) == 1);
                Assert.IsTrue(game.GetRoundWinner(rock, rock) == 0);
                Assert.IsTrue(game.GetRoundWinner(paper, paper) == 0);
                Assert.IsTrue(game.GetRoundWinner(scissors, scissors) == 0);
                Assert.IsTrue(game.GetRoundWinner(lizard, lizard) == 0);
                Assert.IsTrue(game.GetRoundWinner(spock, spock) == 0);
            }
);
        }
    }
}
