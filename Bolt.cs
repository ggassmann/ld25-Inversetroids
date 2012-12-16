using System;
using CultLib;
using SFML.Window;

namespace ld25
{
    class Bolt : Entity
    {
        private bool _start;
        private bool _exit;
        public Bolt(Vector2f pos, float rotation, World parent, bool add = true) : base(Rsc.Tex("rsc/Bolt.png"), pos, parent, add)
        {
            Rotation = rotation;
            Origin = Center;
            var ticker = new Ticker(Parent, new TimeSpan(0,0,0,2), null);
            ticker.OnTick = delegate
                                {
                                    _start = true;
                                    ticker.Destroy();
                                };
        }
        public override void Update()
        {
            base.Update();
            if (_start)
            {
                Col.A -= 15;
            }
            Pos += Angle.GetStep(Rotation)*10;
            Wrap.WrapPos(ref Pos);
            if(Col.A <=0)
            {
                Destroy();
            }
            for (var i = 0; i < Parent.Entities.Count; i++)
            {
                var e = Parent.Entities[i];
                if(e.GetType() == typeof(UFO))
                {
                    var f = e as UFO;
                    if (Angle.DistanceBetween(Pos, f.Pos) < f.Tex.Size.X*0.6f)
                    {
                        Destroy();
                        f.Destroy();
                    }
                }
                if (e.GetType() != typeof (Asteroid)) continue;
                var r = e as Asteroid;
                if (Angle.DistanceBetween(Pos, r.Pos) < r.Tex.Size.X*0.6f)
                {
                    Destroy();
                    var x = r.MoveRot + Rand.RandInt(-45, 45) + 360;
                    switch (r.Size)
                    {
                        case "Medium":
                            new Asteroid("Small", Rand.RandInt(0, 360), r.Pos, r.Parent, 4);
                            new Asteroid("Small", Rand.RandInt(0, 360), r.Pos, r.Parent, 4);
                            break;
                        case "Large":
                            new Asteroid("Medium", x, r.Pos, r.Parent, 10);
                            if (Rand.RandBool())
                            {
                                new Asteroid("Medium", x + Rand.RandInt(10, 45) + 360, r.Pos, r.Parent, 10);
                            }
                            else
                            {
                                new Asteroid("Medium", x - Rand.RandInt(10, 45) + 360, r.Pos, r.Parent, 10);
                            }
                            break;
                        case "Giant":
                            new Asteroid("Large", x, r.Pos, r.Parent, 10);
                            if(Rand.RandBool())
                            {
                                new Asteroid("Large", x + Rand.RandInt(30, 45) + 360, r.Pos, r.Parent, 25);
                            }
                            else
                            {
                                new Asteroid("Large", x - Rand.RandInt(30, 45) + 360, r.Pos, r.Parent, 25);
                            }
                            break;
                    }
                    if (!_exit)
                    {
                        r.Destroy();
                        _exit = true;
                    }
                    break;
                }
            }
        }
    }
}
