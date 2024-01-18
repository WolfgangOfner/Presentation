FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AzureArcDemo/AzureArcDemo.csproj", "AzureArcDemo/"]
COPY ["AzureArcDemo.Test/AzureArcDemo.Test.csproj", "AzureArcDemo.Test/"]

RUN dotnet restore "AzureArcDemo/AzureArcDemo.csproj"
RUN dotnet restore "AzureArcDemo.Test/AzureArcDemo.Test.csproj"
COPY . .

RUN dotnet build "AzureArcDemo/AzureArcDemo.csproj" -c Release -o /app/build --no-restore
RUN dotnet build "AzureArcDemo.Test/AzureArcDemo.Test.csproj" -c Release --no-restore

FROM build AS test  
ARG BuildId=localhost
LABEL test=${BuildId}
RUN dotnet test --no-build -c Release --results-directory /testresults --logger "trx;LogFileName=test_results.trx" /p:CollectCoverage=true /p:CoverletOutputFormat=json%2cCobertura /p:CoverletOutput=/testresults/coverage/ -p:MergeWith=/testresults/coverage/coverage.json  AzureArcDemo.Test/AzureArcDemo.Test.csproj  

FROM build AS publish
RUN dotnet publish "AzureArcDemo/AzureArcDemo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AzureArcDemo.dll"]