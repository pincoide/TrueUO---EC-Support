using Server.Accounting;
using Server.Engines.CityLoyalty;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Prompts;
using Server.Targeting;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Principal;

namespace Server.Gumps
{
    public class BankerGump : Gump
    {
        private readonly int TextColor = 2317;

        public PlayerMobile User { get; }

        public BankerGump( PlayerMobile pm )
            : base( 150, 150 )
        {
            TypeID = 999123;

            User = pm;
            AddGumpLayout();
        }

        public void AddGumpLayout()
        {
            AddBackground( 0, 0, 420, 380, 9300 );

            AddHtmlLocalized( 0, 10, 400, 16, 1113302, "#1156076", 1, false, false ); // Bank Actions

            Account acct = User.Account as Account;

            AddHtmlLocalized( 10, 33, 110, 18, 1156044, TextColor, false, false ); // Total Gold:
            AddHtml( 130, 33, 200, 16, acct != null ? acct.TotalGold.ToString( "N0", CultureInfo.GetCultureInfo( "en-US" ) ) : "0", false, false );

            AddHtmlLocalized( 10, 51, 110, 18, 1156045, TextColor, false, false ); // Total Platinum:
            AddHtml( 130, 51, 200, 16, acct != null ? acct.TotalPlat.ToString( "N0", CultureInfo.GetCultureInfo( "en-US" ) ) : "0", false, false );

            AddHtmlLocalized( 10, 69, 110, 18, 1157003, TextColor, false, false ); // Secure Account:
            AddHtml( 130, 69, 200, 16, acct != null ? acct.GetSecureAccountAmount( User ).ToString( "N0", CultureInfo.GetCultureInfo( "en-US" ) ) : " 0", false, false );

            AddHtmlLocalized( 10, 87, 110, 18, 1157004, TextColor, false, false ); // Transfer Gold:
            AddHtml( 130, 87, 200, 16, "0", false, false );

            AddHtmlLocalized( 10, 105, 110, 18, 1157005, TextColor, false, false ); // Transfer Platinum:
            AddHtml( 130, 105, 200, 16, "0", false, false );

            AddHtmlLocalized( 55, 141, 340, 22, 1156064, TextColor, false, false ); // Deposit Gold into Character Transfer Account
            AddButton( 15, 141, 4005, 4006, 1, GumpButtonType.Reply, 0 );
            AddTooltip( 1156070 ); // Transfers gold from the bank to the character transfer account; capped at 1 billion gold. Any currency that 
                                   // a players wishes to transfer to another shard must be placed in character transfer account. Upon transferring 
                                   // the currency will be added to player's account on the shard.

            AddHtmlLocalized( 55, 174, 340, 22, 1156065, TextColor, false, false ); // Deposit Platinum into Character Transfer Account
            AddButton( 15, 174, 4005, 4006, 2, GumpButtonType.Reply, 0 );
            AddTooltip( 1156071 ); // Transfers platinum from the bank to the character transfer account; capped at 2 billion platinum. Any currency 
                                   // that a players wishes to transfer to another shard must be placed in character transfer account. Upon transferring 
                                   // the currency will be added to player's account on the shard. 

            AddHtmlLocalized( 55, 207, 340, 22, 1156066, TextColor, false, false ); // Withdraw Gold from Character Transfer Account
            AddButton( 15, 207, 4005, 4006, 3, GumpButtonType.Reply, 0 );
            AddTooltip( 1156072 ); // Transfers gold from the character transfer account to the bank; capped at 1 billion gold.

            AddHtmlLocalized( 55, 240, 340, 22, 1156067, TextColor, false, false ); // Withdraw Platinum from Character Transfer Account
            AddButton( 15, 240, 4005, 4006, 4, GumpButtonType.Reply, 0 );
            AddTooltip( 1156073 ); // Transfers platinum from the character transfer account to the bank; capped at 2 billion platinum. Really? Who the fuck has this much?

            AddHtmlLocalized( 55, 273, 340, 22, 1156068, TextColor, false, false ); // Deposit Gold into Secure Account
            AddButton( 15, 273, 4005, 4006, 5, GumpButtonType.Reply, 0 );
            AddTooltip( 1156074 ); // Transfers gold from the bank to the player's secure account; capped at 100,000,000 gold. Only funds added 
                                   // to the secure account can be added to the wall safe account.

            AddHtmlLocalized( 55, 306, 340, 22, 1156069, TextColor, false, false ); // Withdraw Gold from Secure Account
            AddButton( 15, 306, 4005, 4006, 6, GumpButtonType.Reply, 0 );
            AddTooltip( 1156075 ); // Transfers gold from the secure account to the bank; capped at 100,0,000 gold.

            AddHtmlLocalized( 55, 339, 340, 22, 1158381, TextColor, false, false ); // Make deposit into currency account
            AddButton( 15, 339, 4005, 4006, 7, GumpButtonType.Reply, 0 );

            AddHtmlLocalized( 10, 33, 340, 22, 1114514, "#1061037", TextColor, false, false ); // Help
            AddButton( 365, 33, 4014, 4015, 8, GumpButtonType.Reply, 0 );
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
            Account acct = User.Account as Account;

            switch ( info.ButtonID )
            {
                case 0: break;
                case 1:
                case 2:
                case 3:
                case 4:
                    User.SendMessage( "This feature is currenlty disabled." );
                    Refresh( false );
                    break;
                case 5:
                    User.Prompt = new DepositToSecureAccountPrompt( acct );

                    break;
                case 6:
                    User.Prompt = new WithdrawFromSecureAccountPrompt( acct );

                    break;
                case 7:
                    User.SendLocalizedMessage( 1158380 ); // Target the check or gold in your backpack that you wish to add to your currency account.
                    User.Target = new DepositTarget();
                    
                    break;
                case 8:
                    Refresh();
                    User.SendGump( new NewCurrencyHelpGump() );
                    break;
            }
        }

