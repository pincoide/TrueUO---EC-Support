using Microsoft.VisualBasic;
using Server.Gumps;
using Server.Network;
using System;
using System.Reflection;
using static Server.Config;

namespace Server.Prompts
{
	public abstract class Prompt
	{
		private readonly IEntity m_Sender;
		private readonly string m_MessageArgs;

		public IEntity Sender => m_Sender;

		public string MessageArgs => m_MessageArgs;

		public virtual int MessageCliloc => 1042971;

		public virtual int MessageHue => 0;

		public int TypeId;

		public Prompt()
			: this(null)
		{
            TypeId = GetType().FullName.GetHashCode();
        }

        public Prompt( int typeID )
            : this(null)
        {
            TypeId = typeID;
        }

        public Prompt(IEntity sender)
			: this(sender, string.Empty)
		{
            TypeId = GetType().FullName.GetHashCode();
        }

        public Prompt( IEntity sender, int typeID )
            : this( sender, string.Empty )
        {
            TypeId = typeID;
        }

        public Prompt(IEntity sender, string args)
		{
			m_Sender = sender;
			m_MessageArgs = args;
            TypeId = GetType().FullName.GetHashCode();
        }

        public virtual void OnCancel(Mobile from)
		{
		}

		public virtual void OnResponse(Mobile from, string text)
		{
		}
    }
}
