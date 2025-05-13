using Godot;
using System;
using MAPZ_lab_RPG.Entities.Heroes;

namespace Game{
    public class GameManager{
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameManager(MainHero.Instance);
                }
                return _instance;
            }
        }

        private GameManager(MainHero hero){
            this.hero = hero;
        }

        public void StartGame() {
        }

        public MainHero hero;
    }
}
