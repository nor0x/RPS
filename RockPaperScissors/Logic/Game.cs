using Newtonsoft.Json;
using RockPaperScissors.Data;
using RockPaperScissors.Models;
using RockPaperScissors.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media;

namespace RockPaperScissors.Logic
{
    public class Game : BindableBase
    {
        #region SINGLETON
        private static Game instance;
        private Game() { }
        public static Game Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Game();
                }
                return instance;
            }
        }
        #endregion
        #region FIELDS
        int numberOfRounds;
        RoundResult result;
        List<Move> moves;
        #endregion
        #region PROPERTIES

        public GameMode Mode { get; set; }
        public ObservableCollection<Round> Rounds
        {
            get => rounds;
            set => SetField(ref rounds, value);
        }
        ObservableCollection<Round> rounds;
        public List<Move> Moves
        {
            get => moves;
            set => SetField(ref moves, value);
        }
        public Player Player2 { get; set; }
        #endregion
        /// <summary>
        /// initializes a new game with a number of rounds
        /// </summary>
        /// <param name="rounds">number of rounds to play</param>
        public void Init(int rounds)
        {
            Rounds = new ObservableCollection<Round>();
            numberOfRounds = rounds;
        }

        /// <summary>
        /// clears the rounds of the current game
        /// </summary>
        public void Reset()
        {
            Rounds = null;
        }

        /// <summary>
        /// loads the available moves based on the current game mode
        /// </summary>
        public void LoadMoves()
        {
            try
            {
                Uri fileUri;
                if (Mode == GameMode.RPS)
                {
                    fileUri = new Uri(Constants.RPSMovesFileName);
                }
                else
                {
                    fileUri = new Uri(Constants.RPSLSMovesFileName);
                }
                StorageFile jsonFile = StorageFile.GetFileFromApplicationUriAsync(fileUri).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
                string jsonText = FileIO.ReadTextAsync(jsonFile).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
                var moveList = JsonConvert.DeserializeObject<List<Move>>(jsonText);
                Moves = moveList;
            }
            catch (JsonException je)
            {
                Debug.WriteLine("config file corrupt" + je.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine("LoadMoves exception" + e.Message);
            }
        }

        /// <summary>
        /// saves the current round
        /// </summary>
        /// <param name="player1Move">Move of Player 1</param>
        /// <param name="player2Move">Move of Player 2</param>
        /// <param name="player1Color">Color of Player 1</param>
        /// <param name="player2Color">Color of Player 2</param>
        public void SaveCurrentRound(Move player1Move, Move player2Move, Brush player1Color, Brush player2Color)
        {
            Round currentRound = new Round();
            currentRound.Player1Color = player1Color;
            currentRound.Player2Color = player2Color;
            currentRound.Player1Move = player1Move;
            currentRound.Player2Move = player2Move;
            currentRound.Result = result;
            Rounds.Add(currentRound);
        }

        /// <summary>
        /// Gets a Move for a computer based player. Based on Player.Id a random or a tactical Move is returned
        /// </summary>
        /// <returns>Move for computer player</returns>
        public Move GetPlayer2Move()
        {
            Random r = new Random();
            switch (Player2.Id)
            {
                case 0:
                    break;

                case 1:
                    return Moves.ElementAt(r.Next(0, Moves.Count));

                case 2:
                    if (Rounds.Count != 0)
                    {
                        var previousRound = Rounds.LastOrDefault();
                        var previousMove = previousRound.Player2Move;
                        return Moves.FirstOrDefault(m => m.Beats.Contains(previousMove.Id));
                    }
                    else
                    {
                        return Moves.ElementAt(r.Next(0, Moves.Count));
                    }
            }
            return null;
        }

        /// <summary>
        /// evaluates the winner of a round
        /// </summary>
        /// <param name="player1Move">Move of Player 1</param>
        /// <param name="player2Move">Move of Player 2</param>
        /// <returns>0 = draw, 1 = player 1, 2 = player 2</returns>
        public int GetRoundWinner(Move player1Move, Move player2Move)
        {
            if (player1Move.Beats.Contains(player2Move.Id))
            {
                //player1 wins
                result = RoundResult.Player1;
                return 1;
            }
            else if (player2Move.Beats.Contains(player1Move.Id))
            {
                //player2 wins
                result = RoundResult.Player2;
                return 2;
            }
            else
            {
                //draw
                result = RoundResult.Draw;
                return 0;
            }
        }

        /// <summary>
        /// evaluates the winner of a match
        /// </summary>
        public int GetMatchWinner()
        {
            int player1Wins = Rounds.Where(r => r.Result == RoundResult.Player1).Count();
            int player2Wins = Rounds.Where(r => r.Result == RoundResult.Player2).Count();
            int draws = Rounds.Where(r => r.Result == RoundResult.Player2).Count();
            if (player1Wins > player2Wins)
            {
                return 1;
            }
            else if (player1Wins < player2Wins)
            {
                return 2;
            }
            else if (player1Wins == player2Wins || draws == numberOfRounds)
            {
                return 0;
            }
            return int.MaxValue;
        }
    }
}
