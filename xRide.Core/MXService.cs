using Microsoft.Extensions.Configuration;
using Mx.NET.SDK.Configuration;
using Mx.NET.SDK.Core.Domain.Abi;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain.Data.Accounts;
using Mx.NET.SDK.Domain.SmartContracts;
using Mx.NET.SDK.NativeAuthClient;
using Mx.NET.SDK.NativeAuthClient.Entities;
using Mx.NET.SDK.NativeAuthServer;
using Mx.NET.SDK.NativeAuthServer.Entities;
using Mx.NET.SDK.Provider;
using Mx.NET.SDK.Wallet.Wallet;
using Mx.NET.SDK.WalletConnect;
using WalletConnectSharp.Core;

namespace xRide.Core;

public class MXService
{
    const string CHAIN_ID = "D";
    const string PROJECT_ID = "WALLET_CONNECT_PROJECT_ID";

    IWalletConnect WalletConnect { get; set; }

    private readonly NativeAuthClient _nativeAuthToken = default!;
    private readonly NativeAuthServer _nativeAuthServer = default!;

    private Wallet _wallet;
    private IApiProvider _apiProvider;
    private Address? _scAddress = Address.From("erd1qqqqqqqqqqqqqpgqlhcy2y6hkl8xzp909t8646cqrx43ljucm9dsgktymt");
    private WalletSigner _walletSigner;
    private AbiDefinition _abi;
    private Account _account;
    private string _scName = "x-ride";
    private const string bech32Address = "erd1668kkqgu7wme9vmgvmgne99lar4ae6r0m65sgqc2gqqzelsguygsp46p3n";
    private readonly IConfiguration config;

    public MXService(IConfiguration config)
    {
        var metadata = new Metadata()
        {
            Name = "Mx.NET.WinForms",
            Description = "Mx.NET.WinForms login testing",
            Icons = new[] { "https://devnet.remarkable.tools/remarkabletools.ico" },
            Url = "https://devnet.remarkable.tools/"
        };
        WalletConnect = new WalletConnect(metadata, PROJECT_ID, CHAIN_ID);
        _nativeAuthToken = new(new NativeAuthClientConfig()
        {
            //Origin = metadata.Name,
            Origin = "https://devnet.remarkable.tools/",
            ExpirySeconds = 14400,
            BlockHashShard = 2
        });
        var nativeAuthServerConfig = new NativeAuthServerConfig()
        {
            AcceptedOrigins = new[] { "https://devnet.remarkable.tools/" }
        };
        _nativeAuthServer = new(nativeAuthServerConfig);

        _wallet = Wallet.FromMnemonic("dilemma what client solution flag frog auto urge phone salt crawl spike fall index tonight flash entry balcony north clown patch reflect deposit transfer", 1);
        _apiProvider = new ApiProvider(new ApiNetworkConfiguration(Network.DevNet));
        _walletSigner = _wallet.GetSigner();
        _account = new Account(_wallet.GetAddress());
        this.config = config;
    }

    public async Task<MXAccount[]> GetBalance()
    {
        var account = Account.From(await _apiProvider.GetAccount(bech32Address));

        return new MXAccount[] { new MXAccount { Balance = account.Balance.ToCurrencyString(4) } };
    }

    public async Task<string> GetRides(string abiJson)
    {
        _abi = AbiDefinition.FromJson(abiJson);

        var queryResult = await SmartContract.QuerySmartContractWithAbiDefinition<VariadicValue>(_apiProvider,
                                                                _scAddress,
                                                                _abi,
                                                                "getRideStorage");

        return queryResult.ToJson();
    }

    public async Task<string> Connect()
    {
        //TODO

        return string.Empty;
    }
}

