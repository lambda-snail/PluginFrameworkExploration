# Plugin Framework Exploration

A small test environment with a javascript engine, and javascript commands implemented in dotnet using a simple plugin framework.

# Main Ideas

- Dotnet project that executes simple JavaScript files.
- New commands for JS are implemented in c# using a simple plugin framework.
- Project purpose is to explore reflection and source code generation.

# How-To

1. Create a plugin by extending the ```IJSOperation``` interface.

2. Create a JS script in the scripts folder.

3. Run and enter the name of your script when the system promts for it.

That's it! If you did everything correctly the system will execute your script.

## Notes

* The ```Execute``` method does not return any value, but a return value can be defined by adding the ```Output``` attribute to a property, and assigning to this property before returning from the ```Execute``` method.

* Input parameters can be defined by tagging properties with the ```Input``` property. These properties will be set by the framework before running the ```Execute``` method.
    
* The ```Name``` property determines the name of the function that is visible from JS.

* Plugins must be defined using the ```partial``` keyword.

* You can use ```console.log``` in your script to print output to the screen.

# Roadmap

* Refactor the source generator
* Add support for non-primitive types
