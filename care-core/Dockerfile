FROM mcr.microsoft.com/dotnet/core/aspnet:3.1  AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

WORKDIR  /app/resources/db
COPY ./resources/db  /app/resources/db

WORKDIR  /app/resources/documents
#COPY ./resources/documents  /app/resources/documents
RUN mkdir -p =/app/resources/documents

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["care-core.csproj", ""]
RUN dotnet restore "./care-core.csproj"
COPY . .
WORKDIR "/src/."

RUN dotnet build "care-core.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "care-core.csproj" -c Release -o /app/publish
	
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "care-core.dll"]