namespace App.Infrastructure.Mapping.Endpoints.Abstract;


// First, we need an abstraction that will represent our endpoints:
public interface IMinimalEndpoint
{
    void MapRoutes(IEndpointRouteBuilder routeBuilder);
}