﻿using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using R5T.Dacia.Internals;


namespace R5T.Dacia
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection RunServiceAction<TService>(this IServiceCollection services, IServiceAction<TService> serviceAction)
        {
            serviceAction.Run(services);

            return services;
        }

        /// <summary>
        /// Quality-of-life overload for <see cref="IServiceCollectionExtensions.RunServiceAction{T}(IServiceCollection, IServiceAction{T})"/>.
        /// </summary>
        public static IServiceCollection Run<TService>(this IServiceCollection services, IServiceAction<TService> serviceAction)
        {
            services.RunServiceAction(serviceAction);

            return services;
        }

        public static IServiceCollection RunServiceActions<TService>(this IServiceCollection services, IEnumerable<IServiceAction<TService>> serviceActions)
        {
            foreach (var serviceAction in serviceActions)
            {
                services.RunServiceAction(serviceAction);
            }

            return services;
        }

        /// <summary>
        /// Reruns an <see cref="IServiceAction{TService}"/>.
        /// Service actions are designed to only run once. This method allows re-running a service action.
        /// </summary>
        public static IServiceCollection Rerun<TService>(this IServiceCollection services, IServiceAction<TService> serviceAction)
        {
            // Access the service action's action directly, and run it.
            serviceAction.Action(services);

            return services;
        }
    }
}


namespace R5T.Dacia.Extensions
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Allows separation of code-block for adding multiple services.
        /// Does not do anything special, just serves to separate code for adding the services for a multiple service.
        /// </summary>
        public static IServiceCollection AddMultipleServices(this IServiceCollection services, Action<IServiceCollection> action)
        {
            action(services);

            return services;
        }

        /// <summary>
        /// Allows fluent separation of a code-block for adding services.
        /// </summary>
        public static IServiceCollection AddServices(this IServiceCollection services, Action<IServiceCollection> action)
        {
            action(services);

            return services;
        }

        public static IServiceProvider BuildIntermediateServiceProvider(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }

        /// <summary>
        /// Get a service out of the current state of the service collection.
        /// </summary>
        /// <remarks>
        /// Build a service provider from the current state of the service collection and get a required service.
        /// </remarks>
        public static T GetIntermediateRequiredService<T>(this IServiceCollection services)
        {
            var intermediateServiceProvider = services.BuildServiceProvider();

            var output = intermediateServiceProvider.GetRequiredService<T>();
            return output;
        }

        /// <summary>
        /// Adds the <typeparamref name="TImplementation"/> instance as a singleton instance (if not null), else adds the <typeparamref name="TImplementation"/> as a service type implementation.
        /// </summary>
        public static IServiceCollection AddSingletonAsTypeIfInstanceNull<TService, TImplementation>(this IServiceCollection services, TImplementation instance)
            where TService : class
            where TImplementation : class, TService
        {
            var instanceIsNullService = ServiceHelper.IsNullService(instance);
            if (instanceIsNullService)
            {
                services.AddSingleton<TService, TImplementation>();
            }
            else
            {
                services.AddSingleton<TService>(instance);
            }

            return services;
        }

        public static IServiceCollection TryAddSingletonFluent<TService>(this IServiceCollection services)
            where TService : class
        {
            services.TryAddSingleton<TService>();

            return services;
        }

        public static IServiceCollection TryAddSingletonFluent<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.TryAddSingleton<TService, TImplementation>();

            return services;
        }

        /// <summary>
        /// Adds services for a multiple service in a way that allows getting services via <see cref="IServiceProviderExtensions.GetMultipleService{TService}(IServiceProvider)"/>.
        /// </summary>
        public static IServiceCollection AddSingletonMultipleService<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services
                .AddSingleton<TImplementation>()
                .AddSingleton<IMultipleServiceHolder<TService>, MultipleServiceHolder<TImplementation>>();

            return services;
        }
    }
}
