## EF Core Toolbox

[![Build status](https://ci.appveyor.com/api/projects/status/4f1pqkg8kfsuv9kr/branch/master?svg=true)](https://ci.appveyor.com/project/ilchenkob/ef-core-toolbox/branch/master)

It's already available at [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=VitaliiIlchenko.ef-core-toolbox)

You can find **change log [here](https://github.com/ilchenkob/ef-core-toolbox/blob/master/CHANGELOG.md)**

#### Description

EF Core Toolbox is an extension for Visual Studio 2017. It provides a set of tools to help you to avoid an executing console commands during Entity Framework Core based development. Since you do not have to have a deal with EF Core tools commands directly, you don't need to add `Microsoft.EntityFrameworkCore.Tools` package to your project.

Current version supports MS SQL Server database provider only.

This package contains the following tools:

1. **Database scaffolding** tool - provides GUI for [Scaffold-DbContext](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/powershell#scaffold-dbcontext) command.
2. **Add migration** tool - provides GUI for [Add-Migration](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/powershell#add-migration) command.
3. **Script migration** tool - provides GUI for [Script-Migration](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/powershell#script-migration) command.
4. **Update database** tool is coming soon! Stay tuned!

You can find them in Visual Studio **Tools** -> **EF Core Toolbox** menu.
