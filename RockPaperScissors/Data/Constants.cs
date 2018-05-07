using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace RockPaperScissors.Data
{
    public static class Constants
    {
        public static string PlayerFileName = "ms-appx:///Data/Players.json";
        public static string RPSMovesFileName = "ms-appx:///Data/RPS.json";
        public static string RPSLSMovesFileName = "ms-appx:///Data/RPSLS.json";
        public static Brush DrawColor = new SolidColorBrush(Colors.LightBlue);
        public static Brush WinColor = new SolidColorBrush(Colors.Green);
        public static Brush LooseColor = new SolidColorBrush(Colors.Red);
        public static Brush IdleColor = new SolidColorBrush(Colors.White);

    }
    public enum GameMode
    {
        RPS,
        RPSLS
    };
}
