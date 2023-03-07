# Super basic ASP.NET minimal API pastebin clone
Using SQLite for storage.

# First setup after building:
Run in project folder: 
1. dotnet ef migrations add m1 -o Migrations
2. dotnet ef database update
If those don't work, try this first: dotnet tool install --global dotnet-ef --version 6.*

# Thanks to: Medhat Elmasry for this video https://www.youtube.com/watch?v=JG2TeGBs8MU