using System;
using CultLib;
using SFML.Graphics;
using SFML.Window;

namespace ld25
{
    class LevelManager : Entity
    {
        private int _level;
        public int Level
        {
            get { return _level; }
            set
            {
                Alert = new TextEntity("Level " + value + "!", new Vector2f(83, 63), Ld25.UserInterface, Fonts.ExoRegular, 30, Color.White, Color.Black);
                DangerManager.Instance.MaxDP += 25;
                AlertTicker = new Ticker(Parent, new TimeSpan(0, 0, 1), DestroyAlert);
                ChangeLevel.Interval.Add(new TimeSpan(0, 0, 0, 25));
                _level = value;
            }
        }
        public TextEntity Alert;
        public Ticker AlertTicker;
        public Ticker ChangeLevel;
        public LevelManager(World parent, bool add = true) : base(null, new Vector2f(0, 0), parent, add)
        {
            ChangeLevel = new Ticker(parent, new TimeSpan(0, 0, 0, (int)(DangerManager.Instance.MaxDP / 2f)), NextLevel);
            Level = 1;
        }

        private void NextLevel()
        {
            Level = Level + 1;
        }

        void DestroyAlert()
        {
            Alert.Destroy();
            AlertTicker.Destroy();
        }
    }
}
