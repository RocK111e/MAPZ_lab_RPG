using System;
using Godot;

namespace MAPZ_lab_RPG.Entities.Creatures.Heroes.States
{
    public class HeroStateManager
    {
        private readonly Random _random = new Random();

        public IHeroState GenerateRandomState()
        {
            GD.Print("Generating random hero state...");
            double roll = _random.NextDouble();
            if (roll < 0.25) return new WeakState();
            if (roll < 0.75) return new NormalState();
            return new StrongState();
        }

        public IHeroState GetNormalState() => new NormalState();
    }
}