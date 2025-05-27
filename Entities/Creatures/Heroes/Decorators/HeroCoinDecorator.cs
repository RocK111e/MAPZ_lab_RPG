
namespace MAPZ_lab_RPG.Entities.Heroes.Decorators
{
    public class HeroCoinDecorator : HeroBaseDecorator
    {
        private readonly double _multiplier;

        public HeroCoinDecorator(IHero hero, double multiplier) : base(hero)
        {
            _multiplier = multiplier;
        }

        public override int AddCoins(int coins)
        {
            int modifiedCoins = (int)(coins * _multiplier);
            return base.AddCoins(modifiedCoins);
        }
    }
}