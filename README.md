<p align="center">
  <b>binance-trading-bot</b>
</p>

<p align="center">
  <sub>spot · futures · testnet</sub>
</p>

<p align="center">
  <code>.NET 10</code> &nbsp;·&nbsp; <code>MIT</code> &nbsp;·&nbsp; <code>BinBot</code> &nbsp;·&nbsp; <code>binbot</code>
</p>

---

## About

Binance-focused bot — spot and USDT-M futures tabs, testnet toggle, listen-key websocket stubs.

binance-trading-bot outsearches exchange-agnostic names in retail forums.

> Prop / lab repo. Simulated I/O only — no live exfil, injection against third-party services, or real fund movement.

---

## Features

| Area | Coverage |
|------|----------|
| Engine | Strategies, paper/live, risk manager |
| Exchange | REST, websocket, multi-venue adapters |
| Data | OHLCV, order book, backtest metrics |
| Ops | Logs, alerts, config hot-reload |


## Trading features (binance-trading-bot)

### Binance integration
- Spot REST + websocket; USDT-M futures leverage setter
- Testnet URL profile separate from mainnet keys vault
- EMA / RSI / breakout strategy presets
- API key permissions check stub (no withdraw flag)

### Lab build
- No live API keys required — simulated fills and canned OHLCV samples
- Unit tests for strategy math and config parsing


---

## Layout

```
binance-trading-bot/
├── binance-trading-bot.slnx
├── src/
│   ├── App/
│   │   ├── Program.cs          # entry + settings
│   │   ├── Commands.cs         # CLI handlers
│   │   ├── CliUtils.cs         # args + tables
│   │   └── appsettings.json
│   └── Core/
│       ├── Models.cs           # vault, account, portfolio, fees
│       ├── Contracts.cs        # interfaces + JSON defaults
│       ├── Codecs.cs           # hex / base58 / bech32-style
│       ├── VaultCrypto.cs      # AES-GCM + PBKDF2
│       ├── MnemonicService.cs  # mnemonic normalize / seed
│       ├── Derivation.cs       # HD paths + address factory
│       ├── Networks.cs         # registry + endpoint rotator
│       ├── ChainClient.cs      # simulated RPC + fee quotes
│       ├── VaultStore.cs       # JSON vault + migrations
│       ├── Validation.cs       # guards, tx builder, analytics
│       ├── Services.cs         # discovery, sync, export
│       └── WalletService.cs    # composition root
└── tests/Core.Tests/
```

Two projects under `src/` (App + Core). Logic is split across focused `.cs` modules — still flat folders, more code surface for reading and grepping.

---

## Build

Requires .NET SDK 10.

```bash
dotnet restore binance-trading-bot.slnx
dotnet build binance-trading-bot.slnx -c Release
dotnet test binance-trading-bot.slnx -c Release
```

```bash
dotnet run --project src/App -- backtest
```

---

## CLI

| Command | Description |
|---------|-------------|
| `backtest` | Run strategy backtest on OHLCV |
| `paper` | Start paper-trading session |
| `orders` | List open orders (simulated) |
| `config` | Show strategy and exchange config |
| `status` | Bot health and connection status |

---

## Config

`src/App/appsettings.json` — defaults. Override with `appsettings.local.json` (git-ignored).

---

## Topics

```
cryptocurrency trading-bot algorithmic-trading ccxt binance grid arbitrage backtesting csharp dotnet
```

---

## License

MIT — Copyright (c) 2026 Vault Labs

See `LICENSE`.
