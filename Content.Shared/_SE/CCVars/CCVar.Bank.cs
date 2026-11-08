using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

[CVarDefs]
public sealed class BankCVars
{
    /// <summary>
    /// Determines the existence of banjing system in rounedosutaruto
    /// </summary>
    public static readonly CVarDef<bool> BankEnabled =
        CVarDef.Create("bank.enabled", true, CVar.SERVER | CVar.REPLICATED);

    /// <summary>
    /// Base money level for begginers.
    /// </summary>
    public static readonly CVarDef<int> BankStarter =
        CVarDef.Create("bank.starter", 1000, CVar.SERVER | CVar.REPLICATED);
}
