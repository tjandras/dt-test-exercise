# Digital Thinkers test exercise

## Running the app
SDK version used for development (`dotnet --list-sdks`): 5.0.205

Runtime used for testing the app (`dotnet --list-runtimes`): Microsoft.AspNetCore.App 5.0.8

After cloning the repo it can be run:
+ in VS Code: pressing F5 should be sufficient if the c# extension by Microsoft is installed,
+ Or using the command line: the `dotnet run` command should restore, build and run the application.

It will be accessible on this URL: `https://localhost:5001`

## Notes

The app was developed and tested on my machine running Manjaro, but also tested on a Windows PC.

### `api.http` file

It is a special file, which can be used in Visual Studio code, if the Rest Client extension is installed (created by Huachao Mao). It is basically a text file, in which HTTP requests can be declared and run. Similar to Postman, but right inside VS Code. I used this file to test my app.

Github page for the extension: [Rest client](https://github.com/Huachao/vscode-restclient)

### `.vscode` folder

Usually this folder is not checked into source control. I thought it might come handy, therefore it is here.
