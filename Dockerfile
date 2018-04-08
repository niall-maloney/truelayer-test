# Build image
FROM microsoft/aspnetcore-build:2.0-stretch AS build  

WORKDIR /sln  
COPY ./true-layer-test.sln  ./

COPY ./src/TrueLayer.Model/TrueLayer.Model.csproj  ./src/TrueLayer.Model/TrueLayer.Model.csproj
COPY ./src/TrueLayer/TrueLayer.csproj  ./src/TrueLayer/TrueLayer.csproj
COPY ./src/TrueLayerTest/TrueLayerTest.csproj  ./src/TrueLayerTest/TrueLayerTest.csproj
RUN dotnet restore

COPY ./src ./src  
RUN dotnet build "./src/TrueLayerTest/TrueLayerTest.csproj" -c Release

RUN dotnet publish "./src/TrueLayerTest/TrueLayerTest.csproj" -c Release -o "../../dist"

#App image
FROM microsoft/aspnetcore:2.0-stretch  
WORKDIR /app
COPY --from=build /sln/dist .

ENTRYPOINT ["dotnet","TrueLayerTest.dll"]