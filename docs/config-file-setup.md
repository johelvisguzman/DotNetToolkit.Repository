**Config File Setup**

If you wish to use the configuration files to configure the repositories, please follow these steps:

Element | Description | Attributes
--------- | ------------ | ------------
defaultContextFactory | Defines the default repository context factory to use | Type string
interceptors | Accepts a collection of interceptor elements |
interceptor | Defines an interceptor with a type and optional parameters | Type string
parameters | Acceps a collection of parameter elements |
parameter | Defines a parameter with a type and a value | Type string and a Value string

**App.config**
In order for the repositories to read data from the App.config the user will need to add a custom section handler and a configuration section:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <!-- Configuration section-handler declaration area. -->
  <configSections>
    <section name="repository" type="DotNetToolkit.Repository.Internal.ConfigFile.ConfigurationSection, DotNetToolkit.Repository" />
    <!-- Other <section> and <sectionGroup> elements. -->
  </configSections>
  <!-- Configuration section settings area. -->
  <repository>
    <defaultContextFactory type="DotNetToolkit.Repository.InMemory.InMemoryRepositoryContextFactory, DotNetToolkit.Repository.InMemory" />
    <interceptors>
      <interceptor type="ExampleApplication.TestRepositoryInterceptor, ExampleApplication">
        <parameters>
          <parameter value="random parameter"/>
        </parameters>
      </interceptor>
    </interceptors>
  </repository>
</configuration>
```

**appsettings.config**
In order for the repositories to read data from the appsettings.json file the user will need to add a section to the file:

```json
{
  "repository": {
    "defaultContextFactory": {
      "type": "DotNetToolkit.Repository.InMemory.InMemoryRepositoryContextFactory, DotNetToolkit.Repository.InMemory"
    },
    "interceptors": [
      {
        "interceptor": {
          "type": "DotNetToolkit.Repository.Integration.Test.Data.TestRepositoryInterceptor, DotNetToolkit.Repository.Integration.Test",
          "parameters": [
            {
              "parameter": {
                "type": "System.String",
                "value": "random param"
              }
            },
            {
              "parameter": {
                "type": "System.Boolean",
                "value": "True"
              }
            }
          ]
        }
      }
    ]
  }
} 
```
