# Web3G: Empowering Unity Games with Secret Network Blockchain Integration

Web3G is an SDK designed to integrate Secret Network blockchain functionalities directly into games developed using the Unity game engine. By leveraging Web3G, developers can create rich Web3 gaming experiences, incorporating secure and private on-chain interactions, beyond the capabilities of traditional HTML-based games.

## Key Features
- **Wallet Management:** Easily manage player wallets within Unity.
- **On-chain Data Queries:** Seamlessly query blockchain data.
- **Transaction Submission:** Enable players to accept or deny transactions via in-game prompts.
- **Secure On-Chain Randomness:** Utilize Secret Network’s fast and reliable randomness.
- **Private Contract State:** Create tamper-proof gaming experiences with private contract states.

## Problem Statement
Currently, most decentralized applications (DApps) are web-based and primarily use JavaScript/TypeScript. This limits the development of native apps, games, and IoT devices that can leverage blockchain functionality. Web3G aims to bridge this gap by providing a Unity SDK for Secret Network, empowering developers to integrate blockchain into their Unity games.

## Project Summary
Web3G extends the existing SecretNET project to run with Unity, enabling blockchain integration in games. This SDK supports Unity 2022.3.1 LTS and above and includes various modifications to ensure compatibility with Unity’s .NET runtime.

## Installation
Install Web3G through the Unity Package Manager using the "Install from Git" option.

### Requirements
- Unity 2022.3.1 LTS or higher
- Newtonsoft.Json for Unity
- TextMeshPro
- UnityGrpcWeb

## Integration & Setup
Web3G simplifies the process of integrating Secret Network into your game. Follow these steps to get started:

1. **Import Web3G Package:**
   Import the package via Unity Package Manager.

2. **Setup Secret Loader:**
   Drag the provided `Secret Loader` prefab into your scene to set up wallet integration.
   Alternatively, use the pre-constructed wallet scene provided in the `Scenes/` folder.

3. **Configure Post Init Scene:**
   Adjust the `Post Init Scene` property in the Secret Loader component to the name of your game's first scene (e.g., splash screen, main menu).

### Quick Start
1. Add the wallet scene to your build settings at index 0.
2. Ensure the user connects their wallet before accessing the gameplay.

## Usage

### Accessing SecretLoader
```csharp
SecretLoader secret = SecretLoader.Instance;
```

### Getting Signer Wallet Info
```csharp
Debug.Log(secret.Signer.Address);
```

### Querying the Blockchain
```csharp
var myScrtBalance = await secret.Queries.Bank.Balance(new Cosmos.Bank.V1Beta1.QueryBalanceRequest()
{
    Address = "...", 
    Denom = "uscrt" 
});
```

### Smart Contract Query Short-hand
```csharp
var welcomePack = await secret.QueryContractState<WelcomePackQuery>(
    "secret1zag3hdz0e0aqnw9450dawg7j6j56uww8xxhqrn", 
    new 
    { 
        qualified_for_welcome_pack = new 
        { 
            address = secret.Signer.Address 
        } 
    }
);
Debug.Log(welcomePack.RawResponse);
welcomePackRewardsButton.enabled = welcomePack.Response.Qualified;
```

### Broadcasting Transactions
```csharp
await secret.SignTransaction(
    new[] { 
        new MsgExecuteContract(
            "secret1zag3hdz0e0aqnw9450dawg7j6j56uww8xxhqrn", 
            new { receive_welcome_pack = new { } }) 
        {
            Sender = secret.Signer.Address 
        }
    }
);
```
The transaction will prompt the user with a dialog box to accept or deny the transaction.

## Current State
Web3G is in its early stages of development and is not yet production-ready. Feedback and contributions are welcome to help improve and stabilize the framework.

## Future Plans
- Stabilize the library and streamline its usage.
- Develop a trading card game on Secret Network using Web3G as a showcase.
- Encourage community contributions to enhance and expand the SDK’s capabilities.
