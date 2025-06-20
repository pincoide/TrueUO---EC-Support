namespace Server.ContextMenus
{
	// this one is used for the player
	public class OpenBackpackEntry : ContextMenuEntry
	{
		private readonly Mobile m_Mobile;

		public OpenBackpackEntry(Mobile m)
			: base(6145, -1, 302)
		{
			m_Mobile = m;
		}

		public override void OnClick()
		{
			m_Mobile.Use(m_Mobile.Backpack);
		}
	}

	// this one is used for any mobiles that are not the current player
	public class OpenBackpackOthersEntry : ContextMenuEntry
	{
		private readonly Mobile m_Mobile;

		public OpenBackpackOthersEntry(Mobile m)
			: base(6145, -1, 508)
		{
			m_Mobile = m;
		}

		public override void OnClick()
		{
			m_Mobile.Use(m_Mobile.Backpack);
		}
	}
}