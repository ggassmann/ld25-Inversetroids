using System;
using System.Collections.Generic;
using System.Linq;
using CultLib;
using SFML.Graphics;
using SFML.Window;

namespace ld25
{
    class Ship : Entity
    {
        private float _accel;
        private Bolt _primary;
        private Bolt _secondary;
        private Bolt _teritiary;
        public static Ship Instance;
        private bool _shootDown;
        private int _selectedSlot = 1;
        private List<Vector2f> _path;
        private int _timer = 0;

        public Ship(Vector2f pos, World parent, bool add = true)
            : base(Rsc.Tex("rsc/Ship.png"), pos, parent, add)
        {
            Instance = this;
            Origin = Center;
            _path = Safety.CalcPath(new Vector2f(0, 0), new Vector2f(20, 10));
            Scale = new Vector2f(0.5f, 0.5f);
        }
        public override void Update()
        {
            base.Update();
            if (!Keyboard.IsKeyPressed(Keyboard.Key.Space))
            {
                _shootDown = false;
            }
            if(_shootDown == false && Keyboard.IsKeyPressed(Keyboard.Key.Space))
            {
                _shootDown = true;
                switch (_selectedSlot)
                {
                    case 1:
                        if (_primary != null)
                        {
                            _primary.Destroy();
                        }
                        _primary = new Bolt(Pos, Rotation, Parent);
                        _selectedSlot=2;
                        break;
                    case 2:
                        if (_secondary != null)
                        {
                            _secondary.Destroy();
                        }
                        _secondary = new Bolt(Pos, Rotation, Parent);
                        _selectedSlot=3;
                        break;
                    case 3:
                        if (_teritiary != null)
                        {
                            _teritiary.Destroy();
                        }
                        _teritiary = new Bolt(Pos, Rotation, Parent);
                        _selectedSlot=1;
                        break;
                }
            }
            var mult = 1;
            if (Keyboard.IsKeyPressed(Keyboard.Key.LShift))
            {
                mult = 2;
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
            {
                _accel++;
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                _accel-=0.5f;
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                Rotation += 2.5f * mult;
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                Rotation -= 2.5f * mult;
            }
            if(_accel > 35)
            {
                _accel = 35;
            }
            if(_accel < 0)
            {
                _accel = 0;
            }
            Pos.X += (_accel / 8f * (float)Math.Cos(MathHelper.ToRadians(Rotation - 90)));
            Pos.Y += (_accel / 8f * (float)Math.Sin(MathHelper.ToRadians(Rotation - 90)));
            /*
            _timer++;
            if(_path.Count == 0 || _timer == 20)
            {
                _timer = 0;
                var potentialpoints = new List<Vector2f>();
                var brake = false;
                for(var i = 0;; i++)
                {
                    for(var x = 0; x < 31; x++)
                    {
                        for(var y = 0; y < 31; y++)
                        {
                            if (Safety.Danger[x, y] != i) continue;
                            potentialpoints.Add(new Vector2f(x, y));
                            brake = true;
                        }
                    }
                    if(brake)
                    {
                        break;
                    }
                }
                _path = Safety.CalcPath(Pos/20, potentialpoints[Rand.RandInt(0, potentialpoints.Count - 1)]);
            }
            if (_path.Count > 0)
            {
                Pos = (Pos * 9 + _path[0] * 20) / 10f;
                if (Angle.DistanceBetween(Pos, _path[0]*20) < 10)
                {
                    _path.RemoveAt(0);
                }
            }
            */
            Wrap.WrapPos(ref Pos);
            Bounds = new FloatRect(Pos.X + 10 - Origin.X, Pos.Y + 10 - Origin.Y, Tex.Size.X - 20, Tex.Size.Y - 20);

        }
        private int FindClosest(IEnumerable<int> numbers, int x)
        {
            return
                numbers.Aggregate((r, n) => Math.Abs(r - x) > Math.Abs(n - x) ? n
                                         : Math.Abs(r - x) < Math.Abs(n - x) ? r
                                         : r < x ? n : r);
        }
    }
}
