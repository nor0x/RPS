using Newtonsoft.Json;
using RockPaperScissors.Data;
using RockPaperScissors.Logic;
using RockPaperScissors.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        List<Player> players;
        List<Move> moves;
        ObservableCollection<Round> rounds;
        Move player1Move;
        Move player2Move;
        Player player2;
        string playerName = string.Empty;
        string versusText = string.Empty;
        string fightButtonText = string.Empty;
        int numberOfRounds = 3;
        bool gameRunning = false;
        bool multiplayer = false;
        bool player1MoveVisible;
        bool player2MoveVisible;
        bool fightButtonVisible;

        Brush player1Color = Constants.IdleColor;
        Brush player2Color = Constants.IdleColor;
        RoundResult result;

        enum GameMode
        {
            RPS,
            RPSLS
        };
        GameMode mode;
        public List<Player> Players
        {
            get => players;
            set => SetField(ref players, value);
        }

        public List<Move> Moves
        {
            get => moves;
            set => SetField(ref moves, value);
        }

        public ObservableCollection<Round> Rounds
        {
            get => rounds;
            set => SetField(ref rounds, value);
        }

        public string PlayerName
        {
            get => playerName;
            set => SetField(ref playerName, value);
        }

        public string FightButtonText
        {
            get => fightButtonText;
            set => SetField(ref fightButtonText, value);
        }

        public string VersusText
        {
            get => versusText;
            set => SetField(ref versusText, value);
        }

        public int NumberOfRounds
        {
            get => numberOfRounds;
            set => SetField(ref numberOfRounds, value);
        }

        public bool GameRunning
        {
            get => gameRunning;
            set => SetField(ref gameRunning, value);
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

        public bool Multiplayer
        {
            get => multiplayer;
            set => SetField(ref multiplayer, value);
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

        public Player Player2
        {
            get => player2;
            set => SetField(ref player2, value);
        }

        public MainViewModel()
        {
            LoadPlayers();
        }

        private void LoadPlayers()
        {
            Uri fileUri = new Uri(Constants.PlayerFileName);
            StorageFile jsonFile = StorageFile.GetFileFromApplicationUriAsync(fileUri).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            string jsonText = FileIO.ReadTextAsync(jsonFile).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            var playerList = JsonConvert.DeserializeObject<List<Player>>(jsonText);
            Players = playerList;
            Player2 = Players.First();
        }
        private void LoadMoves()
        {
            Uri fileUri;
            if (mode == GameMode.RPS)
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

        public void RPSChecked()
        {
            mode = GameMode.RPS;
        }

        public void RPSLSChecked()
        {
            mode = GameMode.RPSLS;
        }

        public void StartResetGame()
        {
            if (GameRunning)
            {
                //reset
                GameRunning = false;
                Rounds = null;
                Moves = null;
            }
            else
            {
                //start
                LoadMoves();
                GameRunning = true;
                Rounds = new ObservableCollection<Round>();
                VersusText = "pick a move";
                if (Player2.Id == 0)
                {
                    Multiplayer = true;
                }
                else
                {
                    Multiplayer = false;
                }
            }
        }

        public void Player1MoveChanged()
        {
            Player1MoveVisible = true;
            VersusText = "vs.";
            FightButtonText = "Fight";
            if (Player2Move != null)
            {
                FightButtonVisible = true;
            }
            if (Player2.Id != 0)
            {
                FightButtonVisible = true;
            }
        }

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

        public void NextRound()
        {
            //VersusText = "vs.";
            if (Player2Move == null)
            {
                GetPlayer2Move();
                CheckMoves();
                FightButtonText = "Next Round";
            }
            else
            {
                CheckMoves();
                Round currentRound = new Round();
                currentRound.Player1Color = Player1Color;
                currentRound.Player2Color = Player2Color;
                currentRound.Player1Move = Player1Move;
                currentRound.Player2Move = Player2Move;
                currentRound.Result = result;
                Rounds.Add(currentRound);
                Player1Color = Constants.IdleColor;
                Player2Color = Constants.IdleColor;
                Player1Move = null;
                Player2Move = null;

                if (Rounds.Count == NumberOfRounds)
                {
                    Player1Color = Constants.IdleColor;
                    Player2Color = Constants.IdleColor;
                    //check if draw
                    int player1Wins = Rounds.Where(r => r.Result == RoundResult.Player1).Count();
                    int player2Wins = Rounds.Where(r => r.Result == RoundResult.Player2).Count();
                    int draws = Rounds.Where(r => r.Result == RoundResult.Player2).Count();
                    if (player1Wins > player2Wins)
                    {
                        VersusText = $"{PlayerName} Wins 🎉";
                    }
                    else if (player1Wins < player2Wins)
                    {
                        VersusText = "Player 2 Wins 🎉";
                    }
                    else if (player1Wins == player2Wins || draws == NumberOfRounds)
                    {
                        VersusText = "It's a draw 🙌 Start a new Game";
                    }
                    GameRunning = false;
                    Moves = null;
                }
                FightButtonVisible = false;
                Player1MoveVisible = false;
                Player2MoveVisible = false;
            }

        }

        void GetPlayer2Move()
        {
            Player2MoveVisible = true;
            Random r = new Random();
            switch (Player2.Id)
            {
                case 0:
                    break;

                case 1:
                    Player2Move = Moves.ElementAt(r.Next(0, Moves.Count));
                    break;

                case 2:
                    if (Rounds.Count != 0)
                    {
                        var previousRound = Rounds.LastOrDefault();
                        var previousMove = previousRound.Player2Move;
                        Player2Move = Moves.FirstOrDefault(m => m.Beats.Contains(previousMove.Id));
                    }
                    else
                    {
                        Player2Move = Moves.ElementAt(r.Next(0, Moves.Count));
                    }
                    break;
            }
        }

        void CheckMoves()
        {
            if (Player1Move.Beats.Contains(Player2Move.Id))
            {
                //player1 wins
                Player1Color = Constants.WinColor;
                Player2Color = Constants.LooseColor;
                result = RoundResult.Player1;
            }
            else if (Player2Move.Beats.Contains(Player1Move.Id))
            {
                //player2 wins
                Player1Color = Constants.LooseColor;
                Player2Color = Constants.WinColor;
                result = RoundResult.Player2;
            }
            else
            {
                //draw
                Player1Color = Constants.DrawColor;
                Player2Color = Constants.DrawColor;
                result = RoundResult.Draw;
            }
        }
    }
}