using SupermarketSystem;
using SupermarketSystem.Data;

using (var context = new SupermarketContext())
{
    context.Database.EnsureCreated();
    SeedData.Initialise(context);
}

ApplicationConfiguration.Initialize();
Application.Run(new MainForm());