namespace Tgs.GameRecord.WMCasino.Core.UnitTest;

public class UtensilRepositoryUnitTest
{
    [Fact]
    public void WMCasino_User_To_PlayerCode()
    {
        var repository = new UtensilRepository();

        var actual = repository.WMCasinoUserToPlayerCode("wmtf_4uispltzj9d8_PB2");

        Assert.NotNull(actual);
        Assert.Equal("4uispltzj9d8", actual);
    }
}
