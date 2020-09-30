using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Configuration;
using Ocelot.DependencyInjection;
using Ocelot.LoadBalancer.LoadBalancers;
using Ocelot.Responses;
using Ocelot.ServiceDiscovery.Providers;
using Ocelot.Values;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CustomLoadBalancerDemoApp
{
    public class RankingLoadBalancer : ILoadBalancer
    {
		private readonly Func<Task<List<Service>>> _services;
		private int serviceIndexWithBiggestPort;
		public RankingLoadBalancer(Func<Task<List<Service>>> services)
		{
			_services = services;

		}

		public async Task<Response<ServiceHostAndPort>> Lease(HttpContext httpContext)
		{

			var services = await _services();
			Console.WriteLine("Arrived at custom load balancer");
			if (services.Count > 0)
			{
				serviceIndexWithBiggestPort = 0;

				for (var i = 0; i < services.Count - 1; i++)
				{
					if (services[i].HostAndPort.DownstreamPort < services[i + 1].HostAndPort.DownstreamPort)
					{
						serviceIndexWithBiggestPort = i + 1;

					}
				}
			}
			return new OkResponse<ServiceHostAndPort>(services[serviceIndexWithBiggestPort].HostAndPort);
		}

		public void Release(ServiceHostAndPort hostAndPort)
		{
		}
	}
	public static class IOcelotBuilderExtensions
	{
		public static IOcelotBuilder AddRankingLoadBalancer(this IOcelotBuilder ocelot)
		{
			ocelot.AddCustomLoadBalancer((Route, serviceDiscoveryProvider) => new RankingLoadBalancer(serviceDiscoveryProvider.Get));
			return ocelot;
		}
	}
}
