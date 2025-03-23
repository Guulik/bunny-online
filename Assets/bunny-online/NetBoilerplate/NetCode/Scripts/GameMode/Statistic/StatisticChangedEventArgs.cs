using System;

namespace GameMode
{
    public class StatisticChangedEventArgs : EventArgs
    {
        public StatisticChangedEventArgs(Player player, int score)
        {
            Player = player;
            Score = score;
        }
        public Player Player { get; }
        public int Score { get; }
    }
}