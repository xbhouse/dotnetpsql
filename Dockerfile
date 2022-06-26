FROM registry.access.redhat.com/ubi8/dotnet-60-runtime:latest AS base
WORKDIR /app
EXPOSE 5000

FROM registry.access.redhat.com/ubi8/dotnet-60:latest AS build
USER root
WORKDIR /src
COPY ["NotesMinimalAPI/NotesMinimalAPI.csproj", "NotesMinimalAPI/"]
RUN dotnet restore "NotesMinimalAPI/NotesMinimalAPI.csproj"
COPY . .
WORKDIR "/src/NotesMinimalAPI"
RUN dotnet build "NotesMinimalAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NotesMinimalAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotesMinimalAPI.dll"]
