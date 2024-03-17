# Challenge - Compact Folder

The objective is to create a command line application that can create a zip of a folder and its subfolders, excluding certain extensions, folders or file names.
The program also allows the output file to be generated to a local folder, copied to a fileshare or sent to SMTP/Email.
Develop the program using OOP and SOLID best practices, as well as respective unit and integration tests.

## Requirements

    Requirement #1

        As a user, I can invoke the application via the command line by passing the following arguments:

        - the folder to zip (e.g. C:\\temp)
        - the final name of the zip file (e.g. final.zip)
        - a list of extensions to exclude (e.g. .bmp, .jpg, .txt)
        - a list of directories to exclude (e.g. git, directory)
        - a list of files to exclude (e.g. file1, file2)
        - output type (e.g. localFile, filesShare, SMTP/Email)
        - optional parameters according to the type of output (e.g. fileshare path)

    Requirement #2

        All files and folders must be included in the output file in a ZIP file.

    Requirement #3

        Create an "outputs" design in which it is easy to develop new outputs in the future.

    Requirement #4

        Develop unit tests for the code, achieving as much coverage as possible.

    Requirement #5

        The application can be run on either .NET Core 2.1 or .NET Framework 4.8

        Language: C#
        IDE: Visual Studio or Visual Studio Code

## Project 
**Author: Pedro Leitão**

Because .Net Core 2.1 is deprecated and has some security issues, i included version 2.1.30 too.
So, this project has 3 targets: .Net Framework 4.8, .Net Core 2.1 and .Net Core 2.1.30

#### Tools:
- [Papercut SMTP](https://github.com/ChangemakerStudios/Papercut-SMTP): Client to test emails

#### Packages:
- [CommandLineParser](https://github.com/commandlineparser/commandline): For parsing cli arguments
- [Fluent Validation](https://github.com/FluentValidation): For the App Validations
- [Fluent Assertions](https://github.com/fluentassertions/fluentassertions): For Test Assertions
- [NSubstitute](https://github.com/nsubstitute/NSubstitute): For Test Mocking

### Preparation:

1. Clone this repository
2. In the terminal, open the cloned folder and execute:
```shell
dotnet restore
dotnet build
```
3. Configure appsettings.json inside src\CompactFolder.Cli with your e-mail settings
```json
{
  "AppSettings": {
    "LogLevel": "Critical",
    "EmailSettings": {
      "Server": "localhost",
      "Port": 25,
      "EnableSsl": false,
      "DeliveryMethod": "Network",
      "PickupDirectoryLocation": null,
      "FromEmail": "email@outlook.com",
      "UseDefaultCredentials": true,
      "UserName": "email@outlook.com",
      "PasswordSecretKey": "EmailPassword"
    }
  }
}
```
UseDefaultCredentials: If is set to true, it will not use Username and Password.
To use Username and Password "UseDefaultCredentials" must be set to false and
and you will need to define the password inside Secrets.json with the defined key in the appsetings.

```shell
dotnet user-secrets list
dotnet user-secrets set "EmailPassword" "your email password"
```

### First Execution:
Open "src\CompactFolder.Cli"

To publish and run, you will need to specify a .net framework:

- For .Net Framework 4.8
```shell
dotnet publish -c Release -f net48 /p:EnvironmentName=Production
dotnet run -c Release -f net48
```
- For .Net Core 2.1
```shell
dotnet publish -c Release -f netcoreapp2.1 /p:EnvironmentName=Production
dotnet run -c Release -f netcoreapp2.1
```
- For .Net Core 2.1.30
```shell
dotnet publish -c Release -f netcoreapp2.1.30 /p:EnvironmentName=Production
dotnet run -c Release -f netcoreapp2.1.30
```

When run the above command, you receive above message you are good to go.
```
CompactFolder.Cli 1.0.0+cd621fa977c943d346c1c442cb6ca60a53421885
Copyright (C) 2024 CompactFolder.Cli

ERROR(S):
  Required option 'i, inputPath' is missing.
  Required option 'o, outputFile' is missing.
  Required option 't, outputType' is missing.

  -i, --inputPath     Required. Input file path (e.g. C:\Temp)
  -o, --outputFile    Required. Output file name with extension (e.g. myfile.zip)
  --ex                File extension list to exclude (e.g. .bmp, .jpg, .txt)
  --ef                File name list to exclude (e.g. ficheiro1, filcheiro2)
  --ed                Directory list to exclude (e.g. git, diretório)
  -t, --outputType    Required. Output Type (e.g. localFile, filesShare, email)
  --emailTo           Destination Email (e.g. to@email.com)
  --sharedPath        Shared Path (e.g. \\Machine1\Folder)
  --destPath          Destination Path (e.g. C:\Folder)
  --help              Display this help screen.
  --version           Display version information.
```

### Execution exemples:
*Change the execution framework in the command for desired one.*

Create a folder called "CompactExample" in directory "C:\" with some files inside.

#### 1. LocalFile
After run the command bellow, you need to see a file called "file.zip" inside the C:\ directory with the created folder content.
```shell
dotnet run -c Release -f net48 -i "C:\CompactExample" -o file.zip -t localFile --destPath "C:\"
```

#### 2. FileShare
2.1. Create another folder in C:\ called "CompactExampleShare". 
2.2. Right click in the folder -> Properties and create a new share for this folder called "CompactExampleShare"
After run the command bellow, you need to see a file called "file.zip" inside the "\\\\[YOURMACHINE]\CompactExampleShare" with the created folder content.
** *Change [YOURMACHINE] to your machine name before run the command*
```shell
dotnet run -c Release -f net48 -i "C:\CompactExample" -o file.zip -t fileShare --sharedPath "\\[YOURMACHINE]\CompactExampleShare"
```

#### 1. Email
After run the command bellow, you need to receive an attachment called "file.zip" with the created folder content.
```shell
dotnet run -c Release -f net48 -i "C:\CompactExample" -o file.zip -t email --emailTo "email@email.com"
```