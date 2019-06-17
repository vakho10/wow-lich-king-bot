# World of Warcraft - Lich King (v3.3.5a 12340) Bot
This is a basic World of Warcraft Bot. It uses DLL injection strategy to inject C# binary code into Wow client process. 
The injector is a C# application name Bootstrapper, which injects Loader.DLL intO Wow's client process. The Loader.DLL then bootstraps .NET runtime and launches WPF application. 

P.S. The project is not launching WPF well. I'll get back to it :) soon...
