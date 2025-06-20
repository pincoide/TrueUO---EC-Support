using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class ShipCrate : LargeCrate
    {
        public static readonly int DT = 30;

        private Mobile m_Owner;
        private BaseBoat m_Boat;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner => m_Owner;

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseBoat Boat => m_Boat;

        public override TimeSpan DecayTime => TimeSpan.FromMinutes(DT);

        public override bool Decays => true;

        public ShipCrate(Mobile owner, BaseBoat boat)
        {
            LiftOverride = true;
            m_Owner = owner;
            m_Boat = boat;
            Movable = false;
        }

        public override void Delete()
        {
            Mobiles.DockMaster.RemoveCrate(this);
            base.Delete();
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (m_Owner != null)
                list.Add(1116515, m_Owner.Name);
            else
                list.Add("a shipping crate");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from != m_Owner)
            {
                from.SendLocalizedMessage(1112589); //This does not belong to you! Find your own!
                return;
            }

            base.OnDoubleClick(from);
        }

        public override void OnItemRemoved(Item item)
        {
            base.OnItemRemoved(item);

            if (TotalItems == 0)
                Delete();
        }

        public ShipCrate(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(m_Owner);
            writer.Write(m_Boat);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            m_Owner = reader.ReadMobile();
            m_Boat = reader.ReadItem() as BaseBoat;
        }
    }
}
