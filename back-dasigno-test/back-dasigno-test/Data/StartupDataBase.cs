using Microsoft.EntityFrameworkCore;

namespace back_dasigno_test.Data
{
    public static class StartupDataBase
    {       
        public static async void InitializeMigration(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dataBaseContext = services.GetRequiredService<DataBaseContext>();
                dataBaseContext.Database.Migrate();
            }
        }

    }
}
