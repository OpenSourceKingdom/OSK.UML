# OSK.UML
A configurable library for creating UML diagrams from files or directories. 

The command line tool currently supports UML generations for C# projects, but should be configurable to target other languages/files based on user integration with required interfaces. The tool can be targeted to a file or directory and will generate a UML image for the files discovered. An example generation command below:

`umlgenerator -p path/to/directory/or/file -o /output/directory`

Note: When using the PlantUML integration for UML export, the path given should be an absolute path within the file system used or PlanUML will generate the related UML image into a directory that may not be caught by the tool. See https://plantuml.com/command-line for more information.

# Example UML output
![domainModel](https://github.com/OpenSourceKingdom/OSK.UML/assets/7662822/f35244e1-0380-437e-82a4-75f37765278f)

# Docker supprt
A docker image has been deployed to https://hub.docker.com/repository/docker/blankdev117/umlgenerator and can be used to generate UML diagrams within a container
