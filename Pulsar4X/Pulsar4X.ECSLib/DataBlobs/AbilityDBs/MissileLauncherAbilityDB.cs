using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public class MissileLauncherAbilityDB : BaseDataBlob
    {
        [JsonProperty]
        public double MissileSize { get; internal set; }

        [JsonProperty]
        public double ReloadRate { get; internal set; }

        [JsonConstructor]
        public MissileLauncherAbilityDB(double missileSize = 0, double reloadRate = 0)
        {
            MissileSize = missileSize;
            ReloadRate = reloadRate;
        }

        public override object Clone()
        {
            return new MissileLauncherAbilityDB(MissileSize, ReloadRate);
        }
    }
}