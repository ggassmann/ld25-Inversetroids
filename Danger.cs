using CultLib;
using SFML.Graphics;
using SFML.Window;

namespace ld25
{
    class Danger : Entity
    {
        public int Value { get; set; }

        public Danger(Texture tex, Vector2f pos, World parent, int value, bool add = true) : base(tex, pos, parent, add)
        {
            Value = value;
            if(DangerManager.Instance.UsedDP > DangerManager.Instance.MaxDP)
            {
                Destroy();
            }
        }
    }
}
