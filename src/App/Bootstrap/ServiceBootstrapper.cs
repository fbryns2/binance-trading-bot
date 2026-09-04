using BinBot.App.Commands;
using BinBot.ChainProviders.Evm;
using BinBot.ChainProviders.Transport;
using BinBot.ChainProviders.Utxo;
using BinBot.Cryptography.Derivation;
using BinBot.Cryptography.Vault;
using BinBot.Engine.Analytics;
using BinBot.Engine.Domain.Contracts;
using BinBot.Engine.Networks;
using BinBot.Engine.Options;
using BinBot.Engine.Orchestration;
using BinBot.Persistence.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BinBot.App.Bootstrap;

public sealed class ServiceBootstrapper
{
    public ApplicationServices Build(WalletOptions options)
    {
        Directory.CreateDirectory(options.DefaultVaultDirectory);

        var loggerFactory = LoggerFactory.Create(builder => builder.AddSimpleConsole());
        var networkRegistry = new NetworkRegistry();
        var endpointRotator = new EndpointRotator();

        foreach (var network in networkRegistry.GetAllNetworks())
        {
            endpointRotator.RegisterNetwork(network.NetworkId, network.RpcEndpoints);
        }

        var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(options.RpcTimeoutSeconds) };
        var transport = new HttpTransportLayer(httpClient, endpointRotator, new RateLimitHandler(options.MaxConcurrentNetworkRequests));

        var networkClients = new INetworkClient[]
        {
            new EthereumRpcClient(transport),
            new BitcoinRpcClient(transport),
            new PolygonRpcClient(transport),
            new BscRpcClient(transport)
        };

        var dbPath = Path.Combine(options.DefaultVaultDirectory, "vaults.db");
        var walletStore = new SqliteWalletStore(dbPath);
        var bip32 = new Bip32KeyDeriver();
        var mnemonicProcessor = new Bip39MnemonicProcessor(bip32);
        var derivation = new KeyDerivationService(mnemonicProcessor);
        var encryptor = new VaultEncryptor();
        var walletProvider = new DefaultWalletProvider(derivation, encryptor);

        var walletManager = new WalletManager(
            walletProvider,
            walletStore,
            networkRegistry,
            Options.Create(options),
            loggerFactory.CreateLogger<WalletManager>());

        var syncCoordinator = new SyncCoordinator(
            walletStore,
            networkRegistry,
            networkClients,
            new BalanceAggregator(),
            loggerFactory.CreateLogger<SyncCoordinator>());

        return new ApplicationServices(
            options,
            loggerFactory,
            walletManager,
            syncCoordinator,
            walletStore,
            networkRegistry,
            networkClients,
            new PortfolioReporter(),
            new NetworkHealthMonitor(),
            new SyncProgressTracker());
    }
}

public sealed class ApplicationServices
{
    public ApplicationServices(
        WalletOptions options,
        ILoggerFactory loggerFactory,
        WalletManager walletManager,
        SyncCoordinator syncCoordinator,
        IWalletStore walletStore,
        NetworkRegistry networkRegistry,
        IEnumerable<INetworkClient> networkClients,
        PortfolioReporter portfolioReporter,
        NetworkHealthMonitor healthMonitor,
        SyncProgressTracker progressTracker)
    {
        Options = options;
        LoggerFactory = loggerFactory;
        WalletManager = walletManager;
        SyncCoordinator = syncCoordinator;
        WalletStore = walletStore;
        NetworkRegistry = networkRegistry;
        NetworkClients = networkClients;
        PortfolioReporter = portfolioReporter;
        HealthMonitor = healthMonitor;
        ProgressTracker = progressTracker;
    }

    public WalletOptions Options { get; }
    public ILoggerFactory LoggerFactory { get; }
    public WalletManager WalletManager { get; }
    public SyncCoordinator SyncCoordinator { get; }
    public IWalletStore WalletStore { get; }
    public NetworkRegistry NetworkRegistry { get; }
    public IEnumerable<INetworkClient> NetworkClients { get; }
    public PortfolioReporter PortfolioReporter { get; }
    public NetworkHealthMonitor HealthMonitor { get; }
    public SyncProgressTracker ProgressTracker { get; }
}
