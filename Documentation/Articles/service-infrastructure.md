# Services

In this framework, services act as the main interface to the functionality of an application. If the application logic is split into fine grained services, it is easy to maintain, extend and test. The abstract class [Service](xref:HAF.Service) should be derived for all services and provides the service configuration interface.

All services can contribute to and retrive information from a global settings file using the [LoadConfiguration()](xref:HAF.IService.LoadConfiguration(HAF.ServiceConfiguration)) and [SaveConfiguration()](xref:HAF.IService.SaveConfiguration(HAF.ServiceConfiguration)) functions wich are used to access the [ServiceConfiguration](xref:HAF.ServiceConfiguration) instance. This class allows easy manipulation of the XML based storage.

Service dependencies are declared using [compound states](xref:HAF.CompoundState) and the internal state of the service is exported using [states](xref:HAF.State). These allow a unified expression of service relations and have some handy integrated features that makes them usable in bindings and commands. 

Service events are issued using [events](xref:HAF.Event). 

All events and states provide a visualization of the logic flow in the console (in debug builds).
