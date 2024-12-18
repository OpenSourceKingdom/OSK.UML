# OSK.UML
The project provides the core logic to creating Uml diagrams from a file or directory by using the Framework project as a basis. A `UmlParser` will utilize
a `IUmlDefinition` and an `IFileTokenReader` to parse files into respective Uml Diagrams. Consumers wanting to access the Uml Generator will need to add the dependencies
of the project to their dependency container by using the `AddUmlDiagrams` extension. A Uml generator implementation is provided by the library as well as an integration that 
utilizes PlantUml for exporting the uml diagram to a viewable file. The generator should be seen as the focal point of entry into the library, by using the `GenerateUmlAsync` 
function.

# OSK.UML.Framework
This project defines a set of interfaces and implementations that parse and track the state of `UmlElement`s as file tokens are processed. `UmlInterpreter`s work in conjunction with a `UmlDefinition` to parse file tokens
into a Uml Element. The definitions are meant to describe the different modifiers, types, and other descriptive information that can be encountered when parsing a programming language file. A `UmlDefinition` can be viewed as 
the way a programming language generally describes its syntax, by using a `UmlSyntaxRuleTemplate`. These templates allow an interpreter to read file tokens and refer to the rules referenced by the template to determine if a method, property, 
constructor, etc. has been encountered in a file as it is being processed. A base class `UmlDefinition` exists to provide an example and potential inheritance point for creating other definitions for other languages, and a default definition,
`DefaultUmlDefinition`, is provided that should support most C# language use cases. For an example of how to define a syntax template, we will look at one syntax template that could be used to describe a method in a class file:

```
new UmlSyntaxTemplate(SyntaxRuleType.Visibility, SyntaxRuleType.Modifiers,
                        SyntaxRuleType.Type, SyntaxRuleType.Name, SyntaxRuleType.Parameters)
                    {
                        ElementType = UmlElementType.Method
                    }
```

In the above, we can see that a template for a `method` has been provided which consistts of some specific `SyntaxRuleType`s. This is describing a tepmlate of defining a method that should be read in the following order:
`visibility` keywords, `modifier` keywords, `Type` name, object `Name`, and `Parameters`. Syntax rule types are designed to differentiate what each file token an interpreter is looking at belongs to in a given UmlElement diagram.
The keywords mentioned are also defined in a `IUmlDefinition` and are referenced by the interpreter to correctly identify and place them on the resulting Uml diagram that is generated. The `UmlSyntaxTemplate`s defined in the default
utilize a constructor that will create the `UmlSyntaxRule` objects using standard C# conventions, such as allowing multiple keywords for modifiers, a singular response type or object name, etc. but there is an overload for the template
object that allows a consumer to specify their own custom syntax rules that are not tied to C#.

Key Note: the core classes available for use from this project, beyond the stated interfaces that can be used interchangeably within the core logic, are the `UmlDefinition` and `DefaultUmlDefinition` objects. Consumers should create
new definition files for other languages beyond C#, if they are needed, and the base class for definitions can hopefully offer an easier way to integrate with the library if it is necessary.

# OSK.UML.Exporters.PlantUML
The exporter project is meant to provide an implementation of an `IUmlExporter` that utilizes the PlantUml library. To use this exporter, consumers will want to add it using the `AddPlantUml`
dependency extension.

For more information regarding this library, you can visit https://plantuml.com/

# OSK.UML.CommandLine
The command line tool currently supports UML generations for C# projects, but should be configurable to target other languages/files based on user integration with required interfaces. The tool can be targeted to a file or directory and will generate a UML image for the files discovered. An example generation command below:

`umlgenerator -p path/to/directory/or/file -o /output/directory`

Note: When using the PlantUML integration for UML export, the path given should be an absolute path within the file system used or PlanUML will generate the related UML image into a directory that may not be caught by the tool. See https://plantuml.com/command-line for more information.

# Example UML output
![domainModel](https://github.com/OpenSourceKingdom/OSK.UML/assets/7662822/f35244e1-0380-437e-82a4-75f37765278f)

# Docker supprt
A docker image has been deployed to https://hub.docker.com/repository/docker/blankdev117/umlgenerator and can be used to generate UML diagrams within a container

# Contributions and Issues
Any and all contributions are appreciated! Please be sure to follow the branch naming convention OSK-{issue number}-{deliminated}-{branch}-{name} as current workflows rely on it for automatic issue closure. Please submit issues for discussion and tracking using the github issue tracker.