using Mx.NET.SDK.Configuration;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Abi;
using Mx.NET.SDK.Core.Domain.SmartContracts;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain;
using Mx.NET.SDK.Domain.Data.Accounts;
using Mx.NET.SDK.Domain.Data.Network;
using Mx.NET.SDK.Domain.Data.Transactions;
using Mx.NET.SDK.Domain.SmartContracts;
using Mx.NET.SDK.Provider;
using Mx.NET.SDK.TransactionsManager;
using Mx.NET.SDK.Wallet;
using Mx.NET.SDK.Wallet.Wallet;

Operations.Init();
while (true)
{
    await Operations.ExecuteOperation(int.TryParse(Console.ReadLine(), out int a) ? a : -1);
    Console.WriteLine("Execute operation number:");
}

public static class Operations
{
    private static Wallet _wallet;
    private static IApiProvider _apiProvider;
    private static Address? _scAddress = Address.From("erd1qqqqqqqqqqqqqpgq9nz4fq25gn9zhwy8xykfl259e92a2ycauygss4aegy");//erd1qqqqqqqqqqqqqpgqln77pdp8pze2mlq0mzg5932pf8s6e3qxuygs94jwy0");
    private static WalletSigner _walletSigner;
    private static AbiDefinition _abi;
    private static Account _account;
    private static string _scName = "x-ride";

    static Operations()
    {
        Console.WriteLine("Wallet:");
        var filePath = $"SmartContracts\\{Console.ReadLine()}.json";
        Console.WriteLine("Password:");
        var password = Console.ReadLine();

        _wallet = Wallet.FromMnemonic("dilemma what client solution flag frog auto urge phone salt crawl spike fall index tonight flash entry balcony north clown patch reflect deposit transfer", 1);
        _apiProvider = new ApiProvider(new ApiNetworkConfiguration(Network.DevNet));
        _walletSigner = _wallet.GetSigner();
        _account = new Account(_wallet.GetAddress());
        _abi = AbiDefinition.FromFilePath($"SmartContracts\\{_scName}\\{_scName}.abi.json");
        Console.WriteLine("Execute operation number:");
    }

    public static void Init() { }
    public static async Task ExecuteOperation(int num)
    {
        try
        {
            switch (num)
            {
                case 0:
                    await DeployAsync(false);
                    break;
                case 1:
                    await CallAsync(Console.ReadLine()?.Trim() ?? string.Empty);
                    break;
                case 2:
                    await GetAsync(Console.ReadLine()?.Trim() ?? string.Empty);
                    break;
                case 3:
                    await DeployAsync(true);
                    break;
                default:
                    Console.WriteLine("No such operation.");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private static async Task GetAsync(string endpoint)
    {
        var queryResult = await SmartContract.QuerySmartContractWithAbiDefinition<VariadicValue>(_apiProvider,
                                                                       _scAddress,
                                                                       _abi,
                                                                       endpoint);

        Console.WriteLine(queryResult);
    }

    private static async Task CallAsync(string endpoint)
    {
        var _networkConfig = await NetworkConfig.GetFromNetwork(_apiProvider);
        await _account.Sync(_apiProvider);

        var transactionRequest = EGLDTransactionRequest.EGLDTransferToSmartContract(
        _networkConfig,
        _account,
        _scAddress,
        ESDTAmount.Zero(),
        endpoint,
        BytesValue.FromHex("C0B9886D83DACA7652EF"));
        //NumericValue.BigUintValue(33),
        //NumericValue.I64Value(DateTime.UtcNow.Ticks),
        //NumericValue.I64Value(DateTime.UtcNow.AddDays(1).Ticks),
        //NumericValue.U8Value(3));

        transactionRequest.SetGasLimit(GasLimit.ForSmartContractCall(_networkConfig, transactionRequest));
        var signature = _walletSigner.SignTransaction(transactionRequest.SerializeForSigning());
        var signedTransaction = transactionRequest.ApplySignature(signature);
        var response = await _apiProvider.SendTransaction(signedTransaction);
        var transaction = Transaction.From(response.TxHash);
        await transaction.AwaitExecuted(_apiProvider);
    }

    private static async Task DeployAsync(bool upgrade)
    {
        var _networkConfig = await NetworkConfig.GetFromNetwork(_apiProvider);
        await _account.Sync(_apiProvider);

        try
        {
            var code = CodeArtifact.FromFilePath($"SmartContracts\\{_scName}\\{_scName}.wasm");
            var codeMetaData = new CodeMetadata(true, true, false);

            TransactionRequest? contractRequest;
            if (upgrade)
            {
                contractRequest = SmartContractTransactionRequest.Upgrade(_networkConfig,
                                        _account,
                                        _scAddress,
                                        code,
                                        codeMetaData);
            }
            else
            {
                contractRequest = SmartContractTransactionRequest.Deploy(_networkConfig,
                                        _account,
                                        code,
                                        codeMetaData);
            }

            var signature = _walletSigner.SignTransaction(contractRequest.SerializeForSigning());
            var signedTransaction = contractRequest.ApplySignature(signature);
            var response = await _apiProvider.SendTransaction(signedTransaction);
            var transaction = Transaction.From(response.TxHash);
            _scAddress = SmartContract.ComputeAddress(contractRequest);
            await transaction.AwaitExecuted(_apiProvider);
            _account.IncrementNonce();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static void GenerateKey()
    {
        var mnm = Mnemonic.FromString("1 dilemma 2 what 3 client 4 solution 5 flag 6 frog 7 auto 8 urge 9 phone 10 salt 11 crawl 12 spike 13 fall 14 index 15 tonight 16 flash 17 entry 18 balcony 19 north 20 clown 21 patch 22 reflect 23 deposit 24 transfer");
        var wallet = Wallet.FromMnemonic(mnm.SeedPhrase, 1);
    }
}


