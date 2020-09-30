using Microsoft.AspNetCore.Http;
using Ocelot.LoadBalancer.LoadBalancers;
using Ocelot.Responses;
using Ocelot.Values;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomLoadBalancerDemoApp
{
    public class CustomLoadBalancer : ILoadBalancer
    {
		private readonly Func<Task<List<Service>>> _services;
		private int serviceIndexWithBiggestPort;
		public CustomLoadBalancer(Func<Task<List<Service>>> services)
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
}
