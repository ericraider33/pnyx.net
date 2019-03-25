# pnyx.net
Pnyx is an open source project that brings all of your favorite unix tools like grep, sed and awk to your preferred programming language (C#). When using Pnyx in your project, you can built upon these venerable tools without calling external programs. With a flexible, fluent API, the Pnyx framework is easily extended and simple to verify with Unit tests.

[View Pnyx Project website](http://pnyx.me/)

## Library
Pnyx is offered as a libary for use in projects that have Extract - Transform - Load (ETL) needs. Pnyx is not intended to replace Unix tools, which are time tested, pervasive and reliable. Instead, it was built to provide file transformation tools to high level programming languages. Some of the benefits of Pnyx as a libary are:

* Business logic for transforming files remains in high-level program languages, and not in scripts
* Full support for tabular data
* Combine tabular operations with traditional line-based operations
* Business logic can be easily Unit Tested, when using Pnyx
* Pnyx is powerful and easily customized / extended
* Windows line-feeds are handled gracefully
* Removes reliance upon environment, and works on any OS
* Scalable and performant

## Install
To install, go to [Nuget.org](https://www.nuget.org/packages/pnyx.net/)
and download, or open Package Manager and type:

```Install-Package pnyx.net```

## Learn More
Ready to get start? [View Documentation for Library](http://pnyx.me/library)

## CMD
Pnyx can be used directly from the command-line. Use the CMD interface for quick, one-time usage or for prototyping. The pnyx.cmd project uses a YAML file of commands, which map to commands of the library's fluent API. As an alternative, C# scripts can be used in place of YAML. Whatever your preference, use the CMD to run ad-hoc Pnyx commands. Finally, CMD can be used to expose your project's custom filters and transforms to the command-line and scripting.

Ready to get started? [View Documentation for CMD](http://pnyx.me/cmd)
