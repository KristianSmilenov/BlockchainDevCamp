IF NOT "%1"=="" (
	echo "Starting Restore"
	dotnet restore Blockchain.sln

	echo "Starting Build"
	dotnet build Blockchain.sln

	echo "Starting Publish"
	dotnet publish Blockchain/Blockchain.csproj -c Release
	dotnet publish BlockExplorer/BlockExplorer.csproj -c Release
	dotnet publish Miner/Miner.csproj -c Release
	dotnet publish WebWallet/WebWallet.csproj -c Release
	dotnet publish WebFaucet/WebFaucet.csproj -c Release
	)
::----------------------------------


::----------------------------------
:: 3 nodes
::----------------------------------
:: faucet private key: d9811a2c1baf6a2558f8ff5fb4cc624b6f2c32bb6dea659cf68584fcd028ee36
:: faucet public  key: 037ed1e11827c2a341e20e0b54decd9e7f80c5e34e163bf74a2f29946b56f66caa
:: faucet     address: a4a239576a1d25b32cf2a037e3540f6a2326fdc3

SET faucetAddress=a4a239576a1d25b32cf2a037e3540f6a2326fdc3
start dotnet Blockchain/bin/Release/netcoreapp2.0/publish/Blockchain.dll --server.urls "http://localhost:5101" --AppSettings:NodeName "Ivan" --AppSettings:NodeUrl "http://localhost:5101" --AppSettings:FaucetAddress %faucetAddress% --AppSettings:Difficulty 5
::start dotnet Blockchain/bin/Release/netcoreapp2.0/publish/Blockchain.dll --server.urls "http://localhost:5102" --AppSettings:NodeName "Ivan" --AppSettings:NodeUrl "http://localhost:5102" --AppSettings:FaucetAddress %faucetAddress%  --AppSettings:Difficulty 6
::start dotnet Blockchain/bin/Release/netcoreapp2.0/publish/Blockchain.dll --server.urls "http://localhost:5103" --AppSettings:NodeName "Ivan" --AppSettings:NodeUrl "http://localhost:5103" --AppSettings:FaucetAddress %faucetAddress%  --AppSettings:Difficulty 6

::----------------------------------


::----------------------------------
:: Faucet:
::----------------------------------

:: start dotnet WebFaucet/bin/Release/netcoreapp2.0/publish/WebFaucet.dll  --server.urls "http://localhost:5201" --AppSettings:PrivateKey d9811a2c1baf6a2558f8ff5fb4cc624b6f2c32bb6dea659cf68584fcd028ee36 --AppSettings:Address %faucetAddress%

::----------------------------------


::----------------------------------
:: Block Explorer:
::----------------------------------

:: start dotnet BlockExplorer/bin/Release/netcoreapp2.0/publish/BlockExplorer.dll --server.urls "http://localhost:5301"

::----------------------------------


::----------------------------------
:: Wallet:
::----------------------------------

:: start dotnet WebWallet/bin/Release/netcoreapp2.0/publish/WebWallet.dll --server.urls "http://localhost:5401"

::----------------------------------


::----------------------------------
:: 3 miners:
::----------------------------------

:: private key: 35e1c828807efdc358d98d35082fd80f01659120d06dd8dad2e5006e17abb8f3
:: public  key: 031dcfaefb92d3e3bb935a88cfd99a8f148821471436e7ddaa9bc8a1e4996c2a09
:: address    : ea1ca8370ebe39b8ba29fedb9b3786df6d2e9a9c
start dotnet Miner/bin/Release/netcoreapp2.0/publish/Miner.dll "http://localhost:5101" ea1ca8370ebe39b8ba29fedb9b3786df6d2e9a9c

:: private key: 41db7599458eddf2128f7c68e110b7c94fea9c6df69fab3a8ee6a81fa1b362d8
:: public  key: 03dbe54f289af4832b36228f2bcdcf6c959424d44d3b85374d6079e2fa73d22e2d
:: address    : 5868c2f8f6e556f7a7de40f08d0355e951f0e058
start dotnet Miner/bin/Release/netcoreapp2.0/publish/Miner.dll "http://localhost:5101" 5868c2f8f6e556f7a7de40f08d0355e951f0e058

:: private key: 8ce026a4ffe017222cc7a54ab5b3f126c4b386a9751075a9000df65e3aefab51
:: public  key: 0317dbff0d1071e31159535153c96ae1ad1ef4cf7db867aa56fdced38dea6385f3
:: address    : 0094b9f4bed622ce6954222bbedcb6cb6d71f991
::start dotnet Miner/bin/Release/netcoreapp2.0/publish/Miner.dll "http://localhost:5102" 0094b9f4bed622ce6954222bbedcb6cb6d71f991

::----------------------------------