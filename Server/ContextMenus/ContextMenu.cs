#region References
using System;
using System.Collections.Generic;
#endregion

namespace Server.ContextMenus
{
	/// <summary>
	///     Represents the state of an active context menu. This includes who opened the menu, the menu's focus object, and a list of
	///     <see
	///         cref="ContextMenuEntry">
	///         entries
	///     </see>
	///     that the menu is composed of.
	///     <seealso cref="ContextMenuEntry" />
	/// </summary>
	public class ContextMenu : IDisposable
	{
		public bool IsDisposed { get; private set; }

		/// <summary>
		///     Gets the <see cref="Mobile" /> who opened this ContextMenu.
		/// </summary>
		public Mobile From { get; private set; }

		/// <summary>
		///     Gets an object of the <see cref="Mobile" /> or <see cref="Item" /> for which this ContextMenu is on.
		/// </summary>
		public IEntity Target { get; private set; }

		/// <summary>
		///     Gets the list of <see cref="ContextMenuEntry">entries</see> contained in this ContextMenu.
		/// </summary>
		public ContextMenuEntry[] Entries { get; private set; }

		/// <summary>
		///     Instantiates a new ContextMenu instance.
		/// </summary>
		/// <param name="from">
		///     The <see cref="Mobile" /> who opened this ContextMenu.
		///     <seealso cref="From" />
		/// </param>
		/// <param name="target">
		///     The <see cref="Mobile" /> or <see cref="Item" /> for which this ContextMenu is on.
		///     <seealso cref="Target" />
		/// </param>
		public ContextMenu(Mobile from, IEntity target)
		{
			From = from;
			Target = target;

			List<ContextMenuEntry> list = new List<ContextMenuEntry>();

			if (target is Mobile mobile)
			{
				mobile.GetContextMenuEntries(from, list);
			}
			else if (target is Item item)
			{
				item.GetContextMenuEntries(from, list);
			}

            for (var index = 0; index < list.Count; index++)
            {
                ContextMenuEntry e = list[index];

                e.Owner = this;
            }

            Entries = list.ToArray();

			list.Clear();
			list.TrimExcess();
		}

		~ContextMenu()
		{
			Dispose();
		}

        public void Dispose()
		{
			if (IsDisposed)
			{
				return;
			}

			IsDisposed = true;

			if (Entries != null)
            {
                for (var index = 0; index < Entries.Length; index++)
                {
                    ContextMenuEntry e = Entries[index];

                    if (e != null)
                    {
                        e.Dispose();
                    }
                }

                Entries = null;
            }

			if (From != null)
			{
				if (From.ContextMenu == this)
				{
					From.ContextMenu = null;
				}

				From = null;
			}

			Target = null;
		}

		public static bool Display(Mobile m, IEntity target)
		{
			if (m == null || target == null || m.Map != target.Map)
			{
				return false;
			}

			if (target is Mobile && !Utility.InUpdateRange(m, target.Location))
			{
				return false;
			}

			if (target is Item item && !Utility.InUpdateRange(m, item.GetWorldLocation()))
			{
				return false;
			}

			if (!m.CheckContextMenuDisplay(target))
			{
				return false;
			}

			ContextMenu c = new ContextMenu(m, target);

			if (c.Entries.Length <= 0)
			{
				return false;
			}

			if (target is Item i)
			{
				object root = i.RootParent;

				if (root is Mobile mobile && mobile != m && mobile.AccessLevel >= m.AccessLevel)
                {
                    for (var index = 0; index < c.Entries.Length; index++)
                    {
                        ContextMenuEntry e = c.Entries[index];

                        if (!e.NonLocalUse)
                        {
                            e.Enabled = false;
                        }
                    }
                }
			}

			m.ContextMenu = c;

			return true;
		}

		/// <summary>
		/// Returns the proper index of Enhanced Client Context Menu when sent from the icon on 
		/// the vendors status bar. Only known are Bank, Bulk Order Info and Bribe
		/// </summary>
		/// <param name="index">pre-described index sent by client. Must be 0x64 or higher</param>
		/// <returns>actual index of pre-desribed index from client</returns>
		public int GetReturnCode(int index)
		{
			// search for the return code
			for ( int i = 0; i < Entries.Length; i++ )
				if ( Entries[i].ReturnCode == index )
					return i;

			// if the return code is not found, return the index as is
			return index;
		}
	}
}
