Programowanie sieciowe 2020
Laboratorium - Projekt końcowy – serwer czasu

Adam Kowalczyk 215771



Środowisko programistyczne użyte do realizacji zadania: 
	Visual Studio 2017 Version 15.9.21
	Microsoft .NET Framework 4.6.1






Struktura folderów:

PS2020_projekt			<- folder główny
├───client				<- folder projektu aplikacji klienta
│   ├───bin
│   │   └───Debug
│   ├───obj
│   │   └───Debug
│   │       └───TempPE
│   └───Properties
└───serwer				<- folder projektu aplikacji serwera
    ├───bin
    │   └───Debug
    ├───obj
    │   └───Debug
    │       └───TempPE
    └───Properties






Fragmenty z internetu:

	"UdpClient Class"
		https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.udpclient?view=netcore-3.1
		użyte w:

		client:
			DiscoverySender.SendSetup
			DiscoverySender.SendLoop
			DiscoverySender.ListenSetup
			DiscoverySender.ListenLoop

		serwer:
			DiscoveryListener.StartServer
			DiscoveryListener.Loop

	"Use UDP services"
		https://docs.microsoft.com/en-us/dotnet/framework/network-programming/using-udp-services
		użyte w:

		serwer:
			DiscoveryListener.ListenLoop

