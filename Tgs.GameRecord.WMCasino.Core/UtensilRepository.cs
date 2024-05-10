namespace Tgs.GameRecord.WMCasino.Core;

internal class UtensilRepository : IUtensil
{
	public string WMCasinoUserToPlayerCode(string user)
	{
		try
		{
			return user.Split('_')[1];
		}
		catch
		{
			return user;
		}
	}
}
