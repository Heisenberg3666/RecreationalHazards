using Exiled.API.Interfaces;
using RecreationalHazards.API.Items;

namespace RecreationalHazards
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public Alcohol Alcohol { get; set; } = new Alcohol();
        public Heroin Heroin { get; set; } = new Heroin();
        public Steroids Steroids { get; set; } = new Steroids();
        public CrystalMeth CrystalMeth { get; set; } = new CrystalMeth();
        public LSD LSD { get; set; } = new LSD();
    }
}
