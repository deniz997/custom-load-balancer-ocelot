# Usage

This custom load balancer routes the request to the service that is registered to Consul and has the biggest port number. 

### Registering as a service
Ranking load balancer uses an extension method of IOcelotBuilder, which registers our ranking load balancer to the list of load balancers and provides chainability by returning the IOcelotBuilder. So that it can be added to the project as follows.

```C#
public void ConfigureServices(IServiceCollection services)
        {
            services.AddOcelot()
                .AddRankingLoadBalancer();
        }
```
### Adding to the ocelot.json

Custom Load balancer can be added to the route by setting the LoadBalancerOptions property of the route as follows.

```Json
"LoadBalancerOptions": {
        "Type": "CustomLoadBalancer"
    }
```

This project is based on Ocelot documentation,
for more information please visit and read: [Ocelot Documentation - Load Balancer](https://ocelot.readthedocs.io/en/latest/features/loadbalancer.html)