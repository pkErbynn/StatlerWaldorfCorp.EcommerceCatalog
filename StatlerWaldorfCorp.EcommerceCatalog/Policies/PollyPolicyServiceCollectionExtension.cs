namespace StatlerWaldorfCorp.EcommerceCatalog.Policies
{
    public static class PollyPolicyServiceCollectionExtension
    {
        public static IServiceCollection AddPollyPolicyService(this IServiceCollection services)
        {
            services.AddSingleton<ClientPolicy> (s =>
            {
                return new ClientPolicy();
            });
            
            // services.AddSingleton<ClientPolicy> (new ClientPolicy());

            return services;
        }
    }
}