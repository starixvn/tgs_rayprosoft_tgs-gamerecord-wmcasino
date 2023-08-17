namespace Tgs.GameRecord.WMCasinoService.Core;

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
