# Super basic ASP.NET minimal API pastebin clone
Using SQLite for storage. Provided as a sample (and for my own future reference)

# First setup after building:
Run in project folder: 
1. dotnet ef migrations add m1 -o Migrations
2. dotnet ef database update
3. If those don't work, try this first: dotnet tool install --global dotnet-ef --version 6.*

# Thanks
Medhat Elmasry for this useful short video showing how to use SQLite https://www.youtube.com/watch?v=JG2TeGBs8MU