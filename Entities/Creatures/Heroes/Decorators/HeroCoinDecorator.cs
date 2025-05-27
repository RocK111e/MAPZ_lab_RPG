
namespace MAPZ_lab_RPG.Entities.Heroes.Decorators
{
    public class HeroCoinDecorator : HeroBaseDecorator
    {
        private readonly double _multiplier;

        public HeroCoinDecorator(IHero hero, double multiplier) : base(hero)
        {
            _multiplier = multiplier;
        }

        public override void EarnRoundRewards(int coins, int expirience)
        {
            int coinsMultiplied = (int)(coins * 1.5);
            _hero.EarnRoundRewards(coinsMultiplied, expirience);
        }
    }
}