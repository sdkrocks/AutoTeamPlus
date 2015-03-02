using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using TerrariaApi.Server;
using Terraria;
using TShockAPI;
using TShockAPI.Hooks;

namespace AutoTeamPlus
{
	[ApiVersion(1,16)]
	public class AutoTeamPlus : TerrariaPlugin
	{
		public override string Author
		{
			get { return "Olink"; }
		}

		public override string Description
		{
			get { return "Force teams based on user permissions"; }
		}

		public override string Name
		{
			get { return "AutoTeamPlus"; }
		}

		public override Version Version
		{
			get { return new Version(1,0,0,0); }
		}

		public AutoTeamPlus(Main game) : base(game)
		{
		}

		public override void Initialize()
		{
			ServerApi.Hooks.NetGreetPlayer.Register(this, OnJoin);
			TShockAPI.GetDataHandlers.PlayerTeam += OnTeamChange;
			TShockAPI.Hooks.PlayerHooks.PlayerPostLogin += OnLogin;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnJoin);
				TShockAPI.GetDataHandlers.PlayerTeam -= OnTeamChange;
				TShockAPI.Hooks.PlayerHooks.PlayerPostLogin -= OnLogin;
			}
			base.Dispose(disposing);
		}

		private void OnJoin(GreetPlayerEventArgs args)
		{
			var ply = TShock.Players[args.Who];
			SetTeam(ply);
		}

		private void OnTeamChange(object sender, GetDataHandlers.PlayerTeamEventArgs args)
		{
			var ply = TShock.Players[args.PlayerId];

			args.Handled = SetTeam(ply);
		}

		private void OnLogin(PlayerPostLoginEventArgs args)
		{
			SetTeam(args.Player);
		}

		private bool SetTeam(TSPlayer ply)
		{
			if (ply.Group.Name == "superadmin" || ply.Group.HasPermission("*"))
			{
				return false;
			}

			if (ply.Group.HasPermission("autoteam.none"))
			{
				ply.SetTeam(0);
			}
			if (ply.Group.HasPermission("autoteam.red"))
			{
				ply.SetTeam(1);
			}
			if (ply.Group.HasPermission("autoteam.green"))
			{
				ply.SetTeam(2);
			}
			if (ply.Group.HasPermission("autoteam.blue"))
			{
				ply.SetTeam(3);
			}
			if (ply.Group.HasPermission("autoteam.yellow"))
			{
				ply.SetTeam(4);
			}
			else
			{
				return false;
			}

			ply.SendInfoMessage("Your team has been changed to {0}.", ply.Team == 0 ? "none" : ply.Team == 1 ? "the red team" : ply.Team == 2 ? "the green team" : ply.Team == 3 ? "the blue team" : "the yellow team");
			return true;
		}
	}
}
