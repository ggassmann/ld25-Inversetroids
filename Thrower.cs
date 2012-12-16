using System;
using CultLib;
using CultLib.Input;
using SFML.Graphics;
using SFML.Window;

namespace ld25
{
    class Thrower : Entity
    {
        public Vector2f Base = new Vector2f(559, 10);
        private bool _visible;
        private bool _disabled;
        public Thrower(Vector2f pos, World parent, bool add = true) : base(Rsc.Tex("rsc/Throw.png"), pos, parent, add)
        {
            Origin = new Vector2f(32, 1);
            _visible = false;
            Col.A = 100;
        }
        public override void Update()
        {
            base.Update();
            Pos = MouseManager.Pos;
            if (Pos.X < 640 && (Base.X > 558 || Base.Y < 62 || Base.X < 82 || Base.Y > 540) && !_disabled && SpawnButton.Selected!=null)
            {
                Rotation = (float)Angle.AngleBetween(Base, Pos);
                if (_visible == false && MouseManager.LeftDown)
                {
                    Base = Pos;
                }
                if (_visible && !MouseManager.LeftDown)
                {
                    SpawnButton.Selected.Throw(this);
                }
                _visible = MouseManager.LeftDown;
            }
            else
            {
                if (Pos.X > 640 && MouseManager.LeftDown)
                {
                    _disabled = true;
                }
                _visible = false;
            }
            if (MouseManager.LeftDown == false)
            {
                _disabled = false;
            }
            if (Pos.X < 640 && (Base.X > 558 || Base.Y < 62 || Base.X < 82 || Base.Y > 540) && !_disabled && SpawnButton.Selected != null) return;
            _visible = false;
            Base = new Vector2f(559, 10);
        }
        public override void Draw()
        {
            if (!_visible) return;
            var d = new Vector2f(Math.Abs(Base.X - Pos.X), Math.Abs(Base.Y - Pos.Y));

            var sx = Pos.X < Base.X ? 1 : -1;
            var sy = Pos.Y < Base.Y ? 1 : -1;

            var worker = new Vector2f(Pos.X, Pos.Y);

            var err = d.X - d.Y;
            while (!(worker.X == Base.X && worker.Y == Base.Y))
            {
                Parent.Parent.Window.Draw(new Sprite(Rsc.Tex("rsc/ThrowLine.png"))
                                              {
                                                  Position = worker,
                                                  Rotation = Rotation + 90,
                                                  Origin = new Vector2f(2.5f, 2.5f),
                                                  Color = new Color(255,255,255,100)
                                              });
                var e2 = 2*err;
                if (e2 > -d.Y)
                {
                    err -= d.Y;
                    worker.X += sx;
                }
                if (!(e2 < d.X)) continue;
                err += d.X;
                worker.Y += sy;
            }
            base.Draw();
        }
    }
}
