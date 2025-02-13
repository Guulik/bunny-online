using System;

namespace GameMode
{
    public class StatisticChangedEventArgs : EventArgs
    {
        public StatisticChangedEventArgs(Player player, Statistic statistic)
        {
            Player = player;
            Statistic = statistic;
        }
        public Player Player { get; }
        public Statistic Statistic { get; }
    }
}