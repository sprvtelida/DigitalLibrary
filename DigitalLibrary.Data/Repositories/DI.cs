using DigitalLibrary.Data.Contracts.Repositories;
using DigitalLibrary.Models.Entities;
using DigitalLibrary.Models.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalLibrary.Data.Repositories
{
    public static class DI
    {
        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<ISortHelper<Book>, SortHelper<Book>>();

            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        }
    }
}
