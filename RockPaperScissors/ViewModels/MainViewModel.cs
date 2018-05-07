using Newtonsoft.Json;
using RockPaperScissors.Data;
using RockPaperScissors.Logic;
using RockPaperScissors.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace RockPaperScissors.ViewModels
{
    public class MainViewModel : BindableBase
    {
        #region FIELDS
        int numberOfRounds = 3;
        string playerName = string.Empty;
        string versusText = string.Empty;
        string fightButtonText = string.Empty;
        bool player1MoveVisible;
        bool player2MoveVisible;
        bool fightButtonVisible;
        bool gameRunning = false;
        bool multiplayer = false;
        bool movesEnabled = true;
        Brush player1Color = Constants.IdleColor;
        Brush player2Color = Constants.IdleColor;
        List<Player> players;
        Move player1Move;
        Move player2Move;
        Game currentGame;
        #endregion
        #region PROPERTIES
        public int NumberOfRounds
        {
            get => numberOfRounds;
            set => SetField(ref numberOfRounds, value);
        }

        public string PlayerName
        {
            get => playerName;
            set => SetField(ref playerName, value);
        }

        public string VersusText
        {
            get => versusText;
            set => SetField(ref versusText, value);
        }
        public string FightButtonText
        {
            get => fightButtonText;
            set => SetField(ref fightButtonText, value);
        }

        public bool Player1MoveVisible
        {
            get => player1MoveVisible;
            set => SetField(ref player1MoveVisible, value);
        }

        public bool Player2MoveVisible
        {
            get => player2MoveVisible;
            set => SetField(ref player2MoveVisible, value);
        }

        public bool FightButtonVisible
        {
            get => fightButtonVisible;
            set => SetField(ref fightButtonVisible, value);
        }

        public bool GameRunning
        {
            get => gameRunning;
            set => SetField(ref gameRunning, value);
        }

        public bool Multiplayer
        {
            get => multiplayer;
            set => SetField(ref multiplayer, value);
        }
        
        public bool MovesEnabled
        {
            get => movesEnabled;
            set => SetField(ref movesEnabled, value);
        }

        public Brush Player1Color
        {
            get => player1Color;
            set => SetField(ref player1Color, value);
        }
        public Brush Player2Color
        {
            get => player2Color;
            set => SetField(ref player2Color, value);
        }

        public List<Player> Players
        {
            get => players;
            set => SetField(ref players, value);
        }
        public Move Player1Move
        {
            get => player1Move;
            set => SetField(ref player1Move, value);
        }

        public Move Player2Move
        {
            get => player2Move;
            set => SetField(ref player2Move, value);
        }

        public Game CurrentGame
        {
            get => currentGame;
            set => SetField(ref currentGame, value);
        }
        #endregion

        public MainViewModel()
        {
            CurrentGame = Game.Instance;
            LoadPlayers();
        }

        /// <summary>
        /// loads the available player types from the config file
        /// </summary>
        private void LoadPlayers()
        {
            try
            {
                Uri fileUri = new Uri(Constants.PlayerFileName);
                StorageFile jsonFile = StorageFile.GetFileFromApplicationUriAsync(fileUri).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
                string jsonText = FileIO.ReadTextAsync(jsonFile).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
                var playerList = JsonConvert.DeserializeObject<List<Player>>(jsonText);
                Players = playerList;
                CurrentGame.Player2 = Players.First();
            }
            catch(JsonException je)
            {
                Debug.WriteLine("config file corrupt" + je.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine("LoadPlayers exception" + e.Message);
            }
        }

        /// <summary>
        /// bound to the RPS radio button - sets the game mode to RPS
        /// </summary>
        public void RPSChecked()
        {
            CurrentGame.Mode = GameMode.RPS;
        }

        /// <summary>
        /// bound to the RPSLS radio button - sets the game mode to RPSLS
        /// </summary>
        public void RPSLSChecked()
        {
            CurrentGame.Mode = GameMode.RPSLS;
        }

        /// <summary>
        /// starts or resets the current game instance
        /// </summary>
        public void StartResetGame()
        {
            if (GameRunning)
            {
                //reset
                GameRunning = false;
                CurrentGame.Reset();
            }
            else
            {
                //start
                CurrentGame.Init(NumberOfRounds);
                CurrentGame.LoadMoves();
                GameRunning = true;
                VersusText = "pick a move";
                if (CurrentGame.Player2.Id == 0)
                {
                    Multiplayer = true;
                }
                else
                {
                    Multiplayer = false;
                }
            }
        }

        /// <summary>
        /// called when Player 1 selects a move
        /// </summary>
        public void Player1MoveChanged()
        {
            Player1MoveVisible = true;
            VersusText = "vs.";
            FightButtonText = "Fight";
            if (Player2Move != null)
            {
                FightButtonVisible = true;
            }
            if (CurrentGame.Player2.Id != 0)
            {
                FightButtonVisible = true;
            }
        }

        /// <summary>
        /// called when Player 1 selects a move
        /// </summary>
        public void Player2MoveChanged()
        {
            Player2MoveVisible = true;
            VersusText = "vs.";
            if (Player1Move != null)
            {
                FightButtonText = "Fight";
                FightButtonVisible = true;
            }
        }

        /// <summary>
        /// starts a new round in a match
        /// </summary>
        public void NextRound()
        {
            if (Player2Move == null)
            {
                Player2MoveVisible = true;
                Player2Move = CurrentGame.GetPlayer2Move();
                EvaluateRound();
                FightButtonText = "Next Round";
                if (CurrentGame.Player2.Id != 0)
                {
                    MovesEnabled = false;
                }
            }
            else
            {
                EvaluateRound();
                CurrentGame.SaveCurrentRound(Player1Move, Player2Move, Player1Color, Player2Color);
                ResetBoardUI();
                MovesEnabled = true;
                if (CurrentGame.Rounds.Count == NumberOfRounds)
                {
                    EvaluateGame();
                    GameRunning = false;
                    CurrentGame.Moves = null;
                }
            }
        }

        /// <summary>
        /// evaluates the current round and adapts the ui to show the winner
        /// </summary>
        void EvaluateRound()
        {
            int roundWinner = CurrentGame.GetRoundWinner(Player1Move, Player2Move);
            switch (roundWinner)
            {
                case 1:
                    //player1 wins
                    Player1Color = Constants.WinColor;
                    Player2Color = Constants.LooseColor;
                    break;
                case 2:
                    //player2 wins
                    Player1Color = Constants.LooseColor;
                    Player2Color = Constants.WinColor;
                    break;
                case 0:
                    //draw
                    Player1Color = Constants.DrawColor;
                    Player2Color = Constants.DrawColor;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// evaluates the current game and displays the winner
        /// </summary>
        void EvaluateGame()
        {
            int matchWinner = CurrentGame.GetMatchWinner();
            if (matchWinner != int.MaxValue)
            {
                switch (matchWinner)
                {
                    case 1:
                        //player1 wins
                        VersusText = $"{PlayerName} Wins 🎉";
                        break;
                    case 2:
                        //player2 wins
                        VersusText = "Player 2 Wins 🎉";
                        break;
                    case 0:
                        //draw
                        VersusText = "It's a draw 🙌 Start a new Game";
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Game.Instance.Reset();
            }
        }

        /// <summary>
        /// resets the UI for a new round / match
        /// </summary>
        void ResetBoardUI()
        {
            Player1Color = Constants.IdleColor;
            Player2Color = Constants.IdleColor;
            Player1Move = null;
            Player2Move = null;
            FightButtonVisible = false;
            Player1MoveVisible = false;
            Player2MoveVisible = false;
        }

    }
}