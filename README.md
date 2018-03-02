# .NET Core Blockchain

An all-around blockchain solution built with .NET Core exposing a Web API.
It has all the components needed for testing and deploying a fully functional blockchain solution.

## Security

The project implements top-notch cryptographic algorithms to make sure your funds are safe.

Private/public key pairs are generated using ECDSA and the secp256k1 curve used by Bitcoin.

The private key is password protected using scrypt and AES-256,
to make sure that even if somebody manages to steal your browser data, they won't get a hold of your funds.
You can backup your private key, using 24-word mnemonic and a secret password.
Even if somebody got your words, they'd still need your password.

Each transaction is signed using your private key and verified by the nodes, using your public key.

## Mining 

Mining requires proof of work, with varying difficulty. This means that if someone wants to mine a lot of blocks (for the reward),
they would have to put considerable investment in hardware and time.
The work being done is a simple SHA256 hash of the blocks.

Each mined block gives the miner a variable reward (some pre-set bounty plus all the transaction fees in the block)

Whenever new transactions come in, the miner may choose to start mining the new more profitable block or keep mining the old one.

## Components

* Node
* Wallet
* Miner
* Faucet
* Explorer

### Node
The heart of the blockchain. Combine a few of those guys and you've got yourself a network.
Can work alone, but then it's not really distributed, is it?
If you run a few of these guys, you can round them up in a network and they wil keep each other in sync.

#### Node API
The node exposes the following API endpoint categories:
* Balance - allows you to check a ballance of a wallet with a variable amount of confirmations
* Blocks - read all blocks (or a specific one), synchronize the blockchain (via push)
* Info - general info about the node
* Mining - API for miners to do to some work and get some rewards
* Peers - API for other nodes to hook up to and sync
* Transactions - allows you to check past transactions and to create new ones

#### Sync between nodes
When you have at least 2 nodes running (let's say yours and somebody else's),
you can sync them up, so they are up-to-date with each other's blocks and transactions.

In order to achieve that, you have to use the Peers API to notify the other node that you are there and want to exchange info.
When you do a POST to /api/peers, the other node will know of your existence 
and send you notifications about changes to its blocks and/or transactions

If the other node wants you to sent changes to it, it will use your peer API in return.

##### Consensus algorithm
The mechanism of synchronization is the following:
* Whenever a new transaction comes in, broadcast it to all peers
* Whenever a new block comes in, broadcast it to all peers
* Whenever another node broadcasts a transaction, put it in the pending list
* Whenever another node broadcasts a new block, check it for validity.
	* If it's invalid, discard it
	* Otherwise, find if you have it's ancestor in the list
		* If yes, this means, that you just missed this one block. Attach it at the end
		* If no, sync the whole blockchain - take the other guys chain, validate it (hashes and stuff), replace yours with theirs
		In both cases, purge the pending transactions (the ones already mined should be out of the pending list)

### Wallet
Front-end only web app that allows you to create and use a wallet, check your balance and push transactions to the blockchain.
It's so user-friendly and intuitive, that it barely needs any documentation at all.
Functionalities:
* Create Wallet - creates new private, public key and address for you to store however you please
* Open Wallet - restores public key and address from private key
* Account Balance - check current balance 
* Send Transaction - send coins to someone in the network. Beware! No valdiation address is available, so you can easily lose some funds :)

If you click on the title, a magical input will appear, allowing you to change the node URL,
in case you want to connect to a different node.

### Miner
A fully functional miner app that attaches to a node of your choice and mines new blocks to reap awards.
As simple as it gets, started from the command line via:
dotnet Miner.dll <Node_Url> <Miner_Address>
It will mine till it dies. If node goes offline, it will sleep and wait for it to be back online.

### Faucet
Free cash!
Literally!
This simple tool allows you to test your network, by giving you some free coins on demand.
It has anti-spam prevention, so you won't drain it at once.

If you click on the title, a magical input will appear, allowing you to change the node URL,
in case you want to connect to a different node.

### Explorer
The user-friendly alternative to curl http://node_url/api. 
It allows you to see what's going on with the network. Data is cached and refreshed once every 10 seconds.
* Home - basic info about latest changes
* Blocks - all blocks available in the current node 
* Transactions - all transactions available in the current node.
You can switch between pending and confirmed
* Accounts - allows you to check account balance, based on the address of the wallet
* Peers network - funky interactive map of the peers attached to the current node.
* Search - top right - it allows you to easily find transactions and blocks via hash

If you click on the title, a magical input will appear, allowing you to change the node URL,
in case you want to connect to a different node.