        public void Refresh( bool recompile = true )
        {
            if ( recompile )
            {
                Entries.Clear();
                Entries.TrimExcess();
                AddGumpLayout();
            }

            User.CloseGump( GetType() );
            User.SendGump( this, false );
        }

        private class DepositTarget : Target
        {
            public DepositTarget()
                : base( -1, false, TargetFlags.None )
            {
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if ( targeted is Item item )
                {
                    if ( !item.IsChildOf( from.Backpack ) )
                    {
                        from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
                    }
                    else if ( item.GetType() != typeof( Gold ) && item.GetType() != typeof( BankCheck ) )
                    {
                        // no error message on wron target type
                    }
                    else
                    {
                        from.BankBox.TryDropItem( from, item, false );
                    }
                }
                // show the gump again after the targeting is done
                from.SendGump( new BankerGump( (PlayerMobile)from ) );
            }
        }

        private class DepositToSecureAccountPrompt : Prompt
        {
            public override int MessageCliloc => 1155865; // Enter amount to deposit:
            private readonly Account m_Account;

            public DepositToSecureAccountPrompt( Account account )
                : base( 90133 )
            {
                m_Account = account;
            }

            public override void OnResponse( Mobile from, string text )
            {
                if ( m_Account != null )
                {
                    int canHold = Account.MaxSecureAmount - m_Account.GetSecureAccountAmount(from);

                    if ( text != null && !string.IsNullOrEmpty( text ) )
                    {
                        var v = Utility.ToInt32(text);

                        if ( v <= 0 || v > canHold || v > Banker.GetBalance( from ) )
                        {
                            from.SendLocalizedMessage( 1155867 ); // The amount entered is invalid. Verify that there are sufficient funds to complete this transaction.
                        }
                        else
                        {
                            Banker.Withdraw( from, v, true );
                            m_Account.DepositToSecure( from, v );

                            from.SendLocalizedMessage( 1153188 ); // Transaction successful:

                            from.SendGump( new BankerGump( (PlayerMobile)from ) );
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage( 1155867 ); // The amount entered is invalid. Verify that there are sufficient funds to complete this transaction.
                    }
                }
            }

            public override void OnCancel( Mobile from )
            {
                from.SendGump( new BankerGump( (PlayerMobile)from ) );
            }
        }

        private class WithdrawFromSecureAccountPrompt : Prompt
        {
            public override int MessageCliloc => 1155866; // Enter amount to withdraw:
            private readonly Account m_Account;

            public WithdrawFromSecureAccountPrompt( Account account )
                : base( 90134 )
            {
                m_Account = account;
            }

            public override void OnResponse( Mobile from, string text )
            {
                if ( m_Account != null )
                {
                    if ( text != null && !string.IsNullOrEmpty( text ) )
                    {
                        var v = Utility.ToInt32(text);

                        if ( v <= 0 || v > m_Account.GetSecureAccountAmount( from ) )
                        {
                            from.SendLocalizedMessage( 1155867 ); // The amount entered is invalid. Verify that there are sufficient funds to complete this transaction.
                        }
                        else
                        {
                            Banker.Deposit( from, v, true );
                            m_Account.WithdrawFromSecure( from, v );

                            from.SendLocalizedMessage( 1153188 ); // Transaction successful:
                        }
                        from.SendGump( new BankerGump( (PlayerMobile)from ) );
                    }
                    else
                    {
                        from.SendLocalizedMessage( 1155867 ); // The amount entered is invalid. Verify that there are sufficient funds to complete this transaction.
                    }
                }
            }

            public override void OnCancel( Mobile from )
            {
                from.SendGump( new BankerGump( (PlayerMobile)from ) );
            }
        }
    }

    public class NewCurrencyHelpGump : Gump
    {
        public NewCurrencyHelpGump() : base( 50, 75 )
        {
            TypeID = 999002;

            AddBackground( 0, 0, 875, 480, 5170 );
            AddHtmlLocalized( 40, 40, 810, 414, 1156048, 2317, false, false );
        }
    }
}
