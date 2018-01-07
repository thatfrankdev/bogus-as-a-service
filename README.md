# Bogus as a service

This is a simple netcoreapp2.0 app exposing a subset of @bchavez's [**Bogus**](https://github.com/bchavez/Bogus) library APIs as a simple web service. This is more of a dynamic routing experiment than a real world production app, but some may find it usefull to generate fake data in some scenarios!

## Build it locally
```
git clone https://github.com/thatfrankdev/bogus-as-a-service.git
cd bogus-as-a-service
dotnet restore
dotnet run
```

## Usage
*[Play with it live](https://bogus-as-a-service.azurewebsites.net/) :zap:*

** *Note that when a parameters is optional in a route template (i.e. marked with a `?` after its name), you can provide it either in the path, or as a query parameter*

### Query parameters
* `seed` : Used to fix randomizer seed so that the random sequences generated are always the same. This must be an integer number.
* `locale` : Used to set the locale of the inner datasets to be used. Supported locales are documented listed [here](https://github.com/bchavez/Bogus#locales)

### Examples
* [/internet/email](https://bogus-as-a-service.azurewebsites.net/internet/email)
* [/finance/amount?max=700](https://bogus-as-a-service.azurewebsites.net/finance/amount?max=700)
* [/image/transport/300/400/true/true](https://bogus-as-a-service.azurewebsites.net/image/transport/300/400/true/true)
* [/lorem/words/22?seed=935](https://bogus-as-a-service.azurewebsites.net/lorem/words/22?seed=935)
* [/name/firstname?locale=cz](https://bogus-as-a-service.azurewebsites.net/name/firstname?locale=cz)
