using CultLib;
using SFML.Graphics;
using SFML.Window;

namespace ld25
{
    class SmallAsteroidSpawner : SpawnButton
    {
        public SmallAsteroidSpawner(World parent, bool add = true)
            : base(parent, new Vector2f(648, 5), "rsc/AsteroidSmall.png", 4, add)
        {
            Origin = new Vector2f(-48, -48);
        }
        public override void Throw(Thrower thrower)
        {
            new Asteroid("Small", thrower.Rotation, thrower.Base, Ld25.Game, 4, true);
        }
    }
    class MediumAsteroidSpawner : SpawnButton
    {
        public MediumAsteroidSpawner(World parent, bool add = true)
            : base(parent, new Vector2f(648 + 70, 5), "rsc/AsteroidMedium.png", 10, add)
        {
            Origin = new Vector2f(-32, -32);
        }
        public override void Throw(Thrower thrower)
        {
            new Asteroid("Medium", thrower.Rotation, thrower.Base, Ld25.Game, 10, true);
        }
    }
    class LargeAsteroidSpawner : SpawnButton
    {
        public LargeAsteroidSpawner(World parent, bool add = true)
            : base(parent, new Vector2f(648, 5 + 70), "rsc/AsteroidLarge.png", 25, add)
        {
            Origin = new Vector2f(-32, -32);
        }
    }
    class GiantAsteroidSpawner : SpawnButton
    {
        public GiantAsteroidSpawner(World parent, bool add = true)
            : base(parent, new Vector2f(648 + 70, 5 + 70), "rsc/AsteroidGiant.png", 60, add)
        {
            Origin = new Vector2f(-3, -2);
        }
        public override void Throw(Thrower thrower)
        {
            new Asteroid("Giant", thrower.Rotation, thrower.Base, Ld25.Game, 60, true);
        }
    }

    class UFOSpawner : SpawnButton
    {
        public UFOSpawner(World parent, bool add = true)
            : base(parent, new Vector2f(648, 5 + 140), "rsc/UFO.png", 80, add)
        {
            Origin = new Vector2f(-32, -32);
        }

        public override void Throw(Thrower thrower)
        {
            new UFO(thrower.Base, thrower.Rotation, Ld25.Game);
        }
    }
}
