using Content.Shared.Containers.ItemSlots;
using Content.Shared.Stacks;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._NF.Bank.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class BankATMComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite), DataField("cashType")]
    public ProtoId<StackPrototype> CashType = "Credit";

    public static string CashSlotId = "bank-ATM-cashSlot";

    [DataField]
    public Dictionary<SectorBankAccount, float> TaxAccounts = new();

    [DataField]
    public ItemSlot CashSlot = new();

    [DataField]
    public SoundSpecifier ErrorSound =
        new SoundPathSpecifier("/Audio/Effects/Cargo/buzz_sigh.ogg");

    [DataField]
    public SoundSpecifier ConfirmSound =
        new SoundPathSpecifier("/Audio/Effects/Cargo/ping.ogg");
}
