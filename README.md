# HalfLink - A URL Shortener

HalfLink is part URL shortener and part learning exercise. It exists as a personal project for learning Azure in more depth. This repository is here both for posterity and to help others who are learning similar cloud technologies. The project was a fun one, and I [wrote a retrospective](RETROSPECTIVE.md) about the experience for anyone who is interested.

## Requirements & Installation

To run the application locally, you will need:

- [Docker](https://docs.docker.com/get-started/get-docker/)
- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [Azure Functions Core Tools v4](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local)

### Download the Code

[Download a copy of the code](https://github.com/walkersoft/halflink/archive/refs/heads/master.zip), or use Git to clone the repository.

```cli
git clone https://github.com/walkersoft/halflink.git
```

### Import the Cosmos Emulator SSL Certificate

The Docker environment must be up and running to obtain the SSL certificate. From the repository root, run the following commands.

```cli
cd docker
docker compose up -d
```

Next, follow the instructions on [Microsoft's Cosmos DB emulator documentation](https://learn.microsoft.com/en-us/azure/cosmos-db/how-to-develop-emulator?tabs=docker-linux%2Ccsharp&pivots=api-nosql#import-the-emulators-tlsssl-certificate). This project uses the Linux-based Cosmos DB emulator image. The linked documentation covers extracting and installing the certificate on both Windows and Linux systems.

## Running the Application

Once the emulated services are running, you can start the application from either Visual Studio or the command line.

### Visual Studio

- Right-click the `HalfLink.slnx` solution file and select `Properties`
- Under **Common Properties** -> **Configured Startup Projects**, set the following projects' **Action** to **Start**
  - `HalfLink.Functions`
  - `HalfLink.UI`
- Press **Apply**, then **OK**
- Start the application

### Command Line

Run the following commands from the repository root in **separate** terminals.

**Functions App**

```cli
cd ./src/HalfLink.Functions
func start
```

**Blazor Server UI**

```cli
cd ./src/HalfLink.UI
dotnet run -lp https
```

Once both applications are running, navigate to `https://localhost:7269` in your browser.

You should be ready to roll after that!
