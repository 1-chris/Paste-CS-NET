# Super basic ASP.NET minimal API pastebin clone
Using SQLite for storage. Provided as a sample (and for my own future reference)

Built using .NET 6/ASP.NET Minimal API

# First setup after building:
Run in project folder: 
1. First ensure you have dotnet-ef installed: dotnet tool install --global dotnet-ef --version 6.*
2. dotnet ef migrations add m1 -o Migrations
2. dotnet ef database update

# Thanks
Medhat Elmasry for this useful short video showing how to use SQLite https://www.youtube.com/watch?v=JG2TeGBs8MU
