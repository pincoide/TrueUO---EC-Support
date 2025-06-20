using Server.Items;
using Server.Prompts;

namespace Server.Multis
{
    public class RenameBoatPrompt : Prompt
    {
        public override int MessageCliloc => 502580; // What dost thou wish to name thy ship?

        private readonly BaseBoat m_Boat;

        public RenameBoatPrompt(BaseBoat boat)
            : base( (IEntity)( boat.Pilot ?? boat.TillerMan ) , 24 )
        {
            m_Boat = boat;
        }

        public override void OnResponse(Mobile from, string text)
        {
            m_Boat.EndRename(from, text);
        }
    }
}
