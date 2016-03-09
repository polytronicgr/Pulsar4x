namespace Pulsar4X.ECSLib
{
    public enum CargoType
    {
        None = 0,
        General, // Used for general freight. Trade Goods, Minerals, Refined Minerals, etc.
        Troops,
        Ordnance,
        Colonists,
        Fuel,
    }

    public class CargoDefinition
    {
        public CargoType Type { get; internal set; }
        public float Weight { get; internal set; }
    }
}
