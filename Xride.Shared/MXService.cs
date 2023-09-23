using Mx.NET.SDK.Configuration;
using Mx.NET.SDK.Domain.Data.Accounts;
using Mx.NET.SDK.Provider;

namespace Xride.Shared;

public class MXService
{
    public async Task<MXAccount[]> GetBalance()
    {
        var provider = new ApiProvider(new ApiNetworkConfiguration(Network.MainNet));

        var bech32Address = "erd1668kkqgu7wme9vmgvmgne99lar4ae6r0m65sgqc2gqqzelsguygsp46p3n";
        var account = Account.From(await provider.GetAccount(bech32Address));

        return new MXAccount[] { new MXAccount { Balance = account.Balance.ToCurrencyString(4) }  };
    }
}
